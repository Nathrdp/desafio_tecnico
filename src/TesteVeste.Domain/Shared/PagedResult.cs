namespace TesteVeste.Domain.Shared;

public class PagedResult<T>
{
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas => TamanhoPagina > 0
        ? (int)Math.Ceiling((double)TotalItens / TamanhoPagina)
        : 0;
    public IEnumerable<T> Itens { get; set; } = Enumerable.Empty<T>();
}
