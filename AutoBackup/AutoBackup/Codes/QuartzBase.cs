using AutoBackup.Codes.Jobs;
using System.Configuration;

namespace AutoBackup.Codes
{
    public class QuartzBase
    {
        public static async void Start()
        {
            string _cron = "0 30 0 * * ?";
            //string _cron = "0 0/1 * * * ?";   // for test
            _cron = ConfigurationManager.AppSettings["cron"].ToString();
            await QuartzUtil.AddJob<AutobackupJob>("AutoBackupJob", _cron);
        }
    }
}
