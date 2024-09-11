namespace SoulMenu.Consumer.Domain.UseCases;

public interface IItemMenuUseCase
{
    Task<bool> SimulateOrder(IList<string> simulateOrderIds);
}
