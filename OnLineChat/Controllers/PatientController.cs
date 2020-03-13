using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnLineChatDomain;

namespace OnLineChat.Controllers
{
    public class PatientController : Controller
    {
        // GET: Patient
        public ActionResult Index()
        {
            OnLineChatLogic onlineChatLogic = new OnLineChatLogic();

            string token= Request.QueryString["token"];
            string visit_id = Request.QueryString["visit_id"];

            PatientVisitModel patientModel = null;
            //TO-DO
            if (checkToken(token))
            {
                //patientModel = onlineChatLogic.getVisitInfoByVisitID(visit_id);

                patientModel = new PatientVisitModel() { MessageCode="OK", PatientID = "10002018", PatientName = "测试张三",
                    DoctorID = "A00239", DoctorName = "郑林", DeptID = "12700",DeptName="测试科室"};
            }
            else
            {
                patientModel = new PatientVisitModel() { MessageCode="Error",MessageContent="弹出具体的异常原因"};
            }

            return View(patientModel);
        }

        //TO-DO，需要吉卫洁提供判断方法
        private bool checkToken(string v_token)
        {
            return true;
        }

        public string getToken(string patient_id,string visit_date,string visit_dept,string doctor_id)
        {
            return "12312";
        }
    }
}