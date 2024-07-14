namespace Endpoints.ItemMenu.GetById;

public class Mapper : Mapper<Request, Response, object>
{
    public Response ToResponse(SoulMenu.Api.Domain.ItemMenu i) => new()
    {
        Item = new ItemMenuDto
        (
            i.Id,
            i.Name,
            i.Description,
            i.Price,
            i.Stock,
            i.Ingredients.Select(x => new IngredientDto(x.Name, x.Calories)),
            ((int)i.Category),
            i.Category.ToString(),
            i.Size.ToString(),
            i.IsActive
        )
    };
}