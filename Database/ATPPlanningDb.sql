CREATE DATABASE ATPPlanningDb;
GO
USE ATPPlanningDb;
GO

CREATE TABLE Inventories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL,
    LocationCode NVARCHAR(50) NOT NULL,
    OnHandQuantity INT NOT NULL CHECK (OnHandQuantity >= 0),
    ReservedQuantity INT NOT NULL CHECK (ReservedQuantity >= 0),
    UpdatedAtUtc DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Inventories_Product_Location UNIQUE(ProductCode, LocationCode)
);

CREATE TABLE IncomingSupplies (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ProductCode NVARCHAR(50) NOT NULL,
    LocationCode NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    ExpectedDate DATETIME2 NOT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Open'
);

CREATE TABLE CustomerOrders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    OrderNumber NVARCHAR(50) NOT NULL,
    ProductCode NVARCHAR(50) NOT NULL,
    LocationCode NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    RequiredDate DATETIME2 NOT NULL,
    Status NVARCHAR(30) NOT NULL DEFAULT 'Open'
);

INSERT INTO Inventories(ProductCode, LocationCode, OnHandQuantity, ReservedQuantity)
VALUES ('SKU-1001', 'MUM-WH1', 100, 20);

INSERT INTO IncomingSupplies(ProductCode, LocationCode, Quantity, ExpectedDate, Status)
VALUES ('SKU-1001', 'MUM-WH1', 50, DATEADD(day, 2, CAST(GETDATE() AS date)), 'Open');

INSERT INTO CustomerOrders(OrderNumber, ProductCode, LocationCode, Quantity, RequiredDate, Status)
VALUES ('ORD-1001', 'SKU-1001', 'MUM-WH1', 15, CAST(GETDATE() AS date), 'Open');
GO
