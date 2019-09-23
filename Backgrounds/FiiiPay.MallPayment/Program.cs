using System;
using System.Configuration;

#if !DEBUG
using System.ServiceProcess;
#endif

namespace FiiiPay.MallPayment
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var service = new MainService();
#if DEBUG
            service.StartService();
            Console.ReadLine();
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
