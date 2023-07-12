using AJAX_CRUD.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace AJAX_CRUD.Controllers
{
    public class EmployeeServerSideController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetEmployees(int draw, int start, int length, int sortCol, string sortDir, string search)
{
    List<Employee> employees = new List<Employee>();
    int totalCount = 0;

    try
    {
        using (SqlConnection connection = new SqlConnection(cs))
        {
            connection.Open();
            SqlCommand command = new SqlCommand("sp_GetEmployees", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@DisplayLength", length);
            command.Parameters.AddWithValue("@DisplayStart", start);
            command.Parameters.AddWithValue("@SortCol", sortCol);
            command.Parameters.AddWithValue("@SortDir", sortDir);
            command.Parameters.AddWithValue("@Search", search);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            DataTable dataTable = new DataTable();
            dataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                totalCount = Convert.ToInt32(dataTable.Rows[0]["TotalCount"]);

                employees = dataTable.AsEnumerable().Select(row => new Employee
                {
                    EmployeeID = Convert.ToInt32(row["EmployeeID"]),
                    FirstName = Convert.ToString(row["FirstName"]),
                    LastName = Convert.ToString(row["LastName"]),
                    // Age = Convert.ToInt32(row["Age"]),
                    Age = row["Age"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["Age"]),
                    State = Convert.ToString(row["State"]),
                    Country = Convert.ToString(row["Country"])
                }).ToList();
            }
        }

        return Json(new
        {
            draw = draw,
            recordsTotal = totalCount,
            recordsFiltered = totalCount,
            data = employees
        });
    }
    catch (Exception ex)
    {
        return Json(new
        {
            error = "An error occurred while retrieving employee records: " + ex.Message
        });
    }
}

        public JsonResult AddEmployee(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("sp_InsertUpdateEmployee", connection);
                command.CommandType = CommandType.StoredProcedure;
               command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName);
                if (employee.Age == null)
                {
                    command.Parameters.AddWithValue("@Age", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@Age", employee.Age);
                }
                command.Parameters.AddWithValue("@State", employee.State);
                command.Parameters.AddWithValue("@Country", employee.Country);
                command.Parameters.AddWithValue("@Action", "Insert");
                command.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }

        public JsonResult UpdateEmployee(Employee employee)
        {
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("sp_InsertUpdateEmployee", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                command.Parameters.AddWithValue("@LastName", employee.LastName); 
                if (employee.Age == null)
                {
                    command.Parameters.AddWithValue("@Age", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@Age", employee.Age);
                }

                command.Parameters.AddWithValue("@State", employee.State);
                command.Parameters.AddWithValue("@Country", employee.Country);
                command.Parameters.AddWithValue("@Action", "Update");
                command.ExecuteNonQuery();
            }

            return Json(new { success = true });
        }

        public JsonResult DeleteEmployee(int employeeID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand("sp_DeleteEmployee", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmployeeID", employeeID);
                        command.ExecuteNonQuery();
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
         public ActionResult Create()
        {
            return View();
        }

      [HttpPost]
public ActionResult Create(Employee employee)
{
    if (ModelState.IsValid)
    {
        AddEmployee(employee);
        return RedirectToAction("Index"); 
    }
    else
    {
        return Json(new { success = false, error = "Invalid employee data. Please check the input." });
    }
}

        public ActionResult Edit(int id)
        {
            Employee employee = GetEmployeeById(id);

            if (employee == null)
            {
                return View("Error");
            }

            return View(employee);
        }

        [HttpPost]
        public ActionResult Edit(Employee employee)
        {
            UpdateEmployee(employee);
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int employeeID)
        {
            Employee employee = GetEmployeeById(employeeID);

            if (employee == null)
            {
                return View("Error");
            }

            return View(employee);
        }

        [HttpPost]
        public ActionResult DeleteConfirmed(int employeeID)
        {
            DeleteEmployee(employeeID);

           return RedirectToAction("Index");
        }


        private Employee GetEmployeeById(int employeeID)
        {
            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("sp_SelectEmployee", connection);
                command.CommandType = CommandType.StoredProcedure;

                DataTable dataTable = new DataTable();
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(dataTable);
                }

                DataRow row = dataTable.AsEnumerable()
                    .FirstOrDefault(r => Convert.ToInt32(r["EmployeeID"]) == employeeID);

                if (row != null)
                {
                    Employee employee = new Employee()
                    {
                        EmployeeID = Convert.ToInt32(row["EmployeeID"]), 
                        FirstName = Convert.ToString(row["FirstName"]),
                        LastName = Convert.ToString(row["LastName"]),
                        Age = row["Age"] == DBNull.Value ? null : (int?)Convert.ToInt32(row["Age"]),
                        State = Convert.ToString(row["State"]),
                        Country = Convert.ToString(row["Country"])
                    };
                    return employee;
                }
            }
            return null; 
        }   
    }
}
