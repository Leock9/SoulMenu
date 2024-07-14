using FluentValidation;

namespace Endpoints.ItemMenu.GetById;

public class Request
{
    public Guid Id { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
                     .NotEmpty()
                     .NotNull();
    }
}

public class Response
{
    public ItemMenuDto Item { get; init; } = null!;
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