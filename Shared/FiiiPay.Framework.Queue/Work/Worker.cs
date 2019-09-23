using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.Worker.BaseWorker
    /// </summary>
    public class Worker : BaseRabbit, IWoker
    {
        private readonly IList<string> _queues = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Worker"/> class.
        /// </summary>
        public Worker() { }

        /// <summary>
        /// Starts the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public virtual void Start(string queueName, Func<string, bool> action)
        {
            QueueDeclare(queueName);

            Channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(Channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                var result = action(message);
                if (result)
                {
                    Channel.BasicAck(ea.DeliveryTag, true);
                }
                else
                {
                    Channel.BasicReject(ea.DeliveryTag, false);
                }
            };

            Channel.BasicConsume(queueName, false, consumer);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public virtual void Stop()
        {
            Dispose();
        }

        /// <summary>
        /// Queues the declare.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        protected override void QueueDeclare(string queueName)
        {
            if (_queues.Contains(queueName))
            {
                Channel.QueueDeclare(queueName, true, false, false, null);
                _queues.Add(queueName);
            }
        }
    }
}
