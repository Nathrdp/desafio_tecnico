using Moq;
using TesteVeste.Application.Interfaces;
using TesteVeste.Application.Notifications;
using TesteVeste.Application.Services;
using TesteVeste.Domain.Entities;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Domain.Shared;
using TesteVeste.Tests.Helpers;

namespace TesteVeste.Tests.Services;

public class ProdutoServiceTests
{
    private readonly Mock<IProdutoRepository> _repositoryMock;
    private readonly INotificationService _notifications;
    private readonly IProdutoService _service;

    public ProdutoServiceTests()
    {
        _repositoryMock = new Mock<IProdutoRepository>();
        _notifications  = new NotificationService();
        _service        = new ProdutoService(_repositoryMock.Object, _notifications);
    }

    // =========================================================================
    // GetByIdAsync
    // =========================================================================

    [Fact]
    public async Task GetByIdAsync_QuandoProdutoExiste_DeveRetornarSucesso()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(produto.Nome, result.Data!.Nome);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoProdutoNaoExiste_DeveRetornarFalha()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    // =========================================================================
    // GetAllAsync
    // =========================================================================

    [Fact]
    public async Task GetAllAsync_DeveRetornarListaPaginada()
    {
        // Arrange
        var pagedProdutos = new PagedResult<Produto>
        {
            Pagina = 1,
            TamanhoPagina = 10,
            TotalItens = 1,
            Itens = new List<Produto> { ProdutoFake.ProdutoValido() }
        };
        _repositoryMock.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(pagedProdutos);

        // Act
        var result = await _service.GetAllAsync(1, 10);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Single(result.Data!.Itens);
        Assert.Equal(1, result.Data.TotalItens);
    }

    // =========================================================================
    // CreateAsync
    // =========================================================================

    [Fact]
    public async Task CreateAsync_ComDadosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var dto = ProdutoFake.CreateDtoValido();
        _repositoryMock.Setup(r => r.ExistsWithNameAsync(dto.Nome, null)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Produto>())).Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(dto.Nome, result.Data!.Nome);
        Assert.Equal(dto.Preco, result.Data.Preco);
    }

    [Fact]
    public async Task CreateAsync_ComNomeVazio_DeveRetornarFalha()
    {
        // Arrange
        var dto = ProdutoFake.CreateDtoValido();
        dto.Nome = string.Empty;

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task CreateAsync_ComPrecoNegativo_DeveRetornarFalha()
    {
        // Arrange
        var dto = ProdutoFake.CreateDtoValido();
        dto.Preco = -10m;

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task CreateAsync_ComPrecoZero_DeveRetornarFalha()
    {
        // Arrange
        var dto = ProdutoFake.CreateDtoValido();
        dto.Preco = 0m;

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task CreateAsync_ComNomeDuplicado_DeveRetornarFalha()
    {
        // Arrange
        var dto = ProdutoFake.CreateDtoValido();
        _repositoryMock.Setup(r => r.ExistsWithNameAsync(dto.Nome, null)).ReturnsAsync(true);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task CreateAsync_ComNomeMuitoLongo_DeveRetornarFalha()
    {
        // Arrange
        var dto = ProdutoFake.CreateDtoValido();
        dto.Nome = new string('A', 101); // 101 caracteres — acima do limite de 100

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    // =========================================================================
    // UpdateAsync
    // =========================================================================

    [Fact]
    public async Task UpdateAsync_ComDadosValidos_DeveRetornarSucesso()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        var dto     = ProdutoFake.UpdateDtoValido();
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);
        _repositoryMock.Setup(r => r.ExistsWithNameAsync(dto.Nome, 1)).ReturnsAsync(false);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(1, dto);

        // Assert
        Assert.True(result.Succeeded);
        Assert.NotNull(result.Data);
        Assert.Equal(dto.Nome, result.Data!.Nome);
    }

    [Fact]
    public async Task UpdateAsync_QuandoProdutoNaoExiste_DeveRetornarFalha()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _service.UpdateAsync(99, ProdutoFake.UpdateDtoValido());

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task UpdateAsync_QuandoProdutoInativo_DeveRetornarFalha()
    {
        // Arrange
        var produtoInativo = ProdutoFake.ProdutoInativo();
        _repositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(produtoInativo);

        // Act
        var result = await _service.UpdateAsync(2, ProdutoFake.UpdateDtoValido());

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task UpdateAsync_ComNomeVazio_DeveRetornarFalha()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        var dto = ProdutoFake.UpdateDtoValido();
        dto.Nome = string.Empty;
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);

        // Act
        var result = await _service.UpdateAsync(1, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task UpdateAsync_ComPrecoNegativo_DeveRetornarFalha()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        var dto = ProdutoFake.UpdateDtoValido();
        dto.Preco = -1m;
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);

        // Act
        var result = await _service.UpdateAsync(1, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task UpdateAsync_ComNomeDuplicado_DeveRetornarFalha()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        var dto = ProdutoFake.UpdateDtoValido();
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);
        _repositoryMock.Setup(r => r.ExistsWithNameAsync(dto.Nome, 1)).ReturnsAsync(true);

        // Act
        var result = await _service.UpdateAsync(1, dto);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    // =========================================================================
    // DeleteAsync
    // =========================================================================

    [Fact]
    public async Task DeleteAsync_QuandoProdutoExiste_DeveRetornarSucesso()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result.Succeeded);
        Assert.True(result.Data);
        // Soft delete: Ativo deve ser false após a exclusão
        Assert.False(produto.Ativo);
    }

    [Fact]
    public async Task DeleteAsync_QuandoProdutoNaoExiste_DeveRetornarFalha()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Produto?)null);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        Assert.False(result.Succeeded);
        Assert.NotEmpty(result.Messages);
    }

    [Fact]
    public async Task DeleteAsync_DeveChamarSaveChanges()
    {
        // Arrange
        var produto = ProdutoFake.ProdutoValido();
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(produto);
        _repositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        // Act
        await _service.DeleteAsync(1);

        // Assert — verifica que o repositório foi chamado corretamente
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
