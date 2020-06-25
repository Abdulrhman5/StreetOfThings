using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBus
{
    public interface IRabbitMQPersistentConnection
        : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
