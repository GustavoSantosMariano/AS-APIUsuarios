using APIUsuarios.Application.DTOs;
using APIUsuarios.Application.Interfaces;
using APIUsuarios.Domain.Entities;

namespace APIUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repo;

    public UsuarioService(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<UsuarioReadDto>> ListarAsync(CancellationToken ct)
    {
        var usuarios = await _repo.GetAllAsync(ct);

        return usuarios.Select(u => new UsuarioReadDto(
            u.Id, u.Nome, u.Email, u.DataNascimento, u.Telefone, u.Ativo, u.DataCriacao
        ));
    }

    public async Task<UsuarioReadDto?> ObterAsync(int id, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        if (u is null) return null;

        return new UsuarioReadDto(
            u.Id, u.Nome, u.Email, u.DataNascimento, u.Telefone, u.Ativo, u.DataCriacao
        );
    }

    public async Task<UsuarioReadDto> CriarAsync(UsuarioCreateDto dto, CancellationToken ct)
    {
        dto = dto with { Email = dto.Email.ToLower() };

        if (await _repo.EmailExistsAsync(dto.Email, ct))
            throw new Exception("Email já cadastrado.");

        var usuario = new Usuario {
            Nome = dto.Nome,
            Email = dto.Email,
            Senha = dto.Senha,
            DataNascimento = dto.DataNascimento,
            Telefone = dto.Telefone,
            Ativo = true
        };

        await _repo.AddAsync(usuario, ct);
        await _repo.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            usuario.Id, usuario.Nome, usuario.Email,
            usuario.DataNascimento, usuario.Telefone,
            usuario.Ativo, usuario.DataCriacao
        );
    }

    public async Task<UsuarioReadDto> AtualizarAsync(int id, UsuarioUpdateDto dto, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        if (u is null) throw new Exception("Usuário não encontrado.");

        dto = dto with { Email = dto.Email.ToLower() };

        var existente = await _repo.GetByEmailAsync(dto.Email, ct);
        if (existente is not null && existente.Id != id)
            throw new Exception("Email já cadastrado.");

        u.Nome = dto.Nome;
        u.Email = dto.Email;
        u.DataNascimento = dto.DataNascimento;
        u.Telefone = dto.Telefone;
        u.Ativo = dto.Ativo;
        u.DataAtualizacao = DateTime.UtcNow;

        await _repo.UpdateAsync(u, ct);
        await _repo.SaveChangesAsync(ct);

        return new UsuarioReadDto(
            u.Id, u.Nome, u.Email,
            u.DataNascimento, u.Telefone, u.Ativo, u.DataCriacao
        );
    }

    public async Task<bool> RemoverAsync(int id, CancellationToken ct)
    {
        var u = await _repo.GetByIdAsync(id, ct);
        if (u is null) return false;

        u.Ativo = false;
        u.DataAtualizacao = DateTime.UtcNow;

        await _repo.UpdateAsync(u, ct);
        await _repo.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> EmailJaCadastradoAsync(string email, CancellationToken ct)
        => await _repo.EmailExistsAsync(email.ToLower(), ct);
}