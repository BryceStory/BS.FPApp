using FiiiPay.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace FiiiiPay.MQBussiness
{
    class Program
    {
        static void Main(string[] args)
        {
            LogHelper.Config();

            //var ms = new MainService();
            //ms.Start();
            //bool c = true;
            //while (c)
            //{
            //    Console.WriteLine("token is:" + GetToken());
            //    Console.WriteLine("ReGetToken,0 no,1 yes");
            //    var r = Console.ReadLine();
            //    if (r != "1")
            //    {
            //        c = false;
            //    }
            //}
            //ms.Stop();

            try
            {
                HostFactory.Run(c =>
                {
                    c.Service<MainService>(s =>
                    {
                        s.ConstructUsing(name => new MainService());
                        s.WhenStarted(tc => tc.Start());
                        s.WhenStopped(tc => tc.Stop());
                    });
                    c.RunAsLocalSystem();
                    c.UseAssemblyInfoForServiceInfo();
                    c.SetServiceName("FiiiPayMQBussiness");
                    c.StartAutomatically();
                });
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
        }

        private static string GetToken()
        {
            string clientKey = "FiiiPay";
            string secretKey = "u6lvFYbMPlWf9nIHM5KItktyAl2trgUfWSnVB6qW4Uf6IrU8I0LciAK7ZvaLU5fW";
            string password = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + clientKey;
            string token = AES128.Encrypt(password, secretKey);

            return token;
        }
    }
}
