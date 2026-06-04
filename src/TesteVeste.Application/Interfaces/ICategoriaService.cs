using TesteVeste.Application.DTOs;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Application.Interfaces;

public interface ICategoriaService
{
    Task<CommandResult<IEnumerable<CategoriaDto>>> GetAllAsync();
    Task<CommandResult<CategoriaDto>> GetByIdAsync(int id);
}
