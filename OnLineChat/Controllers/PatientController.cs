﻿using System;
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
                patientModel = onlineChatLogic.getVisitInfoByVisitID(visit_id);
            }
            else
            {
                patientModel = new PatientVisitModel();
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