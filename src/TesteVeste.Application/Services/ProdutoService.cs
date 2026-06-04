using TesteVeste.Application.DTOs;
using TesteVeste.Application.Interfaces;
using TesteVeste.Application.Notifications;
using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;

namespace TesteVeste.Application.Services;

/// <summary>
/// Serviço de produtos — aplica as regras de negócio do CRUD.
/// Implementação espelhada no padrão de CategoriaService.cs.
///
/// REGRAS DE NEGÓCIO:
/// [1] Nome é obrigatório e deve ter no máximo 100 caracteres.
/// [2] Preço deve ser maior que zero.
/// [3] Não pode existir dois produtos com o mesmo nome (ignorar capitalização).
/// [4] Um produto INATIVO não pode ser editado (UpdateAsync deve falhar).
/// [5] O DeleteAsync é um "soft delete": apenas define Ativo = false.
/// </summary>
public class ProdutoService : IProdutoService
{
    private const int NomeMaxLength = 100;

    private readonly IProdutoRepository _repository;
    private readonly INotificationService _notifications;

    public ProdutoService(IProdutoRepository repository, INotificationService notifications)
    {
        _repository = repository;
        _notifications = notifications;
    }

    public async Task<CommandResult<PagedResult<ProdutoDto>>> GetAllAsync(int pagina, int tamanhoPagina)
    {
        var paged = await _repository.GetAllAsync(pagina, tamanhoPagina);

        var dto = new PagedResult<ProdutoDto>
        {
            Pagina = paged.Pagina,
            TamanhoPagina = paged.TamanhoPagina,
            TotalItens = paged.TotalItens,
            Itens = paged.Itens.Select(MapToDto)
        };

        return CommandResult<PagedResult<ProdutoDto>>.Success(dto);
    }

    public async Task<CommandResult<ProdutoDto>> GetByIdAsync(int id)
    {
        var produto = await _repository.GetByIdAsync(id);

        if (produto is null)
        {
            _notifications.AddNotification("Produto não encontrado.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }

        return CommandResult<ProdutoDto>.Success(MapToDto(produto));
    }

    public async Task<CommandResult<ProdutoDto>> CreateAsync(CreateProdutoDto dto)
    {
        // Regras 1 e 2 — validação de campos.
        if (!ValidarCampos(dto.Nome, dto.Preco))
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);

        // Regra 3 — nome duplicado (ignora maiúsculas/minúsculas).
        if (await _repository.ExistsWithNameAsync(dto.Nome, null))
        {
            _notifications.AddNotification("Já existe um produto com este nome.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }

        var produto = new Produto
        {
            Nome = dto.Nome,
            Descricao = dto.Descricao,
            Preco = dto.Preco,
            Estoque = dto.Estoque,
            CategoriaId = dto.CategoriaId,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _repository.AddAsync(produto);
        await _repository.SaveChangesAsync();

        return CommandResult<ProdutoDto>.Success(MapToDto(produto));
    }

    public async Task<CommandResult<ProdutoDto>> UpdateAsync(int id, UpdateProdutoDto dto)
    {
        var produto = await _repository.GetByIdAsync(id);

        if (produto is null)
        {
            _notifications.AddNotification("Produto não encontrado.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }

        // Regra 4 — produto inativo não pode ser editado.
        if (!produto.Ativo)
        {
            _notifications.AddNotification("Produto inativo não pode ser editado.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }

        // Regras 1 e 2 — validação de campos.
        if (!ValidarCampos(dto.Nome, dto.Preco))
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);

        // Regra 3 — nome duplicado, ignorando o próprio produto.
        if (await _repository.ExistsWithNameAsync(dto.Nome, id))
        {
            _notifications.AddNotification("Já existe um produto com este nome.");
            return CommandResult<ProdutoDto>.Failure(_notifications.Notifications);
        }

        produto.Nome = dto.Nome;
        produto.Descricao = dto.Descricao;
        produto.Preco = dto.Preco;
        produto.Estoque = dto.Estoque;
        produto.CategoriaId = dto.CategoriaId;

        _repository.Update(produto);
        await _repository.SaveChangesAsync();

        return CommandResult<ProdutoDto>.Success(MapToDto(produto));
    }

    public async Task<CommandResult<bool>> DeleteAsync(int id)
    {
        var produto = await _repository.GetByIdAsync(id);

        if (produto is null)
        {
            _notifications.AddNotification("Produto não encontrado.");
            return CommandResult<bool>.Failure(_notifications.Notifications);
        }

        // Regra 5 — soft delete.
        produto.Ativo = false;
        _repository.Update(produto);
        await _repository.SaveChangesAsync();

        return CommandResult<bool>.Success(true);
    }

    /// <summary>
    /// Valida nome (obrigatório, máx. 100 caracteres) e preço (maior que zero).
    /// Registra notificações para cada regra violada e retorna false se houver erro.
    /// </summary>
    private bool ValidarCampos(string nome, decimal preco)
    {
        var valido = true;

        if (string.IsNullOrWhiteSpace(nome))
        {
            _notifications.AddNotification("Nome é obrigatório.");
            valido = false;
        }
        else if (nome.Length > NomeMaxLength)
        {
            _notifications.AddNotification($"Nome deve ter no máximo {NomeMaxLength} caracteres.");
            valido = false;
        }

        if (preco <= 0)
        {
            _notifications.AddNotification("Preço deve ser maior que zero.");
            valido = false;
        }

        return valido;
    }

    private static ProdutoDto MapToDto(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        Descricao = produto.Descricao,
        Preco = produto.Preco,
        Estoque = produto.Estoque,
        Ativo = produto.Ativo,
        DataCadastro = produto.DataCadastro,
        CategoriaId = produto.CategoriaId,
        CategoriaNome = produto.Categoria?.Nome
    };
}
