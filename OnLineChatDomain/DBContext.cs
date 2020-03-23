using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace OnLineChatDomain
{
    public class DBContext
    {
        #region 获得数据库连接
        public static OracleConnection GetZLOracleConnection()
        {
            string connectionStr = @"User Id=bthuipd; Password=ipdbthu;  Data Source=BTHU1";

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connectionStr;
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
            return conn;
        }
        #endregion
        #region 获得数据库连接
        public static OracleConnection GetDIMSOracleConnection()
        {
            string connectionStr = @"User Id=dimsprog; Password=dimsprog;  Data Source=oracle_main";

            OracleConnection conn = new OracleConnection();
            conn.ConnectionString = connectionStr;
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
            return conn;
        }
        #endregion

        //获取Oracle的String类型数据
        public static string getOracleStringItem(OracleDataReader reader, string columnName)
        {
            if (!Convert.IsDBNull(reader[columnName]))
            {
                return reader.GetString(reader.GetOrdinal(columnName));
            }
            else
            {
                return "";
            }
        }

        //获取Oracle的Int32类型数据
        public static Int32 getOracleInt32Item(OracleDataReader reader, string columnName)
        {
            if (!Convert.IsDBNull(reader[columnName]))
            {
                return Convert.ToInt32(reader[columnName]);
            }
            else
            {
                return 0;
            }
        }

        //获取Oracle的DateTime类型数据
        public static DateTime getOracleDateTimeItem(OracleDataReader reader, string columnName)
        {
            if (!Convert.IsDBNull(reader[columnName]))
            {
                return Convert.ToDateTime(reader[columnName]);
            }
            else
            {
                return new DateTime(2014, 11, 28, 12, 0, 0); //清华长庚医院的开业日期
            }
        }


    }
}
