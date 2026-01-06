CREATE DATABASE agri_supplychain;
GO
USE agri_supplychain;
GO

-- 1. USERS
CREATE TABLE dbo.users (
    id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(100) NOT NULL UNIQUE,
    password_hash NVARCHAR(255) NOT NULL,
    full_name NVARCHAR(200),
    email NVARCHAR(255),
    phone NVARCHAR(50),
	password_reset_token NVARCHAR(100) NULL,
    password_reset_expiry DATETIME2 NULL,
    role NVARCHAR(20) NOT NULL CHECK (role IN ('admin','farmer','distributor','retailer')),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 2. FARMS
CREATE TABLE dbo.farms (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    owner_name NVARCHAR(200),
    location NVARCHAR(500),
    contact_info NVARCHAR(500),
    certifications NVARCHAR(MAX),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 3. PRODUCTS
CREATE TABLE dbo.products (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    sku NVARCHAR(100) NULL,
    category NVARCHAR(100),
    storage_instructions NVARCHAR(500),
    typical_shelf_life_days INT NULL,
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 4. BATCHES
CREATE TABLE dbo.batches (
    id INT IDENTITY(1,1) PRIMARY KEY,
    batch_code NVARCHAR(100) NOT NULL UNIQUE,
    product_id INT NOT NULL,
    farm_id INT NOT NULL,
    created_by_user_id INT NULL,
    harvest_date DATE,
    quantity DECIMAL(12,3) NULL,
    unit NVARCHAR(50) DEFAULT 'kg',
    expiry_date DATE NULL,
    status NVARCHAR(30) NOT NULL DEFAULT 'pending' 
        CHECK (status IN ('pending','stored','in_transit','delivered','expired','rejected')),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT fk_batches_product FOREIGN KEY (product_id) REFERENCES dbo.products(id) ON DELETE CASCADE,
    CONSTRAINT fk_batches_farm FOREIGN KEY (farm_id) REFERENCES dbo.farms(id) ON DELETE CASCADE,
    CONSTRAINT fk_batches_createdby FOREIGN KEY (created_by_user_id) REFERENCES dbo.users(id) ON DELETE SET NULL
);
GO

-- 5. INSPECTIONS
CREATE TABLE dbo.inspections (
    id INT IDENTITY(1,1) PRIMARY KEY,
    batch_id INT NOT NULL,
    inspector_user_id INT NULL,
    inspection_date DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    humidity DECIMAL(6,2) NULL,
    temperature DECIMAL(6,2) NULL,
    chemical_residue DECIMAL(8,4) NULL,
    quality_score INT NULL,
    report_file NVARCHAR(255),
    signature_id INT NULL,
    notes NVARCHAR(MAX),

    CONSTRAINT fk_inspections_batch FOREIGN KEY (batch_id) REFERENCES dbo.batches(id) ON DELETE CASCADE,
    CONSTRAINT fk_inspections_inspector FOREIGN KEY (inspector_user_id) REFERENCES dbo.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_inspections_signature FOREIGN KEY (signature_id) REFERENCES dbo.digital_signatures(id) ON DELETE SET NULL
);
GO

-- 6. DIGITAL_SIGNATURES
CREATE TABLE dbo.digital_signatures (
    id INT IDENTITY(1,1) PRIMARY KEY,
    signer_user_id INT NULL,
    signature_method NVARCHAR(100),
    signature_value NVARCHAR(MAX),
    signed_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    reference_document NVARCHAR(255),
    notes NVARCHAR(MAX),

    CONSTRAINT fk_digsig_user FOREIGN KEY (signer_user_id) REFERENCES dbo.users(id) ON DELETE SET NULL
);
GO

-- 7. WAREHOUSES
CREATE TABLE dbo.warehouses (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    location NVARCHAR(500),
    contact_info NVARCHAR(500),
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 8. DISTRIBUTORS
CREATE TABLE dbo.distributors (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    contact_info NVARCHAR(500),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 9. RETAILERS
CREATE TABLE dbo.retailers (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(200) NOT NULL,
    location NVARCHAR(500),
    contact_info NVARCHAR(500),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO

-- 10. WAREHOUSE_STOCK
CREATE TABLE dbo.warehouse_stock (
    id INT IDENTITY(1,1) PRIMARY KEY,
    warehouse_id INT NOT NULL,
    batch_id INT NOT NULL,
    quantity DECIMAL(12,3) NOT NULL,
    unit NVARCHAR(50) DEFAULT 'kg',
    last_updated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT fk_stock_warehouse FOREIGN KEY (warehouse_id) REFERENCES dbo.warehouses(id) ON DELETE CASCADE,
    CONSTRAINT fk_stock_batch FOREIGN KEY (batch_id) REFERENCES dbo.batches(id) ON DELETE CASCADE,
    CONSTRAINT uq_warehouse_batch UNIQUE (warehouse_id, batch_id)
);
GO

-- 11. RETAILER_STOCK
CREATE TABLE dbo.retailer_stock (
    id INT IDENTITY(1,1) PRIMARY KEY,
    retailer_id INT NOT NULL,
    batch_id INT NOT NULL,
    quantity DECIMAL(12,3) NOT NULL,
    unit NVARCHAR(50) DEFAULT 'kg',
    last_updated DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT fk_retailerstock_retailer FOREIGN KEY (retailer_id) REFERENCES dbo.retailers(id) ON DELETE CASCADE,
    CONSTRAINT fk_retailerstock_batch FOREIGN KEY (batch_id) REFERENCES dbo.batches(id) ON DELETE CASCADE,
    CONSTRAINT uq_retailer_batch UNIQUE (retailer_id, batch_id)
);
GO

-- 12. SHIPMENTS
CREATE TABLE dbo.shipments (
    id INT IDENTITY(1,1) PRIMARY KEY,
    shipment_code NVARCHAR(100) NOT NULL UNIQUE,
    planned_by_user_id INT NULL,
    distributor_id INT NULL,
    from_type NVARCHAR(50) NOT NULL CHECK (from_type IN ('farm','warehouse','distributor')),
    from_id INT NULL,
    to_type NVARCHAR(50) NOT NULL CHECK (to_type IN ('warehouse','distributor','retailer')),
    to_id INT NULL,
    vehicle_info NVARCHAR(255),
    driver_info NVARCHAR(255),
    departure_time DATETIME2 NULL,
    arrival_time DATETIME2 NULL,
    status NVARCHAR(30) NOT NULL DEFAULT 'pending' CHECK (status IN ('pending','in_transit','arrived','cancelled')),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    notes NVARCHAR(MAX),

    CONSTRAINT fk_shipments_planner FOREIGN KEY (planned_by_user_id) REFERENCES dbo.users(id) ON DELETE SET NULL,
    CONSTRAINT fk_shipments_distributor FOREIGN KEY (distributor_id) REFERENCES dbo.distributors(id) ON DELETE SET NULL
);
GO

-- 13. SHIPMENT_ITEMS
CREATE TABLE dbo.shipment_items (
    id INT IDENTITY(1,1) PRIMARY KEY,
    shipment_id INT NOT NULL,
    batch_id INT NOT NULL,
    quantity DECIMAL(12,3) NOT NULL,
    unit NVARCHAR(50) DEFAULT 'kg',

    CONSTRAINT fk_shipmentitems_shipment FOREIGN KEY (shipment_id) REFERENCES dbo.shipments(id) ON DELETE CASCADE,
    CONSTRAINT fk_shipmentitems_batch FOREIGN KEY (batch_id) REFERENCES dbo.batches(id) ON DELETE CASCADE
);
GO

-- 14. QR_CODES
CREATE TABLE dbo.qr_codes (
    id INT IDENTITY(1,1) PRIMARY KEY,
    batch_id INT NOT NULL UNIQUE,
    token NVARCHAR(255) NOT NULL UNIQUE,
    url NVARCHAR(500),
    generated_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT fk_qr_batch FOREIGN KEY (batch_id) REFERENCES dbo.batches(id) ON DELETE CASCADE
);
GO

-- 15. AUDIT_LOGS
CREATE TABLE dbo.audit_logs (
    id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NULL,
    action NVARCHAR(50) NOT NULL,
    table_name NVARCHAR(128),
    row_id NVARCHAR(128),
    old_value NVARCHAR(MAX),
    new_value NVARCHAR(MAX),
    created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT fk_audit_user FOREIGN KEY (user_id) REFERENCES dbo.users(id) ON DELETE SET NULL
);
GO
-----------------------------------------------------------------------------------
--Procedure
-------------------------User--------------------------
--lấy tất cả user
CREATE PROCEDURE GetAllUsers
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        id,
        username,
        password_hash,
        full_name,
        email,
        phone,
        role,
        created_at
    FROM dbo.users;
END
--Lấy theo ID
CREATE PROCEDURE GetUserById
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        id,
        username,
        password_hash,
        full_name,
        email,
        phone,
        role,
        created_at,
        password_reset_token,
        password_reset_expiry
    FROM dbo.users
    WHERE id = @UserId;
END

--Lấy theo tên
CREATE PROCEDURE SP_Login
    @Username NVARCHAR(100)
AS
BEGIN
    SELECT *
    FROM users
    WHERE username = @Username;
END
--Đăng ký user mới
CREATE PROCEDURE SP_RegisterUser
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(255),
    @FullName NVARCHAR(200) = NULL,
    @Email NVARCHAR(255) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Role NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO dbo.users (username, password_hash, full_name, email, phone, role, created_at)
    VALUES (@Username, @PasswordHash, @FullName, @Email, @Phone, @Role, GETDATE());
END
--Cập nhật user
CREATE PROCEDURE UpdateUser
    @UserId INT,
    @FullName NVARCHAR(200) = NULL,
    @Email NVARCHAR(255) = NULL,
    @Phone NVARCHAR(50) = NULL,
    @Role NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE dbo.users
    SET 
        full_name = COALESCE(@FullName, full_name),
        email = COALESCE(@Email, email),
        phone = COALESCE(@Phone, phone),
        role = COALESCE(@Role, role)
    WHERE id = @UserId;
END
--Xoá user
CREATE PROCEDURE DeleteUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM dbo.users
    WHERE id = @UserId;
END
--Đổi mật khẩu
CREATE PROCEDURE SP_ChangePassword
    @UserId INT,
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    UPDATE users
    SET password_hash = @NewPasswordHash
    WHERE id = @UserId
END
--Lấy user
CREATE PROCEDURE SP_GetUserByUsername
    @Username NVARCHAR(100)
AS
BEGIN
    SELECT * FROM users WHERE username = @Username
END
--Quên mật khẩu
CREATE PROCEDURE SP_SetPasswordResetToken
    @Email NVARCHAR(255),
    @ResetToken NVARCHAR(100),
    @Expiry DATETIME2
AS
BEGIN
    UPDATE users
    SET password_reset_token = @ResetToken,
        password_reset_expiry = @Expiry
    WHERE email = @Email
END
--Reset mật khẩu = token
CREATE PROCEDURE SP_ResetPassword
    @ResetToken NVARCHAR(100),
    @NewPasswordHash NVARCHAR(255)
AS
BEGIN
    UPDATE users
    SET password_hash = @NewPasswordHash,
        password_reset_token = NULL,
        password_reset_expiry = NULL
    WHERE password_reset_token = @ResetToken
      AND password_reset_expiry > SYSUTCDATETIME()
END
--reset token
CREATE PROCEDURE SP_UpdateUser_ResetToken
    @UserId INT,
    @PasswordResetToken NVARCHAR(100) = NULL,
    @PasswordResetExpiry DATETIME2 = NULL
AS
BEGIN
    UPDATE users
    SET 
        password_reset_token = @PasswordResetToken,
        password_reset_expiry = @PasswordResetExpiry
    WHERE id = @UserId;
END
CREATE PROCEDURE SP_GetUserByEmail
    @Email NVARCHAR(255)
AS
BEGIN
    SELECT TOP 1 *
    FROM dbo.users
    WHERE email = @Email
END













-- 1. USERS
INSERT INTO dbo.users (username, password_hash, full_name, role, email)
VALUES 
('admin1','1','Admin One','admin','admin1@example.com'),
('farmer1','hashedpwd2','Farmer One','farmer','farmer1@example.com'),
('distributor1','hashedpwd3','Distributor One','distributor','dist1@example.com'),
('retailer1','hashedpwd4','Retailer One','retailer','retailer1@example.com'),
('inspector1','hashedpwd5','Inspector One','admin','inspector1@example.com');
GO

-- 2. FARMS
INSERT INTO dbo.farms (name, owner_name, location, contact_info, certifications)
VALUES 
('Sunny Farm','Nguyen Van A','Hanoi, Vietnam','0123456789','Organic'),
('Green Farm','Tran Thi B','Hue, Vietnam','0987654321','GAP');
GO

-- 3. PRODUCTS
INSERT INTO dbo.products (name, sku, category, storage_instructions, typical_shelf_life_days)
VALUES 
('Tomato','TOM123','Vegetable','Keep cool, avoid sun',7),
('Lettuce','LET456','Vegetable','Refrigerate',5);
GO

-- 4. BATCHES
INSERT INTO dbo.batches (batch_code, product_id, farm_id, created_by_user_id, harvest_date, quantity, unit, expiry_date, status)
VALUES 
('BATCH-TOM-001',1,1,2,'2025-11-15',100,'kg','2025-11-25','pending'),
('BATCH-LET-001',2,2,2,'2025-11-16',50,'kg','2025-11-21','pending');
GO

-- 5. WAREHOUSES
INSERT INTO dbo.warehouses (name, location, contact_info)
VALUES ('Central Warehouse','Hanoi, Vietnam','warehouse@example.com');
GO

-- 6. DISTRIBUTORS
INSERT INTO dbo.distributors (name, contact_info)
VALUES ('Fast Distributor','dist@example.com');
GO

-- 7. RETAILERS
INSERT INTO dbo.retailers (name, location, contact_info)
VALUES ('Fresh Retail','Hanoi, Vietnam','retail@example.com');
GO

-- 8. SHIPMENTS (Farm -> Warehouse)
INSERT INTO dbo.shipments (shipment_code, planned_by_user_id, from_type, from_id, to_type, to_id, status)
VALUES ('SHIP-FARM-WH-001',2,'farm',1,'warehouse',1,'arrived');
GO

-- 9. SHIPMENT_ITEMS
INSERT INTO dbo.shipment_items (shipment_id, batch_id, quantity, unit)
VALUES 
(1,1,100,'kg'),
(1,2,50,'kg');
GO

-- 10. WAREHOUSE_STOCK
INSERT INTO dbo.warehouse_stock (warehouse_id, batch_id, quantity, unit)
VALUES
(1,1,100,'kg'),
(1,2,50,'kg');
GO

-- 11. SHIPMENTS (Warehouse -> Retailer)
INSERT INTO dbo.shipments (shipment_code, planned_by_user_id, from_type, from_id, to_type, to_id, status)
VALUES ('SHIP-WH-RET-001',3,'warehouse',1,'retailer',1,'arrived');
GO

-- 12. SHIPMENT_ITEMS
INSERT INTO dbo.shipment_items (shipment_id, batch_id, quantity, unit)
VALUES 
(2,1,80,'kg'),
(2,2,40,'kg');
GO

-- 13. RETAILER_STOCK
INSERT INTO dbo.retailer_stock (retailer_id, batch_id, quantity, unit)
VALUES 
(1,1,80,'kg'),
(1,2,40,'kg');
GO

-- 14. DIGITAL_SIGNATURES
INSERT INTO dbo.digital_signatures (signer_user_id, signature_method, signature_value, reference_document)
VALUES
(5,'RSA','sigvalue1','Inspection Report BATCH-TOM-001');
GO

-- 15. INSPECTIONS
INSERT INTO dbo.inspections (batch_id, inspector_user_id, humidity, temperature, quality_score, signature_id, notes)
VALUES
(1,5,85,22,90,1,'Batch quality OK'),
(2,5,80,20,88,1,'Batch quality OK');
GO

-- 16. QR_CODES
INSERT INTO dbo.qr_codes (batch_id, token, url)
VALUES 
(9,'QR-TOM-001','https://example.com/qr/BATCH-TOM-001'),
(10,'QR-LET-001','https://example.com/qr/BATCH-LET-001');
GO

