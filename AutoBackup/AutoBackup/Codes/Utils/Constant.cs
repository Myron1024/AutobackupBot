using System.Configuration;

namespace AutoBackup.Codes.Utils
{

    /// <summary>
    /// 常量配置类
    /// </summary>
    public class Constant
    {
        /// <summary>
        /// 默认机器人管理员ID，用于接受通知消息
        /// </summary>
        public static string DEFAULT_ADMIN_ID = "810534137";

        /// <summary>
        /// 机器人token
        /// </summary>
        public static string token = ConfigurationManager.AppSettings["token"].ToString();

        /// <summary>
        /// BOT_CODE 显示到程序标题栏
        /// </summary>
        public static string BOT_CODE = ConfigurationManager.AppSettings["BOT_CODE"]?.ToString() ?? "AutoBackup";

        /// <summary>
        /// AUTHOR 显示到标题栏
        /// </summary>
        public static string AUTHOR = " - by @ticlab";

        public static string apiBasePath = "https://api.telegram.org/bot" + token + "/";
    }
}
