-- UC18: Create Users table
-- Run this ONCE in SSMS to create the table

USE QuantityMeasurementDB;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' and xtype='U')
BEGIN
    CREATE TABLE Users (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Email NVARCHAR(256) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        GoogleId NVARCHAR(256) NULL
    );
    PRINT 'Created Users table.';
END
ELSE
BEGIN
    PRINT 'Users table already exists.';
END
GO
