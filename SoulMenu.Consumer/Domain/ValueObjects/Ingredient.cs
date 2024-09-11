namespace SoulMenu.Consumer.Domain.ValueObjects;

public record Ingredient
{
    public string Name { get; set; } = string.Empty;

    public int Calories { get; set; } = 0;
}
