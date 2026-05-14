CREATE TABLE Contractor (
    ContractorID  INT  PRIMARY KEY IDENTITY(1,1),
    CompanyName   VARCHAR(200)  NOT NULL,
    ContactInfo   VARCHAR(200)  NOT NULL,
    CreditLimit   DECIMAL(15,2) NOT NULL
                  CHECK (CreditLimit >= 0)
);
GO