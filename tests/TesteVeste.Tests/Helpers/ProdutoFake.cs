using TesteVeste.Domain.Entities;
using TesteVeste.Application.DTOs;

namespace TesteVeste.Tests.Helpers;

/// <summary>
/// Fábrica de dados de teste para Produto.
/// Use estes métodos nos testes para evitar repetição de código.
/// </summary>
public static class ProdutoFake
{
    public static Produto ProdutoValido(int id = 1) => new()
    {
        Id = id,
        Nome = "Camiseta Básica",
        Descricao = "Camiseta 100% algodão",
        Preco = 49.90m,
        Estoque = 100,
        Ativo = true,
        CategoriaId = 1,
        DataCadastro = DateTime.UtcNow,
        Categoria = new Categoria { Id = 1, Nome = "Camisetas", Ativo = true }
    };

    public static Produto ProdutoInativo(int id = 2) => new()
    {
        Id = id,
        Nome = "Produto Inativo",
        Preco = 29.90m,
        Estoque = 0,
        Ativo = false,
        CategoriaId = 1,
        DataCadastro = DateTime.UtcNow,
        Categoria = new Categoria { Id = 1, Nome = "Camisetas", Ativo = true }
    };

    public static CreateProdutoDto CreateDtoValido() => new()
    {
        Nome = "Novo Produto",
        Descricao = "Descrição do produto",
        Preco = 99.90m,
        Estoque = 50,
        CategoriaId = 1
    };

    public static UpdateProdutoDto UpdateDtoValido() => new()
    {
        Nome = "Produto Atualizado",
        Descricao = "Descrição atualizada",
        Preco = 79.90m,
        Estoque = 40,
        CategoriaId = 1
    };
}
