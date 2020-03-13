using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

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
                                               and t.reg_time_name = :v_reg_time_name";


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
}
