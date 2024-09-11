using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client.Events;

namespace SoulMenu.Consumer.Infrastructure.RabbitMq;

public interface IQueueService
{
    void Publish(OrderMessage orderMessage);
    Task<OrderMessage> ConsumeAsync();
}

public class QueueService : IQueueService
{
    private readonly IModel _channel;
    private readonly IConnection _connection;

    public QueueService(IRabbitMqSettings rabbitMqSettings)
    {
        var factory = new ConnectionFactory
        {
            HostName = rabbitMqSettings.HostName,
            UserName = rabbitMqSettings.UserName,
            Password = rabbitMqSettings.Password,
            VirtualHost = "/"
        };

        // Criar conexão e canal persistente
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public async Task<OrderMessage> ConsumeAsync()
    {
        // Declarar a fila se ela não existir
        _channel.QueueDeclare(
            queue: "SimulateOrder",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var tcs = new TaskCompletionSource<OrderMessage>();
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var order = JsonSerializer.Deserialize<OrderMessage>(message);

                // Confirmar o processamento da mensagem
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
                tcs.SetResult(order);
            }
            catch (Exception)
            {
                // Em caso de falha, reencaminhar a mensagem para a fila
                _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        // Consumir a mensagem
        _channel.BasicConsume(
            queue: "SimulateOrder",
            autoAck: false,  // Confirmação manual habilitada
            consumer: consumer
        );

        return await tcs.Task;
    }

    public void Publish( OrderMessage orderMessage)
    {
        // Declara a fila com durabilidade e persistência de mensagem
        _channel.QueueDeclare(
            queue: "order-status",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(orderMessage));

        // Propriedades para persistir a mensagem
        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        // Publica a mensagem na fila
        _channel.BasicPublish(
            exchange: "",
            routingKey: orderMessage.Status.ToString(),
            basicProperties: properties,
            body: body
        );
    }
}
