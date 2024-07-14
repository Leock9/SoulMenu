using Npgsql;
using SoulMenu.Api.Domain.Gateways;
using SoulMenu.Api.Domain;
using System.Text.Json;

namespace SoulMenu.Api.Infrastructure.PostgreDb.Gateways;

public class ItemMenuGateway : IItemMenuGateway
{
    private readonly Context _context;

    public ItemMenuGateway(Context context)
    {
        _context = context;
        EnsureTableExistsAsync().Wait();
    }

    public void Create(ItemMenu itemMenu)
    {
        var conn = _context.GetConnection();
        conn.Open();

        var jsonItemMenu = JsonSerializer.Serialize(itemMenu);
        using var cmd = new NpgsqlCommand("INSERT INTO itemmenus (id, data) VALUES (@id, @data::jsonb)", conn);

        cmd.Parameters.AddWithValue("@id", itemMenu.Id);
        cmd.Parameters.AddWithValue("@data", jsonItemMenu);

        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public void Delete(Guid id)
    {
        var conn = _context.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand("DELETE FROM itemmenus WHERE id = @p", conn);
        cmd.Parameters.AddWithValue("@p", id);
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    public async Task<ItemMenu> GetByIdAsync(Guid id)
    {
        await using var conn = _context.GetConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("SELECT data FROM itemmenus WHERE id = @Id", conn);

        cmd.Parameters.AddWithValue("Id", id);

        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.Read()) return null;

        var jsonData = reader.GetString(0);
        await conn.CloseAsync();
        return JsonSerializer.Deserialize<ItemMenu>(jsonData);
    }

    public async Task<IEnumerable<ItemMenu>> GetByCategory(int categoryId)
    {
        var itemMenus = new List<ItemMenu>();

        await using var conn = _context.GetConnection();
        await conn.OpenAsync();
        await using var cmd = new NpgsqlCommand("SELECT data FROM itemmenus WHERE data->>'Category' = @Category", conn);

        cmd.Parameters.AddWithValue("Category", categoryId.ToString());

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var jsonData = reader.GetString(0);
            var itemMenu = JsonSerializer.Deserialize<ItemMenu>(jsonData);
            if (itemMenu != null)
                itemMenus.Add(itemMenu);
        }

        await conn.CloseAsync();
        return itemMenus;
    }

    public void Update(ItemMenu itemMenu)
    {
        using var conn = _context.GetConnection();
        conn.Open();

        var query = @"
        UPDATE itemmenus
        SET data = @Data
        WHERE data->>'id' = @Id::text";

        using var cmd = new NpgsqlCommand(query, conn);

        var itemMenuJson = JsonSerializer.Serialize(itemMenu);
        cmd.Parameters.AddWithValue("Data", itemMenuJson);
        cmd.Parameters.AddWithValue("Id", itemMenu.Id);

        cmd.ExecuteNonQuery();
        conn.Close();
    }

    private async Task EnsureTableExistsAsync()
    {
        var conn = _context.GetConnection();
        conn.Open();

        using var cmd = new NpgsqlCommand
                (
                   "CREATE TABLE IF NOT EXISTS itemmenus (id uuid PRIMARY KEY, data jsonb)",
                   conn
                );

        await cmd.ExecuteNonQueryAsync();
        conn.Close();
    }
}
