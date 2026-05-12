-- =============================================================
-- Industrial Equipment Rental & Service Yard
-- Project 19 - IS211 Database Setup Script
-- =============================================================

USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'EquipmentRentalDB')
    DROP DATABASE EquipmentRentalDB;
GO

CREATE DATABASE EquipmentRentalDB;
GO

USE EquipmentRentalDB;
GO

-- =============================================================
-- TABLE CREATION
-- =============================================================

CREATE TABLE ServiceYard (
    YardID      INT IDENTITY(1,1) PRIMARY KEY,
    Location    NVARCHAR(200) NOT NULL,
    Capacity    INT NOT NULL,
    ContactNo   NVARCHAR(50)
);

CREATE TABLE Equipment (
    EquipmentID  INT IDENTITY(1,1) PRIMARY KEY,
    Model        NVARCHAR(150) NOT NULL,
    EnginePower  NVARCHAR(50),
    HourlyRate   DECIMAL(10,2) NOT NULL,
    Location     NVARCHAR(200),
    Status       NVARCHAR(50) NOT NULL DEFAULT 'Available'
                     CHECK (Status IN ('Available','Rented','Maintenance')),
    YardID       INT REFERENCES ServiceYard(YardID)
);

CREATE TABLE Contractor (
    ContractorID  INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName   NVARCHAR(150) NOT NULL,
    ContactInfo   NVARCHAR(200),
    CreditLimit   DECIMAL(12,2) NOT NULL DEFAULT 0
);

CREATE TABLE Technician (
    TechnicianID  INT IDENTITY(1,1) PRIMARY KEY,
    FullName      NVARCHAR(150) NOT NULL,
    ContactInfo   NVARCHAR(200),
    Specialty     NVARCHAR(100)
);

CREATE TABLE RentalAgreement (
    AgreementID   INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentID   INT NOT NULL REFERENCES Equipment(EquipmentID),
    ContractorID  INT NOT NULL REFERENCES Contractor(ContractorID),
    StartDate     DATE NOT NULL,
    EndDate       DATE,
    ReturnStatus  NVARCHAR(50) DEFAULT 'Active'
                      CHECK (ReturnStatus IN ('Active','Returned','Cancelled')),
    TotalHours    AS (DATEDIFF(HOUR, StartDate, ISNULL(EndDate, GETDATE())))
);

CREATE TABLE SafetyInspection (
    InspectionID   INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentID    INT NOT NULL REFERENCES Equipment(EquipmentID),
    TechnicianID   INT NOT NULL REFERENCES Technician(TechnicianID),
    InspectionDate DATE NOT NULL DEFAULT GETDATE(),
    Result         NVARCHAR(50) NOT NULL DEFAULT 'Pass'
                       CHECK (Result IN ('Pass','Fail','Pending')),
    Notes          NVARCHAR(500)
);

-- =============================================================
-- SAMPLE DATA
-- =============================================================

INSERT INTO ServiceYard (Location, Capacity, ContactNo) VALUES
('Cairo - Nasr City Industrial Zone',  50, '02-24001111'),
('Giza - 6th of October City',         35, '02-38002222'),
('Alexandria - Amreya District',        40, '03-44003333'),
('Suez - Port Area',                   25, '066-3234444'),
('Mansoura - Industrial District',     30, '050-2235555');

INSERT INTO Contractor (CompanyName, ContactInfo, CreditLimit) VALUES
('Delta Construction Co.',    'delta@construction.com | 01001234567',  500000),
('NileBuild Ltd.',            'info@nilebuild.com | 01112345678',       350000),
('SinaCon Industries',        'sinacon@mail.com | 01223456789',         420000),
('Horizon Engineering',       'horizon@eng.com | 01334567890',          280000),
('Al-Masry Heavy Works',      'almasry@works.com | 01445678901',        600000),
('RoxxGroup',                 'roxx@group.com | 01556789012',           150000);

INSERT INTO Technician (FullName, ContactInfo, Specialty) VALUES
('Ahmed Samir',    '01001111111', 'Hydraulic Systems'),
('Mohamed Hassan', '01002222222', 'Engine Maintenance'),
('Sara Khalil',    '01003333333', 'Electrical Systems'),
('Omar Fathy',     '01004444444', 'General Inspection'),
('Nour El-Din',    '01005555555', 'Heavy Equipment');

INSERT INTO Equipment (Model, EnginePower, HourlyRate, Location, Status, YardID) VALUES
('Caterpillar 336 Excavator',   '374 HP',  850.00, 'Yard A-1', 'Available',   1),
('Komatsu PC290 Excavator',     '204 HP',  720.00, 'Yard A-2', 'Rented',      1),
('Volvo EC380 Excavator',       '281 HP',  780.00, 'Yard B-1', 'Available',   2),
('John Deere 872 Grader',       '275 HP',  650.00, 'Yard B-2', 'Available',   2),
('CAT D6T Bulldozer',           '215 HP',  900.00, 'Yard C-1', 'Maintenance', 3),
('Liebherr LTM 1100 Crane',     '505 HP', 1500.00, 'Yard C-2', 'Available',   3),
('Grove GMK5150L Crane',        '600 HP', 1650.00, 'Yard D-1', 'Rented',      4),
('Terex Challenger Crane',      '420 HP', 1200.00, 'Yard D-2', 'Available',   4),
('CAT 336 Next Gen Excavator',  '395 HP',  950.00, 'Yard E-1', 'Available',   5),
('Hitachi ZX350-6 Excavator',   '259 HP',  710.00, 'Yard E-2', 'Available',   5),
('Bomag BW 213 Roller',         '145 HP',  400.00, 'Yard A-3', 'Available',   1),
('Dynapac CA3500 Roller',       '137 HP',  380.00, 'Yard B-3', 'Rented',      2);

INSERT INTO RentalAgreement (EquipmentID, ContractorID, StartDate, EndDate, ReturnStatus) VALUES
(2,  1, DATEADD(DAY,-30, GETDATE()), DATEADD(DAY,-20, GETDATE()), 'Returned'),
(7,  2, DATEADD(DAY,-25, GETDATE()), NULL,                         'Active'),
(2,  3, DATEADD(DAY,-45, GETDATE()), DATEADD(DAY,-35, GETDATE()), 'Returned'),
(12, 4, DATEADD(DAY,-15, GETDATE()), NULL,                         'Active'),
(2,  5, DATEADD(DAY,-10, GETDATE()), NULL,                         'Active'),
(1,  1, DATEADD(DAY,-60, GETDATE()), DATEADD(DAY,-50, GETDATE()), 'Returned'),
(4,  2, DATEADD(DAY,-28, GETDATE()), DATEADD(DAY,-18, GETDATE()), 'Returned'),
(6,  6, DATEADD(DAY,-20, GETDATE()), DATEADD(DAY,-10, GETDATE()), 'Returned'),
(8,  3, DATEADD(DAY,-8,  GETDATE()), NULL,                         'Active'),
(9,  4, DATEADD(DAY,-5,  GETDATE()), NULL,                         'Active');

INSERT INTO SafetyInspection (EquipmentID, TechnicianID, InspectionDate, Result, Notes) VALUES
(1,  1, DATEADD(DAY,-29, GETDATE()), 'Pass',    'All systems operational'),
(2,  2, DATEADD(DAY,-25, GETDATE()), 'Pass',    'Minor hydraulic seal replaced'),
(3,  1, DATEADD(DAY,-20, GETDATE()), 'Pass',    'Routine check completed'),
(5,  3, DATEADD(DAY,-18, GETDATE()), 'Fail',    'Engine fault - sent for repair'),
(4,  4, DATEADD(DAY,-15, GETDATE()), 'Pass',    'Pre-rental inspection'),
(6,  5, DATEADD(DAY,-12, GETDATE()), 'Pass',    'Load test completed'),
(7,  1, DATEADD(DAY,-10, GETDATE()), 'Pass',    'Boom inspection OK'),
(8,  2, DATEADD(DAY,-8,  GETDATE()), 'Pending', 'Awaiting electrical check'),
(9,  1, DATEADD(DAY,-5,  GETDATE()), 'Pass',    'All clear'),
(10, 4, DATEADD(DAY,-3,  GETDATE()), 'Pass',    'Track tension adjusted'),
(2,  1, DATEADD(DAY,-2,  GETDATE()), 'Pass',    'Post-rental inspection'),
(1,  2, DATEADD(DAY,-1,  GETDATE()), 'Pass',    'Regular maintenance check');

PRINT 'Database setup completed successfully!';
GO
