using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using System.IO;

namespace OnLineChatDomain
{
    public class OnLineChatLogic
    {
        //根据visit_id,获取患者就诊的基本信息
        public PatientVisitModel getVisitInfoByVisitID(string visit_id)
        {
            PatientVisitModel resultModel =null;
            OracleConnection conn = DBContext.GetZLOracleConnection();
            OracleCommand command = new OracleCommand();
            string sqlStr = @" select t.PATIENT_ID, t.PATIENT_NAME, t.DEPT_ID, t.DEPT_NAME,t.DOCTOR_ID,t.DOCTOR_NAME
                                          from reg_visit_master t
                                         where t.visit_id = :v_visit_id ";


            try
            {
                command.Connection = conn;
                command.CommandText = sqlStr;
                command.Parameters.Add("v_visit_id", OracleDbType.Varchar2).Value = visit_id;

                using(OracleDataReader reader=command.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        resultModel = new PatientVisitModel();
                        while (reader.Read())
                        {
                            resultModel.PatientID = DBContext.getOracleStringItem(reader, "PATIENT_ID");
                            resultModel.PatientName = DBContext.getOracleStringItem(reader, "PATIENT_NAME");
                            resultModel.DeptID = DBContext.getOracleStringItem(reader, "DEPT_ID");
                            resultModel.DeptName = DBContext.getOracleStringItem(reader, "DEPT_NAME");
                            resultModel.DoctorID= DBContext.getOracleStringItem(reader, "DOCTOR_ID");
                            resultModel.DoctorName = DBContext.getOracleStringItem(reader, "DOCTOR_NAME");
                        }
                    }
                }


                conn.Close();

                return resultModel;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }


        //根据医生登录人信息，获取患者挂号列表
        public List<PatientModel> getPatientList(string doctor_id, string dept_id, string visit_date, string register_type)
        {
            List<PatientModel> resultList = new List<PatientModel>();
            OracleConnection conn = DBContext.GetZLOracleConnection();
            OracleCommand command = new OracleCommand();
            string sqlStr = @" select t.visit_id, t.patient_id, t.patient_name, decode(t.sex,'男','M','F') sex
                                              from reg_visit_master t
                                             where t.doctor_id = :v_doctor_id
                                               and t.dept_id = :v_dept_id
                                               and t.visit_date = :v_visit_date
                                               and t.reg_time_name = :v_reg_time_name
                                                and t.VISIT_STATUS!='DEL'
                                                order by t.SEQUENCE_NO asc ";


            try
            {
                //将string类型的日期转为date类型
                int year = Convert.ToInt32(visit_date.Substring(0, 4));
                int month = Convert.ToInt32(visit_date.Substring(4, 2));
                int day = Convert.ToInt32(visit_date.Substring(6, 2));

                DateTime v_visit_date = new DateTime(year, month, day);

                command.Connection = conn;
                command.CommandText = sqlStr;
                command.Parameters.Add("v_doctor_id", OracleDbType.Varchar2).Value = doctor_id;
                command.Parameters.Add("v_dept_id", OracleDbType.Varchar2).Value = dept_id;
                command.Parameters.Add("v_visit_date", OracleDbType.Date).Value = v_visit_date;
                command.Parameters.Add("v_reg_time_name", OracleDbType.Varchar2).Value = register_type;

                using (OracleDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        PatientModel model = null;
                        while (reader.Read())
                        {
                            model = new PatientModel();

                            model.VisitID= DBContext.getOracleStringItem(reader, "visit_id");
                            model.PatientID = DBContext.getOracleStringItem(reader, "patient_id");
                            model.PatientName = DBContext.getOracleStringItem(reader, "patient_name");
                            model.Sex = DBContext.getOracleStringItem(reader, "sex");

                            resultList.Add(model);
                        }
                    }
                }


                conn.Close();

                return resultList;

            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }
    
    
        public List<messagemodel> getHistoryMessage(string message_from_id,string message_to_id)
        {
            List<messagemodel> resultList = new List<messagemodel>();
            OracleConnection conn = DBContext.GetDIMSOracleConnection();
            OracleCommand command = new OracleCommand();
            
            try
            {
                command.Connection = conn;

                resultList.AddRange(getHistoryMessage(message_from_id, message_to_id, command));
                resultList.AddRange(getHistoryMessage(message_to_id, message_from_id, command));

                var tempList = (from r in resultList orderby r.message_date ascending select r).ToList<messagemodel>();

                conn.Close();

                return tempList;
            }
            catch (Exception)
            {

                return resultList;
            }
            finally
            {
                if (conn != null) conn.Close();
            }

        }
    
        private List<messagemodel> getHistoryMessage(string message_from_id, string message_to_id,OracleCommand command)
        {
            List<messagemodel> resultList = new List<messagemodel>();
            string sqlStr = @" select t.message_from_id,
                                               t.message_to_id,
                                               t.message_type,
                                               t.message_content,
                                                t.message_time
                                          from message_content t
                                         where t.message_from_id = :v_message_from_id
                                           and t.message_to_id=:v_message_to_id
                                         order by t.message_time asc ";
            try
            {
                command.CommandText = sqlStr;
                command.Parameters.Clear();
                command.Parameters.Add("v_message_from_id", OracleDbType.Varchar2).Value = message_from_id;
                command.Parameters.Add("v_message_to_id", OracleDbType.Varchar2).Value = message_to_id;

                using (OracleDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        messagemodel model = null;
                        while (reader.Read())
                        {
                            model = new messagemodel();

                            model.message_type = "normal";
                            model.message_from = DBContext.getOracleStringItem(reader, "message_from_id");
                            model.message_to = DBContext.getOracleStringItem(reader, "message_to_id");
                            model.message_date = DBContext.getOracleDateTimeItem(reader, "message_time");
                            int message_type = DBContext.getOracleInt32Item(reader, "message_type");
                            if (message_type == 1) //代表文件
                            {
                                //根据路径读取文件内容
                                string path = DBContext.getOracleStringItem(reader, "message_content");
                                model.message_content = getContent(path);
                            }
                            else
                            {
                                model.message_content = DBContext.getOracleStringItem(reader, "message_content");
                            }

                            resultList.Add(model);
                        }
                    }
                }


                return resultList;
            }
            catch (Exception)
            {

                return resultList;
            }
        }


        //读取文件内容
        private string getContent(string file_path)
        {
            StreamReader sr = new StreamReader(file_path, Encoding.Default);
            String line = sr.ReadToEnd();
            sr.Dispose();
            sr.Close();
            return line;
        }
    }

    public class PatientVisitModel
    {
        public string MessageCode { get; set; }
        public string MessageContent { get; set; }
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
    }

    public class PatientModel
    {
        public string VisitID { get; set; }
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string Sex { get; set; }
    }

    //界面参数
    public class ClinicDoctorModel
    {
        public string DoctorID { get; set; }
        public string DeptID { get; set; }
        public string VisitDate { get; set; }
        public string RegisterType { get; set; }
    }

    public class messagemodel
    {
        public string message_type { get; set; }
        public string message_from { get; set; }
        public string message_to { get; set; }
        public string message_content { get; set; }
        public DateTime message_date { get; set; }
    }
}
