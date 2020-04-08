using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnLineChatDomain;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

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
            //string token_check = onlychecktoken(token);
            string token_check = "true";
            bool token_result = false;
            bool convert_flag= Boolean.TryParse(token_check, out token_result);

            if(convert_flag)
            {
                if(token_result)
                {
                    patientModel = onlineChatLogic.getVisitInfoByVisitID(visit_id);
                }
                else
                {
                    patientModel = new PatientVisitModel() { MessageCode = "Error", MessageContent = "验证未通过！"};
                }
            }
            else
            {
                patientModel = new PatientVisitModel() { MessageCode = "Error", MessageContent = "将string类型的token转为bool时，未能转换成功！,传入值："+ token+ ";方法返回值："+token_result};
            }



            return View(patientModel);
        }


        public string onlychecktoken(string token)
        {

            System.Net.HttpWebRequest request = null;

            string strURL = "http://10.37.24.17/BtchOBSReportQuery/token/token" + "/onlychecktoken";

            request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
            //Post请求方式
            request.Method = "POST";
            request.Headers.Add("Authorization", "Bearer " + token);
            // 内容类型
            request.ContentType = "application/x-www-form-urlencoded";
            // 参数经过URL编码

            string parama = "token=" + token.Replace("+", "%2B");

            string strValue = "";

            byte[] payload;

            payload = System.Text.Encoding.UTF8.GetBytes(parama);
            //设置请求的 ContentLength 
            request.ContentLength = payload.Length;
            //获得请 求流
            try
            {
                Stream writer = request.GetRequestStream();
                //  将请求参数写入流
                writer.Write(payload, 0, payload.Length);
                // 关闭请求流
                writer.Close();
                // System.Net.HttpWebResponse response;
                // 获得响应流
                System.Net.HttpWebResponse response;
                // 获得响应流     
                response = (System.Net.HttpWebResponse)request.GetResponse();


                System.IO.Stream s;
                s = response.GetResponseStream();
                string StrDate = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);

                while ((StrDate = Reader.ReadLine()) != null)
                {

                    strValue = StrDate;
                }

                return strValue;
            }

            catch (Exception)
            {
                return "false";
            }
        }

        public string getToken(string patient_id,string visit_date,string visit_dept,string doctor_id)
        {
            return "12312";
        }


        public string getAllMessage(string from_id,string to_id)
        {
            OnLineChatLogic chatLogic = new OnLineChatLogic();

            List<messagemodel> messageList = chatLogic.getHistoryMessage(from_id, to_id);

            return JsonConvert.SerializeObject(messageList);
        }
    }
}