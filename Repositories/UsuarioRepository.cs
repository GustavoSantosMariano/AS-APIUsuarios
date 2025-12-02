using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;
using APIUsuarios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace APIUsuarios.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _db;

    public UsuarioRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<Usuario>> GetAllAsync(CancellationToken ct)
        => await _db.Usuarios.AsNoTracking().ToListAsync(ct);

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken ct)
        => await _db.Usuarios.FindAsync(new object?[] { id }, ct);

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken ct)
        => await _db.Usuarios.FirstOrDefaultAsync(x => x.Email == email, ct);

    public async Task AddAsync(Usuario usuario, CancellationToken ct)
        => await _db.Usuarios.AddAsync(usuario, ct);

    public Task UpdateAsync(Usuario usuario, CancellationToken ct)
    {
        _db.Usuarios.Update(usuario);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(Usuario usuario, CancellationToken ct)
    {
        _db.Usuarios.Remove(usuario);
        return Task.CompletedTask;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        => await _db.Usuarios.AnyAsync(x => x.Email == email, ct);

    public async Task<int> SaveChangesAsync(CancellationToken ct)
        => await _db.SaveChangesAsync(ct);
}