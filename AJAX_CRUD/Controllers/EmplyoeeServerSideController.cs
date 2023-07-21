using AJAX_CRUD.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AJAX_CRUD.Controllers
{
    public class EmployeeServerSideController : Controller
    {
        string cs = ConfigurationManager.ConnectionStrings["conStr"].ConnectionString;

        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<IdentityUser> _signInManager;
        
        public EmployeeServerSideController(IAuthenticationService authenticationService, IHttpContextAccessor httpContextAccessor, SignInManager<IdentityUser> signInManager)
        {
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
            _signInManager = signInManager;
        }
        public EmployeeServerSideController()
        {
            //parameterless constructor
        }
        //[Authorize]

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
        //-----------for export all records to excel-----------------------

        [HttpPost]
        //public ActionResult ExportToExcel()
        //{
        //    try
        //    {
        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //        using (SqlConnection connection = new SqlConnection(cs))
        //        {
        //            connection.Open();
        //            SqlCommand command = new SqlCommand("sp_GetEmployees", connection);
        //            command.CommandType = CommandType.StoredProcedure;
        //            command.Parameters.AddWithValue("@DisplayLength", int.MaxValue);
        //            command.Parameters.AddWithValue("@DisplayStart", 0);
        //            command.Parameters.AddWithValue("@SortCol", 0);
        //            command.Parameters.AddWithValue("@SortDir", "asc");
        //            command.Parameters.AddWithValue("@Search", "");

        //            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
        //            DataTable dataTable = new DataTable();
        //            dataAdapter.Fill(dataTable);





        //            using (ExcelPackage excelPackage = new ExcelPackage())
        //            {
        //                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Employees");
        //                worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
        //                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        //                byte[] fileContents = excelPackage.GetAsByteArray();
        //                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, error = "An error occurred while exporting employee data: " + ex.Message }, JsonRequestBehavior.AllowGet);
        //    }

        //}


        public ActionResult ExportToExcel(string employeeIDs)
        {
            try
            {
                string[] employeeIDArray = employeeIDs.Split(',');

                int[] selectedEmployeeIDs = Array.ConvertAll(employeeIDArray, int.Parse);

                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    string query = "SELECT EmployeeID, FirstName, Age, State, Country " +
                                   "FROM Employee " +
                                   "WHERE EmployeeID IN (" + string.Join(",", selectedEmployeeIDs) + ")";

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Employees");
                        worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                        byte[] fileContents = excelPackage.GetAsByteArray();
                        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occurred while exporting employee data: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ExportSelectedEmployeesToExcelFromDB(string employeeIDs)
        {
            try
            {
                string[] employeeIDArray = employeeIDs.Split(',');
                int[] selectedEmployeeIDs = Array.ConvertAll(employeeIDArray, int.Parse);

                using (SqlConnection connection = new SqlConnection(cs))
                {
                    connection.Open();

                    string parameterNames = string.Join(",", selectedEmployeeIDs.Select((_, index) => "@ID" + index));  
                    string query = "SELECT EmployeeID, FirstName, Age, State, Country " +
                                   "FROM Employee " +
                                   "WHERE EmployeeID IN (" + parameterNames + ")";

                    SqlCommand command = new SqlCommand(query, connection);

                    for (int i = 0; i < selectedEmployeeIDs.Length; i++)
                    {
                        command.Parameters.AddWithValue("@ID" + i, selectedEmployeeIDs[i]);
                    }

                    SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (ExcelPackage excelPackage = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Employees");
                        worksheet.Cells["A1"].LoadFromDataTable(dataTable, true);
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                        byte[] fileContents = excelPackage.GetAsByteArray();
                        return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "An error occurred while exporting employee data: " + ex.Message }, JsonRequestBehavior.AllowGet);
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
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
           public ActionResult Login(LoginUser user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(cs))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_LoginUser", connection))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            connection.Open();
                            cmd.Parameters.AddWithValue("@Password", user.Password);
                            cmd.Parameters.AddWithValue("@UserName", user.UserName);
                            cmd.Parameters.AddWithValue("@Action", "Login");
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, reader["UserName"].ToString()),
                                new Claim(ClaimTypes.Role, reader["Role"].ToString())
                            };
                                    var claimsIdentity = new ClaimsIdentity(
                                        claims, CookieAuthenticationDefaults.AuthenticationScheme);
                                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                                    _authenticationService.SignInAsync(_httpContextAccessor.HttpContext, CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, new AuthenticationProperties
                                    { IsPersistent = true });
                                    return RedirectToAction("Index", "EmployeeServerSide");
                                }
                                else
                                {
                                    ModelState.AddModelError("", "Invalid login attempt");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                   
                    ModelState.AddModelError("", "An error occurred while processing your login request." + ex.Message);
                    return View("Error");
                }
            }
            return View(user);
        }
        [HttpPost]
        public ActionResult Logout()
        {
            _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "EmployeeServerSide");
        }

    }
    }

