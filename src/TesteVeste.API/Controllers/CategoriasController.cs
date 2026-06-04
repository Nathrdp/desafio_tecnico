using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteVeste.Application.DTOs;
using TesteVeste.Application.Interfaces;
using TesteVeste.Domain.Shared;

namespace TesteVeste.API.Controllers;

/// <summary>
/// Controller de categorias — implementação completa.
/// Use este arquivo como referência para implementar o ProdutosController.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service)
    {
        _service = service;
    }

    /// <summary>Retorna todas as categorias ativas.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(CommandResult<IEnumerable<CategoriaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    /// <summary>Retorna uma categoria pelo Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CommandResult<CategoriaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.Succeeded ? Ok(result) : NotFound(result);
    }
}
