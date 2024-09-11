namespace SoulMenu.Consumer.Domain.Gateways;

public interface IItemMenuGateway
{
    public Task<ItemMenu> GetByIdAsync(Guid id);
}
