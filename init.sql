BEGIN TRY
    -- Create UserDb if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'UserDb')
BEGIN
        CREATE DATABASE UserDb;
        PRINT '✅ UserDb created successfully';
END
ELSE
BEGIN
        PRINT '✅ UserDb already exists';
END;

    -- Create BlogDb if it doesn't exist
    IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BlogDb')
BEGIN
        CREATE DATABASE BlogDb;
        PRINT '✅ BlogDb created successfully';
END
ELSE
BEGIN
        PRINT '✅ BlogDb already exists';
END;
END TRY
BEGIN CATCH
PRINT '❌ Error occurred:';
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH
GO