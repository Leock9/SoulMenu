using SoulMenu.Consumer.Domain.ValueObjects;

namespace SoulMenu.Consumer.Domain;

public record ItemMenu
(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    IEnumerable<Ingredient> Ingredients,
    Size Size,
    Category Category
)
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; init; } = string.IsNullOrEmpty(Name) ?
                                        throw new DomainException("Name is required") : Name;

    public string Description { get; init; } = string.IsNullOrEmpty(Description) ?
                                               throw new DomainException("Description is required") : Description;

    public decimal Price { get; init; } = Price <= 0 ?
                                          throw new DomainException("Price is required") : Price;

    public int Stock { get; init; } = Stock <= 0 ?
                                      throw new DomainException("Stock is required") : Stock;

    public IEnumerable<Ingredient> Ingredients { get; init; } = Ingredients.Any() ? Ingredients :
                                            throw new DomainException("Ingredients is required");

    public Size Size { get; init; } = GetSize((int)Size);

    public bool IsActive { get; set; } = false;

    public Category Category { get; init; } = GetCategory((int)Category);

    public DateTime CreateAt { get; init; } = DateTime.Now;

    public DateTime UpdateAt { get; set; }

    public void Activate()
    {
        if (IsAvailable()) IsActive = true;
        else throw new DomainException("ItemMenu is invalid to be activated!");
    }

    public void Deactivate() => IsActive = false;

    public ItemMenu Update(ItemMenu itemMenu)
    {
        var updatedItemMenu = this with
        {
            Id = this.Id,
            Name = itemMenu.Name ?? this.Name,
            Description = itemMenu.Description ?? this.Description,
            Price = itemMenu.Price,
            Stock = itemMenu.Stock,
            Ingredients = itemMenu.Ingredients ?? this.Ingredients,
            Size = itemMenu.Size,
            Category = itemMenu.Category,
            UpdateAt = DateTime.Now
        };

        if (updatedItemMenu.Stock <= 0)
            updatedItemMenu = updatedItemMenu with { IsActive = false };

        return updatedItemMenu;
    }

    public bool IsAvailable() => Stock > 0 && Price > 0 && Ingredients.Any();

    public static Size GetSize(int size)
    {
        return size switch
        {
            0 => Size.S,
            1 => Size.M,
            2 => Size.L,
            _ => throw new DomainException("Size must by: 0 = S, 1 = M or 2 = L!")
        };
    }

    public static Category GetCategory(int category)
    {
        return category switch
        {
            0 => Category.Drink,
            1 => Category.Sandwich,
            2 => Category.FrenchFries,
            3 => Category.Dessert,
            4 => Category.Salad,
            5 => Category.Combo,
            _ => throw new DomainException("Category must by: 0 = Drink, 1 = Sandwich, 2 = FrenchFries, 3 = Dessert, 4 = Saladr, 5 = Combo!")
        };
    }
}
