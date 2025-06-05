using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace ProductsShop.Pages.Employees
{
    public class IndexModel : PageModel
    {
        public List<EmployeeInfo> EmployeesList = new List<EmployeeInfo>();
        public void OnGet()
        {
            try
            {
                //String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
                String connectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Shop;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT * FROM dbo.Employees";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read()) 
                            {
                                //EmployeeInfo employeeInfo = new EmployeeInfo();
                                //employeeInfo.id = reader.GetInt32(0).ToString();
                                //employeeInfo.name = reader.GetString(1);
                                //employeeInfo.lastname = reader.GetString(2);
                                //employeeInfo.surname = reader.IsDBNull(3) ? "" : reader.GetString(3); 
                                //employeeInfo.gender = reader.GetInt32(4).ToString();
                                //employeeInfo.dateofbirth = reader.GetDateTime(5).ToString("yyyy-MM-dd"); 
                                //employeeInfo.phonenumber = reader.GetString(6);
                                //employeeInfo.adres = reader.GetString(7);
                                //employeeInfo.city = reader.GetString(8);
                                //employeeInfo.department = reader.GetInt32(9).ToString();
                                //employeeInfo.experienceyear = reader.GetInt32(10).ToString();
                                //employeeInfo.jobtytle = reader.GetInt32(11).ToString();
                                //employeeInfo.yearofemployment = reader.GetDateTime(12).ToString("yyyy-MM-dd");
                                //EmployeesList.Add(employeeInfo);
                                EmployeeInfo employeeInfo = new EmployeeInfo();
                                employeeInfo.id = reader["Id"].ToString();
                                employeeInfo.name = reader["Name"].ToString();
                                employeeInfo.lastname = reader["LastName"].ToString(); // Обратите внимание на LastName
                                employeeInfo.surname = reader.IsDBNull("Surname") ? "" : reader["Surname"].ToString();
                                employeeInfo.gender = reader["Gender"].ToString();
                                employeeInfo.dateofbirth = Convert.ToDateTime(reader["DateOfBirth"]).ToString("yyyy-MM-dd");
                                employeeInfo.phonenumber = reader["PhoneNumber"].ToString(); // PhoneNumber
                                employeeInfo.adres = reader["Address"].ToString(); // Address
                                employeeInfo.city = reader["City"].ToString();
                                employeeInfo.department = reader["Department"].ToString();
                                employeeInfo.experienceyear = reader["ExperienceYear"].ToString(); // ExperienceYear
                                employeeInfo.jobtytle = reader["JobTitle"].ToString(); // JobTitle
                                employeeInfo.yearofemployment = Convert.ToDateTime(reader["YearOfEmployment"]).ToString("yyyy-MM-dd");
                                EmployeesList.Add(employeeInfo);

                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class EmployeeInfo
    {
        public String id { get; set; }
        public String name { get; set; }
        public String lastname { get; set; }
        public String surname { get; set; }
        public String gender { get; set; }
        public String dateofbirth { get; set; }
        public String phonenumber { get; set; }
        public String adres { get; set; }
        public String city { get; set; }
        public String department { get; set; }
        public String experienceyear { get; set; }
        public String jobtytle { get; set; }
        public String yearofemployment { get; set; }

    }
}
