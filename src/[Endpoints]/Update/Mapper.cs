using SoulMenu.Api.Domain.UseCases.Request;

namespace Endpoints.ItemMenu.Update;

public sealed class Mapper : Mapper<Request, Response, object>
{
    public UpdateItemMenuRequest ToRequest(Request r) => new
    (
       r.Id,
       r.Name,
       r.Description,
       r.Price.GetValueOrDefault(),
       r.Stock,
       r.Ingredients!,
       r.Size.GetValueOrDefault(),
       SoulMenu.Api.Domain.ItemMenu.GetCategory(r.CategoryId.GetValueOrDefault())
    );
}