using TesteVeste.Application.DTOs;
using TesteVeste.Application.Interfaces;
using TesteVeste.Application.Notifications;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Application.Services;

/// <summary>
/// Serviço de categorias — implementação completa.
/// Use este arquivo como referência para implementar o ProdutoService.
/// </summary>
public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repository;
    private readonly INotificationService _notifications;

    public CategoriaService(ICategoriaRepository repository, INotificationService notifications)
    {
        _repository = repository;
        _notifications = notifications;
    }

    public async Task<CommandResult<IEnumerable<CategoriaDto>>> GetAllAsync()
    {
        var categorias = await _repository.GetAllAsync();

        var dtos = categorias.Select(c => new CategoriaDto
        {
            Id = c.Id,
            Nome = c.Nome
        });

        return CommandResult<IEnumerable<CategoriaDto>>.Success(dtos);
    }

    public async Task<CommandResult<CategoriaDto>> GetByIdAsync(int id)
    {
        var categoria = await _repository.GetByIdAsync(id);

        if (categoria is null)
        {
            _notifications.AddNotification("Categoria não encontrada.");
            return CommandResult<CategoriaDto>.Failure(_notifications.Notifications);
        }

        var dto = new CategoriaDto
        {
            Id = categoria.Id,
            Nome = categoria.Nome
        };

        return CommandResult<CategoriaDto>.Success(dto);
    }
}
