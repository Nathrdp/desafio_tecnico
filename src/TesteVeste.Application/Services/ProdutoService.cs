using TesteVeste.Application.DTOs;
using TesteVeste.Application.Interfaces;
using TesteVeste.Application.Notifications;
using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Application.Services;

// =============================================================================
//  TODO — SUA TAREFA
// =============================================================================
//  Implemente todos os métodos desta classe seguindo as regras de negócio abaixo.
//  Consulte o CategoriaService.cs como referência de implementação.
//
//  REGRAS DE NEGÓCIO:
//  [1] Nome é obrigatório e deve ter no máximo 100 caracteres.
//  [2] Preço deve ser maior que zero.
//  [3] Não pode existir dois produtos com o mesmo nome (ignorar capitalização).
//  [4] Um produto INATIVO não pode ser editado (UpdateAsync deve falhar).
//  [5] O DeleteAsync é um "soft delete": apenas define Ativo = false.
//
//  DICA: Use _notifications.AddNotification("mensagem") para registrar erros
//  e retorne CommandResult<T>.Failure(_notifications.Notifications) em caso de falha.
// =============================================================================

public class ProdutoService : IProdutoService
{
    private readonly IProdutoRepository _repository;
    private readonly INotificationService _notifications;

    public ProdutoService(IProdutoRepository repository, INotificationService notifications)
    {
        _repository = repository;
        _notifications = notifications;
    }

    public Task<CommandResult<PagedResult<ProdutoDto>>> GetAllAsync(int pagina, int tamanhoPagina)
    {
        // TODO: Busque os produtos paginados no repositório e mapeie para ProdutoDto.
        throw new NotImplementedException();
    }

    public Task<CommandResult<ProdutoDto>> GetByIdAsync(int id)
    {
        // TODO: Busque o produto pelo Id.
        //       Se não existir, adicione uma notificação e retorne Failure.
        throw new NotImplementedException();
    }

    public Task<CommandResult<ProdutoDto>> CreateAsync(CreateProdutoDto dto)
    {
        // TODO: Valide os campos (regras 1 e 2), verifique duplicidade de nome (regra 3)
        //       e persista o novo produto.
        throw new NotImplementedException();
    }

    public Task<CommandResult<ProdutoDto>> UpdateAsync(int id, UpdateProdutoDto dto)
    {
        // TODO: Busque o produto, valide se está ativo (regra 4),
        //       valide os campos (regras 1 e 2), verifique duplicidade (regra 3)
        //       e salve as alterações.
        throw new NotImplementedException();
    }

    public Task<CommandResult<bool>> DeleteAsync(int id)
    {
        // TODO: Busque o produto. Se não existir, retorne Failure.
        //       Caso contrário, defina Ativo = false e salve (regra 5).
        throw new NotImplementedException();
    }
}
