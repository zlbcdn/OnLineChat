using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace OnLineChat.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Doctor
        public ActionResult Index()
        {
            return View();
        }

        //根据医生ID、科室ID、就诊时间、诊别（上午诊、下午诊、夜诊），获取患者列表
        public string getRegisterPatientList(string doctor_id,string dept_id,string visit_date,string register_type)
        {
            List<PatientModel> registerList = new List<PatientModel>();
            PatientModel p1 = new PatientModel() { VisitID = "001", PatientID = "10002018", PatientName = "张三", Sex = "M" };
            PatientModel p2 = new PatientModel() { VisitID = "002", PatientID = "10002019", PatientName = "李四", Sex = "M" };
            PatientModel p3 = new PatientModel() { VisitID = "003", PatientID = "10002020", PatientName = "王五", Sex = "F" };
            PatientModel p4 = new PatientModel() { VisitID = "004", PatientID = "10002021", PatientName = "赵六", Sex = "M" };
            PatientModel p5 = new PatientModel() { VisitID = "005", PatientID = "10002022", PatientName = "赵钱", Sex = "F" };

            registerList.Add(p1);
            registerList.Add(p2);
            registerList.Add(p3);
            registerList.Add(p4);
            registerList.Add(p5);

            return JsonConvert.SerializeObject(registerList);
        }

    }

    public class PatientModel
    {
        public string VisitID { get; set; }
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string Sex { get; set; }
    }

}