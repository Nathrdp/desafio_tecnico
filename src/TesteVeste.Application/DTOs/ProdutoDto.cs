namespace TesteVeste.Application.DTOs;

public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCadastro { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
}
