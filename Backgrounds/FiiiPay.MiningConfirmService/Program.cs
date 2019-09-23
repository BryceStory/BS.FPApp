using System.ServiceProcess;

namespace FiiiPay.MiningConfirmService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var ServicesToRun = new ServiceBase[]
            {
                new MainService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
