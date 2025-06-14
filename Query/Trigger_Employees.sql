


CREATE TRIGGER trg_Employees_Validation
ON Employees
INSTEAD OF INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE DATEADD(YEAR, 18, DateOfBirth) > GETDATE()
    )
    BEGIN
        RAISERROR('Employee must be at least 18 years old!', 16, 1);
        RETURN;
    END
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE ExperienceYear > DATEDIFF(YEAR, DateOfBirth, GETDATE()) - 18
    )
    BEGIN
        RAISERROR('Work experience cannot exceed employee age minus 18 years!', 16, 1);
        RETURN;
    END
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE PhoneNumber NOT LIKE '380%' OR LEN(PhoneNumber) <> 12 OR PhoneNumber NOT LIKE '380[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'
    )
    BEGIN
        RAISERROR('Phone number must start with 380 and be 12 digits long!', 16, 1);
        RETURN;
    END
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE City IS NULL OR LEN(TRIM(City)) < 2
    )
    BEGIN
        RAISERROR('City name must be specified and be at least 2 characters long!', 16, 1);
        RETURN;
    END
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM Positions p
            WHERE p.Id = i.JobTitle
        )
    )
    BEGIN
        RAISERROR('The specified job title does not exist!', 16, 1);
        RETURN;
    END
    
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM Departments d
            WHERE d.ID = i.Department
        )
    )
    BEGIN
        RAISERROR('The specified department does not exist!', 16, 1);
        RETURN;
    END
    
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE NOT EXISTS (
            SELECT 1 FROM Genders g
            WHERE g.Id = i.Gender
        )
    )
    BEGIN
        RAISERROR('The specified gender does not exist!', 16, 1);
        RETURN;
    END
    IF EXISTS (
        SELECT 1 FROM inserted i
        WHERE i.ExperienceYear = 0 
        AND YEAR(i.YearOfEmployment) < YEAR(GETDATE())
        AND DATEDIFF(MONTH, i.YearOfEmployment, GETDATE()) > 6
    )
    BEGIN
        RAISERROR('Employee cannot have zero experience if employed more than 6 months ago!', 16, 1);
        RETURN;
    END
    IF EXISTS (SELECT 1 FROM deleted) 
    BEGIN
        UPDATE e
        SET e.Name = i.Name,
            e.LastName = i.LastName,
            e.Surname = i.Surname,
            e.Gender = i.Gender,
            e.DateOfBirth = i.DateOfBirth,
            e.PhoneNumber = i.PhoneNumber,
            e.Address = i.Address,
            e.City = i.City,
            e.Department = i.Department,
            e.ExperienceYear = i.ExperienceYear,
            e.JobTitle = i.JobTitle,
            e.YearOfEmployment = i.YearOfEmployment
        FROM Employees e
        INNER JOIN inserted i ON e.Id = i.Id;
    END
    ELSE 
    BEGIN
        INSERT INTO Employees (Name, LastName, Surname, Gender, DateOfBirth, PhoneNumber, 
                              Address, City, Department, ExperienceYear, JobTitle, YearOfEmployment)
        SELECT Name, LastName, Surname, Gender, DateOfBirth, PhoneNumber, 
              Address, City, Department, ExperienceYear, JobTitle, YearOfEmployment
        FROM inserted;
    END
END;
GO


CREATE TRIGGER trg_Employees_EmploymentDate_Correction
ON Employees
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (
        SELECT 1 FROM inserted 
        WHERE CAST(YearOfEmployment AS DATE) > CAST(GETDATE() AS DATE)
    )
    BEGIN
        UPDATE e
        SET YearOfEmployment = CAST(GETDATE() AS DATE)
        FROM Employees e
        INNER JOIN inserted i ON e.Id = i.Id
        WHERE CAST(i.YearOfEmployment AS DATE) > CAST(GETDATE() AS DATE);
        PRINT 'Employment date corrected to today for employees with future employment dates.';
    END
END;
GO

