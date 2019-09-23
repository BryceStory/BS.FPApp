using System.ServiceProcess;

namespace FiiiPos.MessageWorkerService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            var ServicesToRun = new ServiceBase[]
            {
                new RabbitService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
