-- Заменяем вашу процедуру на исправленную версию
ALTER PROCEDURE GetEmployeesByPositionAndExperience
    @JobTitleId INT,
    @MinExperience INT = 0, 
    @MaxExperience INT = 100 
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        e.Id,
        e.Name,
        e.LastName,
        e.Surname,
        g.Gender,
        e.DateOfBirth,
        e.PhoneNumber,
        e.Address,
        e.City,
        d.Department as DepartmentName,   
        e.ExperienceYear,
        p.JobTitleName,
        p.Salary,
        p.WorkingTime,                   
        e.YearOfEmployment
    FROM Employees e
    INNER JOIN Departments d ON e.Department = d.ID
    INNER JOIN Positions p ON e.JobTitle = p.Id
    INNER JOIN Genders g ON e.Gender = g.Id
    WHERE e.JobTitle = @JobTitleId
      AND e.ExperienceYear BETWEEN @MinExperience AND @MaxExperience
    ORDER BY e.ExperienceYear DESC, e.LastName, e.Name;
END;