using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace AJAX_CRUD.Models
{
    public class EmployeeDB
    {
        //declare connection string  
        string cs = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

        //Return list of all Employees  
        public List<Employee> ListAll()
        {
            List<Employee> lst = new List<Employee>();

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("sp_SelectEmployee", con);
                    com.CommandType = CommandType.StoredProcedure;
                    SqlDataReader rdr = com.ExecuteReader();

                    while (rdr.Read())
                    {
                        lst.Add(new Employee
                        {
                            EmployeeID = Convert.ToInt32(rdr["EmployeeId"]),
                            FirstName = rdr["FirstName"].ToString(),
                            LastName = rdr["LastName"].ToString(),
                            Age = Convert.ToInt32(rdr["Age"]),
                            State = rdr["State"].ToString(),
                            Country = rdr["Country"].ToString(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return lst;
        }


        //Method for Adding an Employee  
        public int Add(Employee emp)
        {
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("sp_InsertUpdateEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", emp.EmployeeID);
                com.Parameters.AddWithValue("@Name", emp.FirstName);
                com.Parameters.AddWithValue("@Name", emp.LastName);
                com.Parameters.AddWithValue("@Age", emp.Age);
                com.Parameters.AddWithValue("@State", emp.State);
                com.Parameters.AddWithValue("@Country", emp.Country);
                com.Parameters.AddWithValue("@Action", "Insert");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        // ...

        //Method to retrieve a single employee by ID
        public Employee GetEmployee(int employeeID)
        {
            Employee employee = null;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand("sp_GetEmployee", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Id", employeeID);
                    SqlDataReader rdr = com.ExecuteReader();

                    if (rdr.Read())
                    {
                        employee = new Employee
                        {
                            EmployeeID = Convert.ToInt32(rdr["EmployeeId"]),
                            FirstName = rdr["Name"].ToString(),
                            Age = Convert.ToInt32(rdr["Age"]),
                            State = rdr["State"].ToString(),
                            Country = rdr["Country"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return employee;
        }

        // ...


        //Method for Updating Employee record  
        public int Update(Employee emp)
        {
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("sp_InsertUpdateEmployee", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", emp.EmployeeID);
                com.Parameters.AddWithValue("@Name", emp.FirstName);
                com.Parameters.AddWithValue("@Age", emp.Age);
                com.Parameters.AddWithValue("@State", emp.State);
                com.Parameters.AddWithValue("@Country", emp.Country);
                com.Parameters.AddWithValue("@Action", "Update");
                i = com.ExecuteNonQuery();
            }
            return i;
        }

        //Method for Deleting an Employee  
        public int Delete(int ID)
        {
            int i;
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                SqlCommand com = new SqlCommand("sp_DeleteEmployee", con);  
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@Id", ID);
                i = com.ExecuteNonQuery();
            }
            return i;
        }
    }
}