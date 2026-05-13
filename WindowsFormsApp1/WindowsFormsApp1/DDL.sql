CREATE TABLE ServiceYard (
    YardID INT PRIMARY KEY IDENTITY(1,1),
    YardName VARCHAR(100) NOT NULL,
    Location VARCHAR(150),
    Capacity INT
);

CREATE TABLE RentalAgreement (
    AgreementID INT PRIMARY KEY IDENTITY(1,1),
    ContractorID INT,
    EquipmentID INT,
    StartDate DATE,
    EndDate DATE,
    TotalCost DECIMAL(10, 2),
    CONSTRAINT FK_Contractor FOREIGN KEY (ContractorID) REFERENCES Contractor(ContractorID),
    CONSTRAINT FK_Equipment FOREIGN KEY (EquipmentID) REFERENCES Equipment(EquipmentID)
);

INSERT INTO ServiceYard (YardName, Location, Capacity) VALUES 
('Main East Yard', 'Cairo', 50),
('Giza Hub', 'Giza', 30),
('Alex Depot', 'Alexandria', 20),
('Suez Branch', 'Suez', 15),
('Delta Storage', 'Tanta', 25);

INSERT INTO RentalAgreement (ContractorID, EquipmentID, StartDate, EndDate, TotalCost) VALUES 
(1, 1, '2026-05-01', '2026-05-05', 5000),
(2, 3, '2026-05-02', '2026-05-10', 12000),
(1, 5, '2026-05-04', '2026-05-06', 3000),
(3, 2, '2026-05-10', '2026-05-15', 7500),
(4, 4, '2026-05-12', '2026-05-20', 15000);