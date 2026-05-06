USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskManagementDB')
BEGIN
    CREATE DATABASE TaskManagementDB COLLATE SQL_Latin1_General_CP1_CI_AS;
END
GO

USE TaskManagementDB;
GO

-- Tablas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(200) NOT NULL UNIQUE,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tasks')
BEGIN
    CREATE TABLE Tasks (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NULL,
        Status INT NOT NULL DEFAULT 0,
        AssignedUserId UNIQUEIDENTIFIER NOT NULL REFERENCES Users(Id),
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AdditionalInfo NVARCHAR(MAX) NULL,
        CONSTRAINT CK_Tasks_AdditionalInfo_IsJson CHECK (AdditionalInfo IS NULL OR ISJSON(AdditionalInfo) = 1)
    );
END
GO

-- Índices
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_Status')
    CREATE INDEX IX_Tasks_Status ON Tasks (Status) INCLUDE (Id, Title, AssignedUserId, CreatedAt);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_AssignedUserId')
    CREATE INDEX IX_Tasks_AssignedUserId ON Tasks (AssignedUserId) INCLUDE (Id, Title, Status, CreatedAt);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tasks_CreatedAt')
    CREATE INDEX IX_Tasks_CreatedAt ON Tasks (CreatedAt DESC);
GO

-- Vista
CREATE OR ALTER VIEW vw_TasksWithJsonDetails AS
SELECT
    t.Id, t.Title, t.Description,
    CASE t.Status WHEN 0 THEN 'Pending' WHEN 1 THEN 'InProgress' ELSE 'Done' END AS StatusText,
    u.Name AS AssignedUserName, t.CreatedAt,
    JSON_VALUE(t.AdditionalInfo, '$.priority') AS Priority,
    JSON_VALUE(t.AdditionalInfo, '$.estimatedEndDate') AS EstimatedEndDate
FROM Tasks t
INNER JOIN Users u ON t.AssignedUserId = u.Id;
GO
