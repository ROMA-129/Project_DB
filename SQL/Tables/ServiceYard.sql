CREATE TABLE ServiceYard (
    YardID INT PRIMARY KEY IDENTITY(1,1),
    YardName VARCHAR(100) NOT NULL,
    Location VARCHAR(150),
    Capacity INT
);