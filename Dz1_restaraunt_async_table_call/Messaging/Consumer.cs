using Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging
{
    public class Consumer : IConsumer
    {
        private string _queueName; //название очереди

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly IConfigurationSection _exchangeSection;

        public Consumer(string queueName)
        {
            _queueName = queueName;

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            IConfigurationSection sect = config.GetSection("HostConfig");
            _exchangeSection = config.GetSection("ExchangeDeclare");

            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = sect.GetSection("HostName").Value,
                VirtualHost = sect.GetSection("VirtualHost").Value,
                UserName = sect.GetSection("UserName").Value,
                Password = sect.GetSection("Password").Value,
                Port = int.Parse(sect.GetSection("Port").Value),
                RequestedHeartbeat = TimeSpan.FromSeconds(int.Parse(sect.GetSection("RequestedHeartbeat").Value)),
                Ssl = new SslOption()
                {
                    Enabled = true,
                    AcceptablePolicyErrors = System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch |
                                 System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors,
                    Version = System.Security.Authentication.SslProtocols.Tls11 |
                  System.Security.Authentication.SslProtocols.Tls12
                }

            };
            _connection = factory.CreateConnection(); //создаем подключение
            _channel = _connection.CreateModel();
        }

        public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
        {
            _channel.ExchangeDeclare(
                _exchangeSection.GetSection("Exchange").Value,
                _exchangeSection.GetSection("Type").Value,
                bool.Parse(_exchangeSection.GetSection("Durable").Value),
                bool.Parse(_exchangeSection.GetSection("AutoDelete").Value),
                null); // объявляем обменник

            if (_exchangeSection.GetSection("Type").Value.ToLower().Equals("fanout"))
            {
                _queueName = _channel.QueueDeclare().QueueName;//объявляем очередь fanout
            }
            else
            {
                _channel.QueueDeclare(
                    queue: _queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null); //объявляем очередь direct
            }

            _channel.QueueBind(
                queue: _queueName,
                exchange: _exchangeSection.GetSection("Exchange").Value,
                routingKey: _queueName); //биндим

            EventingBasicConsumer consumer = new EventingBasicConsumer(_channel); // создаем consumer для канала
            consumer.Received += receiveCallback; // добавляем обработчик события приема сообщения

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer); //стартуем!
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}