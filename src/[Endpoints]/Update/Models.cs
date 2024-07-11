using FluentValidation;
using SoulMenu.Api.Domain.ValueObjects;
using System.Net;

namespace Endpoints.ItemMenu.Update;

public sealed class Request
{
    public Guid Id { get; set; }

    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal? Price { get; init; }

    public int Stock { get; init; }

    public IEnumerable<Ingredient>? Ingredients { get; init; } = new List<Ingredient>();

    public Size? Size { get; init; }

    public IEnumerable<Additional>? Additionals { get; init; } = new List<Additional>();

    public int? CategoryId { get; init; }
}

public sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id)
                            .NotEmpty()
                            .NotNull();

        When(x => x.Name is not null, () => RuleFor(x => x.Name).Length(5, 10));

        When(x => x.Description is not null, () => RuleFor(x => x.Description).Length(5, 120));

        When(x => x.Price is not null, () => RuleFor(x => x.Price).GreaterThan(0));

        When(x => x.Size is not null, () => RuleFor(x => x.Size).IsInEnum());

        When(x => x.Ingredients is not null, () => RuleFor(x => x.Ingredients).NotEmpty());

        When(x => x.Additionals is not null, () => RuleFor(x => x.Additionals).NotEmpty());

        When(x => x.CategoryId is not null, () => RuleFor(x => x.CategoryId).GreaterThan(0).LessThan(0));
    }
}

public sealed class Response
{
    public string StatusCode { get; init; } = HttpStatusCode.OK.ToString();
}

