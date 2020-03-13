using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using OnLineChatDomain;

namespace OnLineChat.Controllers
{
    public class DoctorController : Controller
    {
        // GET: Doctor
        public ActionResult Index()
        {
            string doctor_id = Request.QueryString["doctor_id"];
            string dept_id = Request.QueryString["dept_id"];
            string visit_date = Request.QueryString["visit_date"];
            string reg_type = Request.QueryString["reg_type"];

            ClinicDoctorModel doctorModel = new ClinicDoctorModel();

            doctorModel.DoctorID = doctor_id;
            doctorModel.DeptID = dept_id;
            doctorModel.VisitDate = visit_date;
            doctorModel.RegisterType = reg_type;

            return View(doctorModel);
        }

        //根据医生ID、科室ID、就诊时间、诊别（上午诊、下午诊、夜诊），获取患者列表
        public string getRegisterPatientList(string doctor_id,string dept_id,string visit_date,string register_type)
        {
            List<PatientModel> registerList = new List<PatientModel>();

            //OnLineChatLogic onlineChatLogic = new OnLineChatLogic();
            //registerList=onlineChatLogic.getPatientList(doctor_id, dept_id, visit_date, register_type);

            
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



}