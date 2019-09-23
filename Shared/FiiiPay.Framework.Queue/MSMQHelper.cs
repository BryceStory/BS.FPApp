using System;
using System.Messaging;
using System.Threading.Tasks;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.MSMQHelper
    /// </summary>
    public class MSMQHelper
    {
        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path.
        /// </value>
        public string Path { get; set; }

        private MessageQueue _messageQueue;

        /// <summary>
        /// Gets or sets the receive complate.
        /// </summary>
        /// <value>
        /// The receive complate.
        /// </value>
        public Action<Message> ReceiveComplate { get; set; }

        /// <summary>
        /// 创建MSMQ队列
        /// </summary>
        /// <param name="queuePath">路径地址 demo:FormatName:Direct=TCP:121.0.0.1//private$//queue</param>
        /// <param name="isNetwork">是否远程地址</param>
        public MSMQHelper(string queuePath, bool isNetwork)
        {
            if (!isNetwork)
            {
                //检查队列是否存在
                if (!MessageQueue.Exists(queuePath))
                {
                    //创建队列
                    MessageQueue.Create(queuePath);
                }                
            }
            Path = queuePath;
        }

        /// <summary>
        /// 向MSMQ发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="msg">消息正文</param>
        /// <param name="priority"></param>
        public void SendMessage<T>(T msg, MessagePriority priority)
        {
            //声明队列
            MessageQueue queue = new MessageQueue(Path);

            //创建消息
            Message message = new Message();
            message.Priority = priority;
            message.Body = msg;
            message.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });

            //发送消息
            queue.Send(message);
        }

        /// <summary>
        /// 从MSMQ中接收消息
        /// </summary>
        public Task<Message> ReciveMessage()
        {
            MessageQueue queue = new MessageQueue(Path);
            return Task.Factory.StartNew(queue.Receive);
        }

        /// <summary>
        /// Begins the recive message.
        /// </summary>
        public void BeginReciveMessage()
        {
            if (_messageQueue == null)
                _messageQueue = new MessageQueue(Path);

            _messageQueue.ReceiveCompleted += MessageQueueOnReceiveCompleted;
            _messageQueue.BeginReceive();
        }

        private void MessageQueueOnReceiveCompleted(object sender, ReceiveCompletedEventArgs receiveCompletedEventArgs)
        {
            MessageQueue mq = (MessageQueue)sender;
            var message = mq.EndReceive(receiveCompletedEventArgs.AsyncResult);
            Task.Factory.StartNew(() => { ReceiveComplate?.Invoke(message); });
            mq.BeginReceive();
        }
    }
}
