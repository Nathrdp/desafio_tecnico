using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Domain.Interfaces;

public interface IProdutoRepository
{
    Task<PagedResult<Produto>> GetAllAsync(int pagina, int tamanhoPagina);
    Task<Produto?> GetByIdAsync(int id);
    Task<bool> ExistsWithNameAsync(string nome, int? excludeId = null);
    Task AddAsync(Produto produto);
    void Update(Produto produto);
    Task<bool> SaveChangesAsync();
}
