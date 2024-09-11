using Microsoft.Extensions.Logging;
using SoulMenu.Consumer.Domain.Gateways;

namespace SoulMenu.Consumer.Domain.UseCases;

public class ItemMenuUseCase : IItemMenuUseCase
{
    private readonly ILogger<ItemMenuUseCase> _logger;
    private readonly IItemMenuGateway _itemMenuRepository;

    public ItemMenuUseCase
    (
        IItemMenuGateway itemMenuRepository,
        ILogger<ItemMenuUseCase> logger
    )
    {
        _itemMenuRepository = itemMenuRepository;
        _logger = logger;
    }

    public async Task<bool> SimulateOrder(IList<string> simulateOrderIds)
    {
        var orderHasStock = 0; // 0 = false, 1 = true
        var cts = new CancellationTokenSource();

        try
        {
            await Parallel.ForEachAsync(simulateOrderIds, cts.Token, async (simulateOrderId, cancellationToken) =>
            {
                if (cts.Token.IsCancellationRequested)
                    return;

                if (!Guid.TryParse(simulateOrderId, out var orderId))
                {
                    _logger.LogWarning($"Invalid GUID: {simulateOrderId}");
                    return;
                }

                var itemOrder = await _itemMenuRepository.GetByIdAsync(orderId);
                if (itemOrder is null)
                {
                    _logger.LogWarning($"Item order not found: {orderId}");
                    return;
                }

                _logger.LogInformation($"Item order received: {itemOrder.Id}");

                if (!itemOrder.IsAvailable()) 
                {
                    _logger.LogInformation($"Item order has negative stock: {itemOrder.Id}");
                    cts.Cancel();  // Trigger cancellation for all remaining tasks
                    Interlocked.Exchange(ref orderHasStock, 0);
                    return;
                }

                _logger.LogInformation($"Item order has stock: {itemOrder.Id}");
                Interlocked.Exchange(ref orderHasStock, 1);
                return;
            });
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Order simulation was canceled due to insufficient stock.");
        }

        return orderHasStock == 1;
    }
}
