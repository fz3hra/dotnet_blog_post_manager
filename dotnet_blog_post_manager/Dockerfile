FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["dotnet_blog_post_manager/dotnet_blog_post_manager.csproj", "dotnet_blog_post_manager/"]
RUN dotnet restore "dotnet_blog_post_manager/dotnet_blog_post_manager.csproj"
COPY . .
WORKDIR "/src/dotnet_blog_post_manager"
RUN dotnet build "dotnet_blog_post_manager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "dotnet_blog_post_manager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dotnet_blog_post_manager.dll"]
