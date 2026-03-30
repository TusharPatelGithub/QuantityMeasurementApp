-- UC17: ALTER TABLE script
-- Run this ONCE in SSMS before starting the application
-- Adds IsError and ErrorMessage columns to the existing Measurements table

USE QuantityMeasurementDB;
GO

-- Add IsError column (0 = success, 1 = error)
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Measurements' AND COLUMN_NAME = 'IsError'
)
BEGIN
    ALTER TABLE Measurements ADD IsError BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsError column.';
END
ELSE
BEGIN
    PRINT 'IsError column already exists.';
END
GO

-- Add ErrorMessage column
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Measurements' AND COLUMN_NAME = 'ErrorMessage'
)
BEGIN
    ALTER TABLE Measurements ADD ErrorMessage NVARCHAR(500) NULL;
    PRINT 'Added ErrorMessage column.';
END
ELSE
BEGIN
    PRINT 'ErrorMessage column already exists.';
END
GO

-- Verify the columns were added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Measurements'
ORDER BY ORDINAL_POSITION;
GO
