namespace Endpoints.ItemMenu.GetCategory;

public sealed class Mapper : Mapper<Request, Response, object>
{
    public Response ToResponse(IEnumerable<SoulMenu.Api.Domain.ItemMenu> i) => new()
    {
        Items = i.Select(x => new ItemMenuDto
        (
            x.Id,
            x.Name,
            x.Description,
            x.Price,
            x.Stock,
            x.Ingredients.Select(x => new IngredientDto(x.Name, x.Calories)),
            ((int)x.Category),
            x.Category.ToString(),
            x.Size.ToString(),
            x.IsActive
         ))
    };
}