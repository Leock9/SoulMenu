using FluentValidation;

namespace Endpoints.ItemMenu.GetCategory;

public sealed class Request
{
    public int CategoryId { get; init; }
}

public sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.CategoryId)
                            .GreaterThan(0)
                            .LessThan(5)
                            .NotEmpty()
                            .NotNull();

    }
}

public sealed class Response
{
    public IEnumerable<ItemMenuDto> Items { get; init; } = new List<ItemMenuDto>();
}

public record ItemMenuDto
(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    IEnumerable<IngredientDto> Ingredients,
    int CategoryId,
    string CategoryName,
    string Size,
    bool IsActive
);

public record IngredientDto
(
       string Name,
       int Calories
);