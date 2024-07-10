using SoulMenu.Api.Domain.ValueObjects;

namespace SoulMenu.Api.Domain.UseCases.Request;

public record BaseItemMenuRequest
(
        string Name,
        string Description,
        decimal Price,
        int Stock,
        IEnumerable<Ingredient> Ingredients,
        Size Size,
        Category Category
);
