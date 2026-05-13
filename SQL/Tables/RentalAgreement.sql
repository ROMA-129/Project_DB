
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
