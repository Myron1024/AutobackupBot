using AutoBackup.Codes;
using AutoBackup.Codes.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackup
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "【" + Constant.BOT_CODE + "】自动备份数据 " + Constant.AUTHOR;

            Console.WriteLine("初始化程序...");

            // 取当前作用域
            AppDomain currentDomain = AppDomain.CurrentDomain;
            // 当前作用域出现未捕获异常时，使用MyExceptionHandler函数响应事件
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyExceptionHandler);

            //初始化日志配置
            log4net.Config.XmlConfigurator.Configure();

            // 初始化启动定时任务引擎
            QuartzUtil.Init();

            // 启动设定的任务
            QuartzBase.Start();

            Console.WriteLine();
            Console.WriteLine("开启定时任务..");
            Console.WriteLine();

            QuartzUtil.PrintAllJobsInfo();
            Console.WriteLine();

            Console.ReadLine();
        }


        private static void MyExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception) args.ExceptionObject;
            Log.Error("未经处理的异常 : " + e.Message, e);

            Console.Write("未经处理的异常。 ");
            Console.ReadKey();
        }
    }
}
