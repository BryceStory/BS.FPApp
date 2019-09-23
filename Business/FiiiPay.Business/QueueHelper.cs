using FiiiPay.Framework.Queue;

namespace FiiiPay.Business
{
    internal class QueueHelper
    {
        private static ISender _sender;
        private static ISender _delaySender;

        public const int DelayTTL = 86400000;// 24H
        //public const int DelayTTL = 900000;// 15M

        public static ISender Sender => _sender ?? (_sender = new SendClient());

        public static ISender DelaySender => _delaySender ?? (_delaySender = new DelaySendClient(DelayTTL));
    }
}
