﻿using SoulMenu.Api.Domain.UseCases;
using SoulMenu.Api.Domain;
using System.Net;

namespace Endpoints.ItemMenu.GetCategory;

public sealed class Endpoint : Endpoint<Request, Response, Mapper>
{
    public ILogger<Endpoint> Log { get; set; } = null!;
    public IItemMenuUseCase? ItemMenuService { get; set; }

    public override void Configure()
    {
        AllowAnonymous();
        Get("/itemMenu/GetByCategory");
    }

    public override async Task HandleAsync(Request r, CancellationToken c)
    {
        try
        {
            var itemMenus = await ItemMenuService?.GetByCategory(r.CategoryId);
            await SendAsync(Map.ToResponse(itemMenus), cancellation: c);
        }
        catch (DomainException dx)
        {
            ThrowError(dx.Message);
        }
        catch (Exception ex)
        {
            Log.LogError("Ocorreu um erro inesperado ao executar o endpoint:{typeof(Endpoint).Namespace}. {ex.Message}", typeof(Endpoint).Namespace, ex.Message);
            ThrowError("Unexpected Error", (int)HttpStatusCode.BadRequest);
        }
    }
}