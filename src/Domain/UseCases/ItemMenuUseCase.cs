using SoulMenu.Api.Domain.Gateways;
using SoulMenu.Api.Domain.UseCases.Request;

namespace SoulMenu.Api.Domain.UseCases;

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

    public void Create(BaseItemMenuRequest itemMenuRequest)
    {
        try
        {
            var itemMenu = new ItemMenu
            (
                itemMenuRequest.Name,
                itemMenuRequest.Description,
                itemMenuRequest.Price,
                itemMenuRequest.Stock,
                itemMenuRequest.Ingredients,
                itemMenuRequest.Size,
                itemMenuRequest.Category
            );

            _itemMenuRepository.Create(itemMenu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public void Delete(Guid id)
    {
        try
        {
            _itemMenuRepository.Delete(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }

    }

    public async Task<ItemMenu> Get(Guid id)
    {
        try
        {
            return await _itemMenuRepository.Get(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<IEnumerable<ItemMenu>> GetByCategory(int categoryId)
    {
        try
        {
            return await _itemMenuRepository.GetByCategory(categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task Update(UpdateItemMenuRequest itemMenuRequest)
    {
        try
        {
            var itemMenu = await _itemMenuRepository.Get(itemMenuRequest.Id) ??
                                    throw new Exception("Item menu not found");

            itemMenu.Update(new ItemMenu
            (
              itemMenuRequest.Name,
              itemMenuRequest.Description,
              itemMenuRequest.Price,
              itemMenuRequest.Stock,
              itemMenuRequest.Ingredients,
              itemMenuRequest.Size,
              itemMenuRequest.Category
            ));

            _itemMenuRepository.Update(itemMenu);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
}
