using System;
#if !DEBUG
using System.ServiceProcess;
#endif

namespace FiiiPay.MessageWorkerService
{ 
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            var service = new RabbitService();
#if DEBUG
            service.Start();
            Console.ReadLine();
            service.Stop();
#else
            var ServicesToRun = new ServiceBase[]
            {
                service
            };
            ServiceBase.Run(ServicesToRun);
#endif

        }
    }
}
