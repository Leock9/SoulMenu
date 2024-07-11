using SoulMenu.Api.Domain.UseCases.Request;

namespace Endpoints.ItemMenu.Create;

public sealed class Mapper : Mapper<Request, Response, object>
{
    public BaseItemMenuRequest ToRequest(Request r) => new
        (
           r.Name,
           r.Description,
           r.Price,
           r.Stock,
           r.Ingredients,
           r.Size,
           SoulMenu.Api.Domain.ItemMenu.GetCategory(r.CategoryId)
         );
}