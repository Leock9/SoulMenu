﻿using Npgsql;

namespace SoulMenu.Consumer.Infrastructure.PostgreDb;

public class Context
{
    private readonly string _connectionString;

    public Context(string connectionString)
    {
        _connectionString = connectionString;
    }

    public NpgsqlConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
