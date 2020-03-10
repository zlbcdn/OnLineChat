using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnLineChat.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult Index()
        {
            return View();
        }


        public string getToken(string patient_id,string visit_date,string visit_dept,string doctor_id)
        {
            return "12312";
        }
    }
}