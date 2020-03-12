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

    }

    public class PatientVisitModel
    {
        public string PatientID { get; set; }
        public string PatientName { get; set; }
        public string DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string DeptID { get; set; }
        public string DeptName { get; set; }
    }
}
