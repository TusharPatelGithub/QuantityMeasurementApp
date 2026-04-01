-- UC19: Alter Users table to add FullName and MobileNumber
-- Run this ONCE in SSMS

USE QuantityMeasurementDB;
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Users') AND name = 'FullName')
BEGIN
    ALTER TABLE Users ADD FullName NVARCHAR(100) NOT NULL DEFAULT '';
    PRINT 'Added FullName column.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Users') AND name = 'MobileNumber')
BEGIN
    ALTER TABLE Users ADD MobileNumber NVARCHAR(10) NOT NULL DEFAULT '';
    PRINT 'Added MobileNumber column.';
END
GO
