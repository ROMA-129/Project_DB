
-- DDL FOR TECHNICIAN TABLE
CREATE TABLE Technician (
    TechnicianID INT PRIMARY KEY IDENTITY(1,1),
    TechnicianName VARCHAR(100) NOT NULL,
    Specialization VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20),
    HireDate DATE,
    Salary DECIMAL(10,2)
);

-- DDL FOR SAFETY INSPECTION TABLE
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

-- POPULATED DATA (5-10 INSERTS)

INSERT INTO Technician 
(TechnicianName, Specialization, PhoneNumber, HireDate, Salary)
VALUES
('Ahmed Ali', 'Mechanical', '01011111111', '2023-01-10', 12000),
('Omar Hassan', 'Electrical', '01022222222', '2022-05-15', 13500),
('Youssef Emad', 'Hydraulic', '01033333333', '2021-08-20', 14000),
('Karim Adel', 'Engine Systems', '01044444444', '2024-02-01', 11000),
('Mostafa Samir', 'Heavy Equipment', '01055555555', '2023-06-11', 12500);

INSERT INTO SafetyInspection
(EquipmentID, TechnicianID, InspectionDate, ConditionChecklist, MaintenanceWork, InspectionStatus)
VALUES
(1, 1, '2025-05-01', 'Good Condition', 'Oil Changed', 'Approved'),
(2, 2, '2025-05-02', 'Minor Damage', 'Brake Repair', 'Pending'),
(3, 3, '2025-05-03', 'Excellent', 'No Maintenance Needed', 'Approved'),
(4, 4, '2025-05-04', 'Hydraulic Leak', 'Hydraulic Pipe Replaced', 'Approved'),
(5, 5, '2025-05-05', 'Average Condition', 'Filter Replaced', 'Approved'),
(2, 1, '2025-05-06', 'Engine Checked', 'Engine Tuning', 'Approved'),
(3, 2, '2025-05-07', 'Electrical Issue', 'Wire Replacement', 'Pending');
