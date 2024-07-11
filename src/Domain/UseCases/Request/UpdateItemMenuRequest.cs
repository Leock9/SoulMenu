using SoulMenu.Api.Domain.ValueObjects;

namespace SoulMenu.Api.Domain.UseCases.Request;

public record UpdateItemMenuRequest : BaseItemMenuRequest
{
    public UpdateItemMenuRequest
    (
        Guid id,
        string name,
        string description,
        decimal price,
        int stock,
        IEnumerable<Ingredient> ingredients,
        Size size,
        Category category
    ) : base(name, description, price, stock, ingredients, size, category)
    {
        Id = id;
    }

    public Guid Id { get; init; }
}
