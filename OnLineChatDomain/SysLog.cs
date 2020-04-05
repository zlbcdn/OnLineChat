using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OnLineChatDomain
{
    public class SysLog
    {
        static string v_filePath = @"c:/TsinghuaHIS/Data/Pharmacy/Debug/";
        /// <summary>
        /// 创建文件夹
        /// </summary>
        private static void CreateLogFile(string fileName)
        {
            if (!Directory.Exists(v_filePath))
            {
                try
                {
                    Directory.CreateDirectory(v_filePath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            if (!File.Exists(v_filePath + fileName))
            {
                try
                {
                    FileStream file = File.Create(v_filePath + fileName);
                    file.Dispose();
                    file.Close();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        ///向日志文件中写入日志
        /// </summary>
        /// <param name="logText"></param>
        public static void LogWrite(string fileName, string logText)
        {
            CreateLogFile(fileName);

            string fileWholePath = v_filePath + fileName;

            StreamWriter sw = new StreamWriter(fileWholePath, true, System.Text.Encoding.UTF8);
            try
            {

                sw.WriteLine("/********************************/");
                sw.WriteLine("日期：" + System.DateTime.Now.ToString());
                sw.WriteLine();
                sw.WriteLine(logText);
                sw.WriteLine("/********************************/");
                sw.WriteLine();
                sw.WriteLine();
                sw.Flush();
                sw.Dispose();
                sw.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sw.Dispose();
                sw.Close();
            }
        }

        public static void LogWrite(string fileName, Exception ex)
        {
            CreateLogFile(fileName);
            string fileWholePath = v_filePath + fileName;

            string filePath = "c:/TsinghuaHIS/Data/Pharmacy/Debug/OrderLogFile.log";

            StreamWriter sw = new StreamWriter(filePath, true, System.Text.Encoding.UTF8);
            try
            {
                sw.WriteLine("/********************************/");
                sw.WriteLine("日期：" + System.DateTime.Now.ToString());
                sw.WriteLine();
                sw.WriteLine("错误源：" + ex.Source);
                sw.WriteLine("错误信息：" + ex.Message);
                sw.WriteLine("堆栈信息：" + ex.StackTrace);
                sw.WriteLine("引发错误的方法：" + ex.TargetSite);
                sw.WriteLine("/********************************/");
                sw.WriteLine();
                sw.WriteLine();
                sw.Flush();
                sw.Dispose();
                sw.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sw.Dispose();
                sw.Close();
            }
        }
    }
}
