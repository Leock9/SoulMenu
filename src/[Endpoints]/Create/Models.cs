using FluentValidation;
using SoulMenu.Api.Domain.ValueObjects;

namespace Endpoints.ItemMenu.Create;

public sealed class Request
{
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Price { get; init; }

    public int Stock { get; init; }

    public IEnumerable<Ingredient> Ingredients { get; init; } = new List<Ingredient>();

    public Size Size { get; init; }

    public IEnumerable<Additional> Additionals { get; init; } = new List<Additional>();

    public int CategoryId { get; init; }
}

internal sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
                            .NotEmpty()
                            .NotNull();

        RuleFor(x => x.Description)
                            .NotEmpty()
                            .NotNull()
                            .Length(5, 120);

        RuleFor(x => x.Price)
                            .NotEmpty()
                            .NotNull()
                            .GreaterThan(0);

        RuleFor(x => x.Stock)
                            .NotEmpty()
                            .NotNull();

        RuleFor(x => x.Size)
                            .NotEmpty()
                            .NotNull()
                            .IsInEnum().WithMessage("Size must by: 0 = S, 1 = M or 2 = L!");

        RuleFor(x => x.Ingredients)
                            .NotEmpty()
                            .NotNull();

        RuleFor(x => x.CategoryId)
                            .GreaterThan(0)
                            .LessThan(5)
                            .NotEmpty()
                            .NotNull();
    }
}

public sealed class Response
{
    public string Message { get; init; } = string.Empty;
}

