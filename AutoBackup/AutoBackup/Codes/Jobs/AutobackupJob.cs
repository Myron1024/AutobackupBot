using AutoBackup.Codes.Utils;
using Quartz;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AutoBackup.Codes.Jobs
{
    [DisallowConcurrentExecution]   //拒绝同一时间重复执行，同一任务串行
    public class AutobackupJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Log.Info("开始备份文件列表.");
            try
            {
                string files = Common.getFileList();
                if (!string.IsNullOrEmpty(files) && files.Trim() != "0")
                {
                    string backupPath = AppDomain.CurrentDomain.BaseDirectory + "\\backup\\";

                    string[] arr = files.Split('\n');
                    foreach (var item in arr)   //循环保存的目录， 挨个压缩
                    {
                        try
                        {
                            Log.Info("正在备份 " + item);

                            string[] fArr = item.Split('|');
                            string botCode = fArr[0];   // 机器人code
                            string dataDir = fArr[1];   // 要打包的文件夹目录

                            // 创建临时文件夹， 将要打包的文件夹内容copy 进来
                            string tmpPath = AppDomain.CurrentDomain.BaseDirectory + "\\backup\\tmp_" + botCode + "\\";
                            if (!Directory.Exists(Path.GetDirectoryName(tmpPath)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(tmpPath));
                            }

                            // 先复制要备份的文件夹内容到临时文件夹，防止源文件占用，无法直接打包
                            ZipUtils.directoryCopy(dataDir, tmpPath);

                            // 然后给临时文件夹打包
                            string fileName = botCode + "-" + (DateTime.Now.ToString("yyyy-MM-dd-HH")) + "-backup.zip";
                            string file = backupPath + fileName;    // 最终的压缩文件
                            ZipUtils.CompressDirectory(tmpPath, file, 9, true);

                            // 机器人 api 接口发送文件，
                            NameValueCollection values = new NameValueCollection();
                            NameValueCollection filesPar = new NameValueCollection();
                            values.Add("chat_id", Constant.DEFAULT_ADMIN_ID);   //发送给机器人管理员。也可以换成群组对话的ID
                            //values.Add("caption", "这是文件说明");
                            filesPar.Add("document", file);
                            HttpUtils.HttpPostFile(Constant.apiBasePath + "sendDocument", values, filesPar);

                            Log.Info("文件发送成功，2s后准备删除...");

                            // 暂停2s后删除备份文件
                            Thread.Sleep(2000);
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Log.Error("备份文件并发送时出错。" + ex.Message, ex);
                            Common.SendMsgToAdmin("备份文件并发送时出错。" + ex.Message);
                        }
                    }
                }
                else
                {
                    Log.Info("要备份的文件列表未指定");
                    Common.SendMsgToAdmin("要备份的文件列表未指定");
                }
            }
            catch (Exception ex)
            {
                Log.Error("备份文件列表发生异常。", ex);
                Common.SendMsgToAdmin("备份文件列表发生异常。" + ex.Message);
            }
            finally
            {
                Common.SendMsgToAdmin("备份文件列表 job执行完毕");
                Log.Info("*********************   JOB 【" + context.JobDetail.Key + "】执行结束   *********************");
            }
        }
    }
}
