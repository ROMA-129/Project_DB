CREATE TABLE SafetyInspection (
    InspectionID INT PRIMARY KEY IDENTITY(1,1),
    EquipmentID INT NOT NULL,
    TechnicianID INT NOT NULL,
    InspectionDate DATE NOT NULL,
    ConditionChecklist VARCHAR(255),
    MaintenanceWork VARCHAR(255),
    InspectionStatus VARCHAR(50),

    FOREIGN KEY (TechnicianID) REFERENCES Technician(TechnicianID)
);
