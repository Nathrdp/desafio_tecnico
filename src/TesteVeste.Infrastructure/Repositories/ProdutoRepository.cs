using Microsoft.EntityFrameworkCore;
using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;
using TesteVeste.Infrastructure.Data;

namespace TesteVeste.Infrastructure.Repositories;

/// <summary>
/// Repositório de produtos usando Entity Framework Core (InMemory).
/// Implementação espelhada no padrão de CategoriaRepository.cs.
/// </summary>
public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Produto>> GetAllAsync(int pagina, int tamanhoPagina)
    {
        var query = _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking();

        var totalItens = await query.CountAsync();

        var itens = await query
            .OrderBy(p => p.Id)
            .Skip((pagina - 1) * tamanhoPagina)
            .Take(tamanhoPagina)
            .ToListAsync();

        return new PagedResult<Produto>
        {
            Pagina = pagina,
            TamanhoPagina = tamanhoPagina,
            TotalItens = totalItens,
            Itens = itens
        };
    }

    public async Task<Produto?> GetByIdAsync(int id)
    {
        return await _context.Produtos
            .Include(p => p.Categoria)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExistsWithNameAsync(string nome, int? excludeId = null)
    {
        return await _context.Produtos
            .AnyAsync(p => p.Nome.ToLower() == nome.ToLower()
                        && (excludeId == null || p.Id != excludeId));
    }

    public async Task AddAsync(Produto produto)
    {
        await _context.Produtos.AddAsync(produto);
    }

    public void Update(Produto produto)
    {
        _context.Produtos.Update(produto);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
