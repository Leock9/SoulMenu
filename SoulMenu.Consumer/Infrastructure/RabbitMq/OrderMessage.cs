namespace SoulMenu.Consumer.Infrastructure.RabbitMq;

public class OrderMessage
{
    public Guid Id { get; init; }

    public Guid EventId { get; init; }

    public decimal TotalOrder { get; init; }

    public Status Status { get; set; }

    public string Document { get; init; }

    public IEnumerable<string> ItemMenuIds { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public Payment Payment { get; init; }
}

public enum Status
{
    NewOrder = 0,
    SimulateOrder = 1,
    PaymentPending = 2,
    Preparation = 3,
    Ready = 4,
    Finished = 5,
    Canceled = 6
}

public class Payment
{
    public Guid Id { get; init; }

    public decimal TotalOrder { get; init; }

    public bool IsAproved { get; set; } = false;
}