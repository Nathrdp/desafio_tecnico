using TesteVeste.Domain.Entities;

namespace TesteVeste.Infrastructure.Data;

public static class DataSeeder
{
    public static void Seed(AppDbContext context)
    {
        if (context.Categorias.Any()) return;

        var categorias = new List<Categoria>
        {
            new() { Id = 1, Nome = "Camisetas",  Ativo = true },
            new() { Id = 2, Nome = "Calças",     Ativo = true },
            new() { Id = 3, Nome = "Sapatos",    Ativo = true },
        };

        context.Categorias.AddRange(categorias);

        var produtos = new List<Produto>
        {
            new() { Id = 1, Nome = "Camiseta Básica Branca", Preco = 49.90m,  Estoque = 100, Ativo = true,  CategoriaId = 1, DataCadastro = DateTime.UtcNow },
            new() { Id = 2, Nome = "Camiseta Polo Azul",     Preco = 89.90m,  Estoque = 50,  Ativo = true,  CategoriaId = 1, DataCadastro = DateTime.UtcNow },
            new() { Id = 3, Nome = "Calça Jeans Slim",       Preco = 159.90m, Estoque = 30,  Ativo = true,  CategoriaId = 2, DataCadastro = DateTime.UtcNow },
            new() { Id = 4, Nome = "Tênis Casual Preto",     Preco = 199.90m, Estoque = 20,  Ativo = false, CategoriaId = 3, DataCadastro = DateTime.UtcNow },
        };

        context.Produtos.AddRange(produtos);
        context.SaveChanges();
    }
}
