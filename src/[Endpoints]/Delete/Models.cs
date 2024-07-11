using FluentValidation;

namespace Endpoints.ItemMenu.Delete;

public sealed class Request
{
    public Guid Id { get; set; } = Guid.Empty;
}

public sealed class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Id).NotEmpty()
                                 .NotNull()
                                 .Must(BeGuid).WithMessage("Invalid Guid");
    }

    private bool BeGuid(Guid guid) => guid != Guid.Empty;
}
