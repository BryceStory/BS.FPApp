using System;
using System.Configuration;
using System.Text;
using log4net;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FiiiPay.Framework.Queue
{
    /// <summary>
    /// Class FiiiPay.Framework.Queue.RabbitMQWorker
    /// </summary>
    public class RabbitMQWorker
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(RabbitMQWorker));

        private static readonly ConnectionFactory _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(ConfigurationManager.AppSettings.Get("RabbitMQConnectionString"))
        };

        private static IConnection _connection;
        private static IModel _channel;

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>
        /// The connection.
        /// </value>
        public static IConnection Connection => _connection ?? (_connection = _connectionFactory.CreateConnection());

        /// <summary>
        /// Gets the channel.
        /// </summary>
        /// <value>
        /// The channel.
        /// </value>
        public static IModel Channel => _channel ?? (_channel = Connection.CreateModel());

        /// <summary>
        /// Starts the specified queue name.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="action">The action.</param>
        public static void Start(string queueName, Action<string> action)
        {
            Channel.QueueDeclare(queueName, true, false, false, null);

            Channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                _log.InfoFormat("[x] Received {0}", message);
                action(message);
                Channel.BasicAck(ea.DeliveryTag, false);
            };

            Channel.BasicConsume(queueName, false, consumer);
            _log.Info($"Start Receive Queue - {queueName}");
        }

        /// <summary>
        /// Starts the nack.
        /// </summary>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="action">The action.</param>
        public static void StartNack(string queueName, Action<string> action)
        {
            Channel.QueueDeclare(queueName, true, false, false, null);

            Channel.BasicQos(0, 1, false);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                _log.InfoFormat("[x] Received {0}", message);

                int actionCount = 0;
                try
                {
                    action(message);

                    actionCount = 1;
                }
                catch
                {
                    actionCount = 2;
                }
                finally
                {
                    if (actionCount == 1)
                    {
                        Channel.BasicAck(ea.DeliveryTag, false);
                    }
                    else if (actionCount == 2)
                    {
                        Channel.BasicNack(ea.DeliveryTag, false, true);
                    }
                }
            };

            Channel.BasicConsume(queueName, false, consumer);
            _log.Info($"Start Receive Queue - {queueName}");
        }

        /// <summary>
        /// Starts the specified exchange.
        /// </summary>
        /// <param name="exchange">The exchange.</param>
        /// <param name="type">The type.</param>
        /// <param name="routingKey">The routing key.</param>
        /// <param name="action">The action.</param>
        public static void Start(string exchange, string type, string routingKey, Action<string> action)
        {

            Channel.ExchangeDeclare(exchange, type);

            Channel.BasicQos(0, 1, false);
            var queueName = Channel.QueueDeclare().QueueName;
            Channel.QueueBind(queueName, exchange, routingKey);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                _log.InfoFormat("[x] Received {0}", message);

                action(message);

                Channel.BasicAck(ea.DeliveryTag, false);
            };

            Channel.BasicConsume(queueName, false, consumer);
            _log.Info($"Start Receive Queue - {queueName}");
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public static void Stop()
        {
            Channel.Close();
            Connection.Close();
            Channel.Dispose();
            Connection.Dispose();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="RabbitMQWorker"/> class.
        /// </summary>
        ~RabbitMQWorker()
        {
            Stop();
        }
    }
}