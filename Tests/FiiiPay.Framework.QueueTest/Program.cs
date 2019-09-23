using FiiiPay.Framework.Cache;
using FiiiPay.Framework.Queue;
using static System.Console;

namespace FiiiPay.Framework.QueueTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //Send();
            //DeadLetterSend();
            //DelaySend();

            //Work("Framework_DelayTest-delay");

            var t = RedisHelper.KeyTimeToLive(2, "FiiiPay:RedPocket:17b9d142-614c-44bc-b41f-881b25559b68");

            ReadKey();

            //Stop();
        }

        public static void Send()
        {
            ISender sender = new SendClient();
            var result = sender.Send("Framework_Test", "Test");
            WriteLine("Send " + result);
        }

        public static void DeadLetterSend()
        {
            var sender = new DeadLetterClient(30000);
            var result = sender.Send("Framework_DeadLetterTest", "Test");
            WriteLine("Dead Letter Send " + result);
        }

        public static void DelaySend()
        {
            ISender sender = new DelaySendClient(30000);
            var result = sender.Send("Framework_DelayTest", "Test");
            WriteLine("Delay Send " + result);
        }
        
        public static void Work(string queueName)
        {
            _woker = new Worker();
            _woker.Start(queueName, Action);
        }

        public static void Stop()
        {
            _woker?.Stop();
        }

        private static bool Action(string arg)
        {
            WriteLine("Receive message : " + arg);

            return false;
        }

        private static IWoker _woker;
    }
}
