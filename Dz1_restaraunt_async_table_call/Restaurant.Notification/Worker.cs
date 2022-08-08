using System.Text;
using Messaging;
using Messaging.Interfaces;
using Microsoft.Extensions.Hosting;


namespace Restaurant.Notification
{
    public class Worker : BackgroundService
    {
        private readonly IConsumer _consumer;

        public Worker()
        {
            //важно чтобы имя очереди совпадало
            _consumer = new Consumer("BookingNotification");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(() =>
            {
                _consumer.Receive((sender, args) =>
                {
                    byte[] body = args.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body); //декодируем
                    Console.WriteLine("[x] Received {0}", message);
                });
            }, stoppingToken).ConfigureAwait(true);
        }
    }
}
