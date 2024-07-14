using SoulMenu.Api.Domain.UseCases.Request;

namespace SoulMenu.Api.Domain.UseCases;

public interface IItemMenuUseCase
{
    public Guid Create(BaseItemMenuRequest itemMenuRequest);
    public Task Update(UpdateItemMenuRequest itemMenuRequest);
    public void Delete(Guid id);
    public Task<ItemMenu> GetByIdAsync(Guid id);
    public Task<IEnumerable<ItemMenu>> GetByCategory(int categoryId);
}
