using RabbitMQ.Client.Events;

namespace Messaging.Interfaces
{
    public interface IConsumer
    {
        public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback);
    }
}
