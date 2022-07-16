using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBackup.Codes.Utils
{
    public static class Common
    {
        /// <summary>
        /// 读取文件配置列表
        /// </summary>
        /// <returns></returns>
        public static string getFileList()
        {
            try
            {
                string fileName = "files.dll";
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\App_Data\\";
                string file = path + fileName;
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                if (!File.Exists(file))
                {
                    File.AppendAllText(file, "0");
                    return "0";
                }
                else
                {
                    string content = File.ReadAllText(file);
                    return content;
                }
            }
            catch (Exception ex)
            {
                Log.Error("获取文件配置异常", ex);
                return "0";
            }
        }
        
        /// <summary>
        /// 给管理员发送消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="isHtml"></param>
        /// <returns></returns>
        public static void SendMsgToAdmin(string msg)
        {
            NameValueCollection values = new NameValueCollection();
            NameValueCollection filesPar = new NameValueCollection();
            values.Add("chat_id", Constant.DEFAULT_ADMIN_ID);
            values.Add("text", msg);
            values.Add("parse_mode", "HTML");
            HttpUtils.HttpPostFile(Constant.apiBasePath + "sendMessage", values, filesPar);
        }
    }
}
