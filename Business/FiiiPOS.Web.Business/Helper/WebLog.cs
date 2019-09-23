using log4net;

namespace FiiiPOS.Web.Business
{
    /// <summary>  
    /// WebLog的摘要说明。   
    /// </summary>   
    public class WebLog
    {
        private static ILog log;
        private static WebLog webLog = null;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public static ILog GetInstance()
        {
            webLog = new WebLog(null);

            return log;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static ILog GetInstance(string configPath)
        {
            webLog = new WebLog(configPath);

            return log;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="configPath"></param>
        private WebLog(string configPath)
        {
            if (!string.IsNullOrEmpty(configPath))
            {
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo(configPath));
            }
            else
            {
                log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            }
        }

    }
}
