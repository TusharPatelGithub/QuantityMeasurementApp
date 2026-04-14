-- ============================================================
-- PostgreSQL Schema for QuantityMeasurementApp
-- Run this ONCE on your PostgreSQL database (Supabase/Railway/Render)
-- before starting the application.
-- ============================================================

-- Create Measurements table
CREATE TABLE IF NOT EXISTS "Measurements" (
    "Id"              SERIAL PRIMARY KEY,
    "MeasurementType" VARCHAR(100)   NOT NULL,
    "OperationType"   VARCHAR(100)   NOT NULL,
    "Value1"          DOUBLE PRECISION NOT NULL,
    "Value2"          DOUBLE PRECISION NOT NULL,
    "Result"          DOUBLE PRECISION NOT NULL,
    "Unit"            VARCHAR(50)    NOT NULL,
    "CreatedAt"       TIMESTAMP      NOT NULL DEFAULT NOW(),
    "IsError"         BOOLEAN        NOT NULL DEFAULT FALSE,
    "ErrorMessage"    VARCHAR(500)   NULL
);

-- Create Users table
CREATE TABLE IF NOT EXISTS "Users" (
    "Id"           SERIAL          PRIMARY KEY,
    "FullName"     VARCHAR(100)    NOT NULL DEFAULT '',
    "Email"        VARCHAR(256)    NOT NULL UNIQUE,
    "PasswordHash" TEXT            NOT NULL DEFAULT '',
    "MobileNumber" VARCHAR(10)     NOT NULL DEFAULT '',
    "GoogleId"     VARCHAR(256)    NULL
);

-- Verify tables
SELECT table_name FROM information_schema.tables
WHERE table_schema = 'public'
ORDER BY table_name;
