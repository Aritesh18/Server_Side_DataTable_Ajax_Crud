using AJAX_CRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AJAX_CRUD.Controllers
{
    public class HomeController : Controller
    {
            EmployeeDB empDB = new EmployeeDB();
        public ActionResult Index()
        {

            var employees = empDB.ListAll().Take(5).ToList();
            return View(employees);
            //EmployeeDB empDB = new EmployeeDB();
            //List<Employee> employees = empDB.ListAll();
            //return View(employees);
        }
        //public JsonResult List()
        //    {
        //        return Json(empDB.ListAll(), JsonRequestBehavior.AllowGet);
        //    }

        public JsonResult List(int pageNumber, int pageSize)
        {
            var data = empDB.ListAll();

            // Apply pagination
            var paginatedData = data
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(paginatedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Add(Employee emp)
            {
                return Json(empDB.Add(emp), JsonRequestBehavior.AllowGet);
            }
            public JsonResult GetbyID(int ID)
            {
                var Employee = empDB.ListAll().Find(x => x.EmployeeID.Equals(ID));
                return Json(Employee, JsonRequestBehavior.AllowGet);
            }
            public JsonResult Update(Employee emp)
            {
                return Json(empDB.Update(emp), JsonRequestBehavior.AllowGet);
            }
            public JsonResult Delete(int ID)
            {
                return Json(empDB.Delete(ID), JsonRequestBehavior.AllowGet);
            }
        }
    }
