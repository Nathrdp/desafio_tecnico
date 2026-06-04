using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteVeste.Application.DTOs;
using TesteVeste.Application.Interfaces;
using TesteVeste.Domain.Shared;

namespace TesteVeste.API.Controllers;

/// <summary>
/// Controller do CRUD de Produtos.
/// Implementação espelhada no padrão de CategoriasController.cs.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoService _service;

    public ProdutosController(IProdutoService service)
    {
        _service = service;
    }

    /// <summary>Lista paginada de produtos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(CommandResult<PagedResult<ProdutoDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10)
    {
        var result = await _service.GetAllAsync(pagina, tamanhoPagina);
        return Ok(result);
    }

    /// <summary>Retorna um produto pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CommandResult<ProdutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }

    /// <summary>Cria um novo produto.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommandResult<ProdutoDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateProdutoDto dto)
    {
        var result = await _service.CreateAsync(dto);

        if (!result.Succeeded)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);
    }

    /// <summary>Atualiza um produto existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CommandResult<ProdutoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProdutoDto dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (result.Succeeded)
            return Ok(result);

        // Produto inexistente → 404; demais falhas de negócio → 400.
        return result.Messages.Any(m => m.Contains("não encontrado", StringComparison.OrdinalIgnoreCase))
            ? NotFound(result)
            : BadRequest(result);
    }

    /// <summary>Desativa um produto (soft delete).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result.Succeeded ? NoContent() : NotFound(result);
    }
}
