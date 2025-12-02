using APIUsuarios.Application.Interfaces;
using APIUsuarios.Application.Services;
using APIUsuarios.Application.Validators;
using APIUsuarios.Infrastructure.Persistence;
using APIUsuarios.Infrastructure.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddValidatorsFromAssemblyContaining<UsuarioCreateDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/usuarios", async (IUsuarioService service, CancellationToken ct)
    => Results.Ok(await service.ListarAsync(ct)));

app.MapGet("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var usuario = await service.ObterAsync(id, ct);
    return usuario is null ? Results.NotFound() : Results.Ok(usuario);
});

app.MapPost("/usuarios", async (UsuarioCreateDto dto, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var created = await service.CriarAsync(dto, ct);
        return Results.Created($"/usuarios/{created.Id}", created);
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("Email já cadastrado"))
            return Results.Conflict(ex.Message);

        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/usuarios/{id:int}", async (int id, UsuarioUpdateDto dto, IUsuarioService service, CancellationToken ct) =>
{
    try
    {
        var updated = await service.AtualizarAsync(id, dto, ct);
        return Results.Ok(updated);
    }
    catch (Exception ex)
    {
        if (ex.Message.Contains("não encontrado"))
            return Results.NotFound();

        if (ex.Message.Contains("Email já cadastrado"))
            return Results.Conflict(ex.Message);

        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/usuarios/{id:int}", async (int id, IUsuarioService service, CancellationToken ct) =>
{
    var ok = await service.RemoverAsync(id, ct);
    return ok ? Results.NoContent() : Results.NotFound();
});

await app.RunAsync();