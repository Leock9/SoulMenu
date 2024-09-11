using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using SoulMenu.Consumer.Domain.Gateways;
using SoulMenu.Consumer.Domain.UseCases;
using SoulMenu.Consumer.Infrastructure.RabbitMq;

namespace SoulMenu.Consumer.Consumer;

public class SimulateOrderWorker : BackgroundService
{
    private readonly ILogger<SimulateOrderWorker> _logger;
    private readonly IQueueService _queueService;
    private readonly AsyncCircuitBreakerPolicy _circuitBreakerPolicy;
    private readonly IItemMenuUseCase _itemMenuUseCase;
    private readonly IItemMenuGateway _itemMenuGateway;

    public SimulateOrderWorker(ILogger<SimulateOrderWorker> logger, IQueueService queueService, IItemMenuUseCase itemMenuUseCase, IItemMenuGateway itemMenuGateway)
    {
        _logger = logger;
        _queueService = queueService;
        _itemMenuUseCase = itemMenuUseCase;
        _itemMenuGateway = itemMenuGateway;

        _circuitBreakerPolicy = Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30),
              onBreak: (exception, timespan) =>
              {
                  _logger.LogWarning($"Circuito aberto por {timespan.TotalSeconds} segundos devido a erro: {exception.Message}");
              },
              onReset: () => _logger.LogInformation("Circuito fechado, a operação foi restabelecida."),
              onHalfOpen: () => _logger.LogInformation("Circuito em modo half-open, testando novamente...")
            );
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Aguardando mensagens...");

        while (!stoppingToken.IsCancellationRequested)
        {
            var order = await _queueService.ConsumeAsync();

            if (order is not null)
            {
                _logger.LogInformation($"Pedido recebido: {order.Id}");

                try
                {
                    await _circuitBreakerPolicy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation($"Simulando pedido: {order.Id}. EnventId: {order.EventId}");
                        if(!await _itemMenuUseCase.SimulateOrder(order.ItemMenuIds.ToList())) 
                        {
                           _logger.LogWarning($"Pedido {order.Id} com estoque não aprovado. EnventId: {order.EventId}");
                           order.Status = Status.Canceled;
                           _logger.LogInformation($"Pedido {order.Id} cancelado. EnventId: {order.EventId}");
                           _queueService.Publish(order);
                        }

                        _logger.LogInformation($"Pedido {order.Id} com estoque aprovado. EnventId: {order.EventId}");
                        order.Status = Status.PaymentPending;
                        _logger.LogInformation($"Pedido {order.Id} com pagamento pendente. EnventId: {order.EventId}");
                        _queueService.Publish(order);
                    });
                }
                catch (BrokenCircuitException ex)
                {
                    _logger.LogError($"Circuito aberto, rejeitando processamento do pedido {order.Id}. Erro: {ex.Message}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao processar o pedido {order.Id}: {ex.Message}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
