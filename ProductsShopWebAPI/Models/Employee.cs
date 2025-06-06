namespace ProductsShopWebAPI.Models
{
    //public class Employee
    //{
    //    public int Id { get; set; }
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string Department { get; set; } = string.Empty;
    //    public string Position { get; set; } = string.Empty;
    //    public decimal Salary { get; set; }
    //    public DateTime HireDate { get; set; }
    //    public bool IsActive { get; set; } = true;
    //}

    //public class CreateEmployeeRequest
    //{
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string Department { get; set; } = string.Empty;
    //    public string Position { get; set; } = string.Empty;
    //    public decimal Salary { get; set; }
    //}

    //public class UpdateEmployeeRequest
    //{
    //    public string FirstName { get; set; } = string.Empty;
    //    public string LastName { get; set; } = string.Empty;
    //    public string Email { get; set; } = string.Empty;
    //    public string Department { get; set; } = string.Empty;
    //    public string Position { get; set; } = string.Empty;
    //    public decimal Salary { get; set; }
    //    public bool IsActive { get; set; }
    //}


    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Surname { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Department { get; set; }
        public int ExperienceYear { get; set; }
        public int JobTitle { get; set; }
        public DateTime YearOfEmployment { get; set; }
    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Surname { get; set; }
        public string FullName => !string.IsNullOrEmpty(Surname)
            ? $"{LastName} {Name} {Surname}"
            : $"{LastName} {Name}";
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int Age => DateTime.Now.Year - DateOfBirth.Year -
            (DateTime.Now.DayOfYear < DateOfBirth.DayOfYear ? 1 : 0);
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public int ExperienceYear { get; set; }
        public string JobTitleName { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int WorkingTime { get; set; }
        public DateTime YearOfEmployment { get; set; }
    }

  
    public class CreateEmployeeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Surname { get; set; }
        public int Gender { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Department { get; set; }
        public int ExperienceYear { get; set; }
        public int JobTitle { get; set; } 
    }

    public class UpdateEmployeeRequest
    {
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Surname { get; set; }
        public int Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int Department { get; set; }
        public int ExperienceYear { get; set; }
        public int JobTitle { get; set; }
        public DateTime YearOfEmployment { get; set; }
    }

  
    public class Gender
    {
        public int Id { get; set; }
        public string GenderName { get; set; } = string.Empty;
    }

    public class Department
    {
        public int ID { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int LocationRow { get; set; }
        public bool Sales { get; set; }
        public int Counters { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class Position
    {
        public int Id { get; set; }
        public string JobTitleName { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public int WorkingTime { get; set; }
    }

    public class Country
    {
        public int ID { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }
}