using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Post.API.Configurations;
using Post.API.Contracts;
using Post.API.Data;
using Post.API.Repository;
using Post.API.Data;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);
});
// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("UserDbConnectionString");
builder.Services.AddDbContext<PostDbContext>(options => 
    options.UseSqlServer(connectionString));

// AutoMapper and Repository
builder.Services.AddAutoMapper(typeof(MapperConfig));

// JWT Authentication - For validating tokens from User service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
        };
    });

builder.Services.AddScoped<IPostRepository, PostRepository>();


// Authorization Policies
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("ADMIN"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("USER"));
});

// CORS Configuration
builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", builder => 
        builder.SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// API Controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// // Initialize Database (optional)
// using (var scope = app.Services.CreateScope())
// {
//     var dbContext = scope.ServiceProvider.GetRequiredService<PostDbContext>();
//     var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
//
//     try
//     {
//         dbContext.Database.Migrate();
//         logger.LogInformation("Database migrated successfully");
//     }
//     catch (Exception ex)
//     {
//         logger.LogError(ex, "An error occurred while migrating the database");
//     }
// }
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PostDbContext>();
    context.Database.Migrate();
}

// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();