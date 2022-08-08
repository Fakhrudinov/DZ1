using Messaging.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;


namespace Messaging
{
    public class Producer : IProducer
    {
        private readonly string _queueName;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        private readonly IConfigurationSection _exchangeSection;

        public Producer(string queueName)
        {
            _queueName = queueName;

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();
            IConfigurationSection hostSection = config.GetSection("HostConfig");
            _exchangeSection = config.GetSection("ExchangeDeclare");

            ConnectionFactory connectionFactory = new ConnectionFactory()
            {
                HostName = hostSection.GetSection("HostName").Value,
                VirtualHost = hostSection.GetSection("VirtualHost").Value,
                UserName = hostSection.GetSection("UserName").Value,
                Password = hostSection.GetSection("Password").Value,
                Port = int.Parse(hostSection.GetSection("Port").Value),
                RequestedHeartbeat = TimeSpan.FromSeconds(int.Parse(hostSection.GetSection("RequestedHeartbeat").Value)),
                Ssl= new SslOption()
                {
                    Enabled = true,
                    AcceptablePolicyErrors = System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch |
                                             System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors,
                    Version = System.Security.Authentication.SslProtocols.Tls11 |
                              System.Security.Authentication.SslProtocols.Tls12
                }
            };

            _connection = connectionFactory.CreateConnection(); //создаем подключение
            _channel = _connection.CreateModel();
        }

        public void Send(string message)
        {
            _channel.ExchangeDeclare(
                _exchangeSection.GetSection("Exchange").Value,
                _exchangeSection.GetSection("Type").Value,
                bool.Parse(_exchangeSection.GetSection("Durable").Value),
                bool.Parse(_exchangeSection.GetSection("AutoDelete").Value),
                null
            );

            byte[] body = Encoding.UTF8.GetBytes(message); // формируем тело сообщения для отправки

            Console.WriteLine("Message copy: " + message);

            _channel.BasicPublish(
                exchange: _exchangeSection.GetSection("Exchange").Value,
                routingKey: _queueName,
                basicProperties: null,
                body: body); //отправляем сообщение
        }
    }
}
