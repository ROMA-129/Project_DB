CREATE TABLE Equipment (
    EquipmentID   INT           PRIMARY KEY IDENTITY(1,1),
    Model         VARCHAR(100)  NOT NULL,
    EnginePower   DECIMAL(10,2) NOT NULL,
    HourlyRate    DECIMAL(10,2) NOT NULL,
    [Status]      VARCHAR(20)   NOT NULL DEFAULT 'Available'
                  CHECK ([Status] IN ('Available', 'Rented', 'Under Maintenance')),
    YardID        INT           NULL,
    CONSTRAINT FK_Equipment_ServiceYard
        FOREIGN KEY (YardID) REFERENCES ServiceYard(YardID)
);
GO

CREATE TABLE Contractor (
    ContractorID  INT           PRIMARY KEY IDENTITY(1,1),
    CompanyName   VARCHAR(200)  NOT NULL,
    ContactInfo   VARCHAR(200)  NOT NULL,
    CreditLimit   DECIMAL(15,2) NOT NULL
                  CHECK (CreditLimit >= 0)
);
GO

INSERT INTO Equipment (Model, EnginePower, HourlyRate, [Status], YardID)
VALUES
    ('Caterpillar 320 Excavator',       320.00, 150.00, 'Available',         1),
    ('Liebherr LTM 1200 Crane',         750.00, 380.00, 'Rented',            1),
    ('Atlas Copco XAS 375 Generator',   120.00,  95.00, 'Available',         2),
    ('Komatsu D65 Bulldozer',           305.00, 175.00, 'Under Maintenance', 2),
    ('JLG 600S Aerial Work Platform',    84.00,  80.00, 'Available',         3),
    ('Volvo EC480 Excavator',           380.00, 165.00, 'Rented',            1),
    ('Manitowoc MLC300 Crawler Crane',  690.00, 420.00, 'Available',         3);
GO

INSERT INTO Contractor (CompanyName, ContactInfo, CreditLimit)
VALUES
    ('Apex Construction LLC',       'apex@apexconstruct.com',    75000.00),
    ('BuildRight Group',            '+1-212-555-0192',           50000.00),
    ('NorthStar Contractors Inc.',  'info@northstarcon.com',    120000.00),
    ('TerraForm Engineering',       '+1-312-555-0874',           40000.00),
    ('Ironclad Industrial Corp.',   'ops@ironclad.com',         200000.00),
    ('Skyline Builders Co.',        'contact@skylinebuild.com',  60000.00),
    ('Pacific Works Ltd.',          '+1-415-555-0331',           85000.00);
GO
