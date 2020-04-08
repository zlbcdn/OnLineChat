using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnLineChatDomain;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;

namespace OnLineChat.Controllers
{
    public class ConsultationController : Controller
    {
        // GET: Consultation
        public ActionResult Index()
        {
            string sessionid = Request.QueryString["id"];
            LoginSessionModel model = new LoginSessionModel();

            model.LoginID = sessionid;

            return View(model);
        }

        /**
            1、创建sessionID的文件夹
            2、获取问题ID，并生成文件，文件名称中包含：问题ID、标题、状态（是否回复）
            3、文件中包含的内容有：1、详细信息：xxx；2、图片附件路径。
            4、图片单独存储为文件

            举例：

            提问的问题：
            存储的路径：
            c:/program/sessionid/questionid/questionid^title^status.txt
            其中：status：0：代表新增待回复；1：新增已回复；2：修改待回复；3：修改已回复

            提问详细内容
            xxxxx
            提问附件1：c:/program/sessionid/questionid/附件1.txt
            提问附件2：c:/program/sessionid/questionid/附件2.txt
            回复详细内容
            yyyyyyy
            回复附件1：c:/program/sessionid/questionid/附件1.txt
            回复附件2：c:/program/sessionid/questionid/附件2.txt
             **/
        //提交
        public string addConsultation(string id,string consultationid, string title,string content,string appendix)
        {
            var message_result = new { result = false, msg = "" };

            try
            {
                //创建文件夹
                string full_path = createDirectory(id, consultationid);
                if(full_path=="0"|| full_path=="-1")
                {
                    string message_str = string.Format("创建文件夹时发生异常！方法返回：{0}", full_path);
                    message_result = new { result = false, msg = message_str };
                    return JsonConvert.SerializeObject(message_result); ;
                }

                //创建文件
                string file_name = consultationid + "^" + title + "^0.txt";
                string file_full_path = Path.Combine(full_path, file_name);
                if (!System.IO.File.Exists(file_full_path))
                {
                    FileStream file = System.IO.File.Create(file_full_path);
                    file.Dispose();
                    file.Close();
                }

                //形成内容
                string content_detail = "提问详细内容" + Environment.NewLine;
                content_detail += content + Environment.NewLine;

                //追加附件内容
                if(!string.IsNullOrEmpty(appendix))
                {
                    string temp_str = "";
                    string[] split_file_array = appendix.Split('^');
                    for (int i = 0; i < split_file_array.Count(); i++)
                    {
                        if(i== split_file_array.Count()-1)
                        {
                            temp_str += string.Format("提问附件{0}:{1}", i, split_file_array[i]);
                        }
                        else
                        {
                            temp_str += string.Format("提问附件{0}:{1}", i, split_file_array[i])+Environment.NewLine;
                        }
                    }
                    content_detail += temp_str;
                }

                //写入文件
                Write(file_full_path, content_detail);

                message_result = new { result = true, msg = "OK" };
                return JsonConvert.SerializeObject(message_result);
            }
            catch (Exception ex)
            {
                message_result = new { result = false, msg = "发生异常！exception具体信息："+ex.Message };
                return JsonConvert.SerializeObject(message_result);
            }
        }

        //生成唯一ID
        private string CreateId()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        //写文件
        public void Write(string path,string content)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write(content);
            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }

        //返回问题ID
        public string getConsultationID(string id)
        {
            return CreateId();
        }

        //形成唯一文件夹
        private string createDirectory(string id,string consultation_id)
        {
            string result_str = "0";
            try
            {
                System.Collections.Specialized.NameValueCollection keyValueCollection = ConfigurationManager.AppSettings;
                string store_path = keyValueCollection["store_path"];

                //创建路径
                string full_path = Path.Combine(store_path, id, consultation_id);
                if (!Directory.Exists(full_path)) Directory.CreateDirectory(full_path);

                return full_path;
            }
            catch (Exception)
            {
                result_str ="-1";
                return result_str;
            }
        }

        //形成基本的路径
        private string createBaseDirectory(string id)
        {
            string result_str = "0";
            try
            {
                System.Collections.Specialized.NameValueCollection keyValueCollection = ConfigurationManager.AppSettings;
                string store_path = keyValueCollection["store_path"];

                //创建路径
                string full_path = Path.Combine(store_path, id);
                if (!Directory.Exists(full_path)) Directory.CreateDirectory(full_path);

                return full_path;
            }
            catch (Exception)
            {
                result_str = "-1";
                return result_str;
            }
        }

        //上传文件
        public string upLoad(HttpPostedFileBase fileData,string id, string consultationid)
        {
            var message = new { result = false, msg = "init",appendmsg="", appendfilename = "" };
            try
            {
                if (fileData == null || String.IsNullOrEmpty(fileData.FileName) || fileData.ContentLength == 0)
                {
                    message = new { result = false, msg = "接收参数异常，文件为null", appendmsg = "",appendfilename="" };
                    return JsonConvert.SerializeObject(message);
                }

                string base_path = createDirectory(id, consultationid);
                string filename = System.IO.Path.GetFileName(fileData.FileName);
                string full_path = Path.Combine(base_path, filename);

                //判断是否存在，若存在，则删除文件
                if (System.IO.File.Exists(full_path)) System.IO.File.Delete(full_path);

                fileData.SaveAs(full_path);
                message = new { result = true, msg = "ok", appendmsg = full_path, appendfilename = filename };

                return JsonConvert.SerializeObject(message);
            }
            catch(Exception ex)
            {
                message = new { result = true, msg = "发生异常！异常信息如下："+ex.Message, appendmsg = "", appendfilename = "" };
                return JsonConvert.SerializeObject(message);
            }
        }

        //根据ID获取问题列表
        public string getConsultation(string id)
        {
            List<ConsultationModel> resultList = new List<ConsultationModel>();
            var message_result = new { result = false, msg = "init",data= resultList };
            try
            {
                string base_path= createBaseDirectory(id);
                if(base_path=="0"||base_path=="-1")
                {
                    message_result = new { result = false, msg = "创建文件时发生异常，返回为"+ base_path, data = resultList };
                    return JsonConvert.SerializeObject(message_result);
                }

                //获取路径下的所有文件夹
                DirectoryInfo dir = new DirectoryInfo(base_path);
                DirectoryInfo[] dirArray = dir.GetDirectories();
                for (int i = 0; i < dirArray.Length; i++)
                {
                    ConsultationModel model = new ConsultationModel();

                    model.consultationid = dirArray[i].Name;

                    DirectoryInfo temp_dir = new DirectoryInfo(dirArray[i].FullName);
                    FileInfo[] fil_array = temp_dir.GetFiles("*.txt");
                    if (fil_array.Length == 1) {
                        string temp_file_name= fil_array[0].Name;
                        string[] file_name_array = temp_file_name.Split('^');

                        if(file_name_array.Length==3)
                        {
                            model.consultationtitle = file_name_array[1];
                            model.consultationstatus = file_name_array[2];

                            resultList.Add(model);
                        }
                    }
                }

                message_result = new { result = true, msg = "OK", data = resultList };
                return JsonConvert.SerializeObject(message_result);
            }
            catch (Exception ex)
            {
                message_result = new { result = false, msg = "创建文件时发生异常，错误信息" + ex.Message, data = resultList };
                return JsonConvert.SerializeObject(message_result);
            }
        }

        public string getConsultationDetail(string id,string consultationid)
        {
            List<ConsultationDetailModel> resultList = new List<ConsultationDetailModel>();
            var message_result = new { result = false, msg = "init", data = resultList };
            try
            {
                string dir_path = createDirectory(id, consultationid);

                DirectoryInfo dir = new DirectoryInfo(dir_path);
                FileInfo[] fil_array = dir.GetFiles("*.txt");
                if (fil_array.Length == 1)
                {
                    StreamReader sr = new StreamReader(fil_array[0].FullName, System.Text.Encoding.UTF8);
                    List<string> tempList = new List<string>();
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        tempList.Add(line);
                    }
                    sr.Dispose();
                    sr.Close();

                    int first_index = 0;
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        if(tempList[i].Contains("提问附件"))
                        {
                            first_index = i;
                            break;
                        }
                    }

                    string content = "";
                    for (int i = 1; i < first_index; i++)
                    {
                        if(i== first_index-1)
                        {
                            content += tempList[i];
                        }
                        else
                        {
                            content += (tempList[i] + Environment.NewLine);
                        }
                        
                    }


                    ConsultationDetailModel model = new ConsultationDetailModel();
                    model.consultationdetail = content;
                    string[] title_array = fil_array[0].Name.Split('^');
                    if (title_array.Length == 3) model.consultationtitle = title_array[1];

                    model.consultationid = consultationid;


                    resultList.Add(model);
                }


                message_result = new { result = true, msg = "OK", data = resultList };

                return JsonConvert.SerializeObject(message_result);

            }
            catch (Exception ex)
            {
                message_result = new { result = false, msg = "发生异常，异常原因"+ex.Message, data = resultList };
                return JsonConvert.SerializeObject(message_result);
            }
        }
    }


    public class ConsultationModel
    {
        public string consultationid { get; set; }
        public string consultationtitle { get; set; }
        public string consultationstatus { get; set; }
    }

    public class ConsultationDetailModel
    {
        public string consultationid { get; set; }
        public string consultationtitle { get; set; }
        public string consultationdetail { get; set; }
        public string consultationappend { get; set; }
    }



}