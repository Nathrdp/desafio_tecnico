using TesteVeste.Application.DTOs;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Application.Interfaces;

public interface IProdutoService
{
    Task<CommandResult<PagedResult<ProdutoDto>>> GetAllAsync(int pagina, int tamanhoPagina);
    Task<CommandResult<ProdutoDto>> GetByIdAsync(int id);
    Task<CommandResult<ProdutoDto>> CreateAsync(CreateProdutoDto dto);
    Task<CommandResult<ProdutoDto>> UpdateAsync(int id, UpdateProdutoDto dto);
    Task<CommandResult<bool>> DeleteAsync(int id);
}
