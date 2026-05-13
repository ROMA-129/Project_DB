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