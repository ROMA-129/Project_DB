CREATE TABLE Technician (
    TechnicianID INT PRIMARY KEY IDENTITY(1,1),
    TechnicianName VARCHAR(100) NOT NULL,
    Specialization VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20),
    HireDate DATE,
    Salary DECIMAL(10,2)
);