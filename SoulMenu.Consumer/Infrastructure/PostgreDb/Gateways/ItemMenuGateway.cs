using Npgsql;
using SoulMenu.Consumer.Domain.Gateways;
using SoulMenu.Consumer.Domain;
using System.Text.Json;

namespace SoulMenu.Consumer.Infrastructure.PostgreDb.Gateways;

public class ItemMenuGateway : IItemMenuGateway
{
    private readonly Context _context;

    public ItemMenuGateway(Context context)
    {
        _context = context;
        EnsureTableExistsAsync().Wait();
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
