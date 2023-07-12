using System.Collections.Generic;
using System.Web.Mvc;
using AJAX_CRUD.Models;
namespace AJAX_CRUD.Controllers
{
    public class EmployeeController : Controller
    {
       

        // GET: Employee
        public ActionResult Index()
        {
            EmployeeDB empDB = new EmployeeDB();
            List<Employee> employees = empDB.ListAll();
            return View(employees);
        }

        // GET: Employee/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        public ActionResult Create(Employee emp)
        {
            try
            {
                EmployeeDB empDB = new EmployeeDB();
                empDB.Add(emp);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Edit/5
        public ActionResult Edit(int id)
        {
            EmployeeDB empDB = new EmployeeDB();
            Employee emp = empDB.GetEmployee(id);
            return View(emp);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Employee emp)
        {
            try
            {
                EmployeeDB empDB = new EmployeeDB();
                empDB.Update(emp);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Employee/Delete/5
        public ActionResult Delete(int id)
        {
            EmployeeDB empDB = new EmployeeDB();
            Employee emp = empDB.GetEmployee(id);
            return View(emp);
        }

        // POST: Employee/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                EmployeeDB empDB = new EmployeeDB();
                empDB.Delete(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
