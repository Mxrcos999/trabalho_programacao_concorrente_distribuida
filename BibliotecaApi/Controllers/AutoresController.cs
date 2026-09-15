using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/autores")]
[Produces("application/json")]
public class AutoresController(AppDbContext db) : ControllerBase
{
    /// <summary>Lista todos os autores.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AutorResponse>>> Listar()
    {
        var autores = await db.Autores.Include(a => a.Livros).OrderBy(a => a.Id).ToListAsync();
        return Ok(autores.Select(AutorResponse.From));
    }

    /// <summary>Detalha um autor pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponse>> Detalhar(int id)
    {
        var autor = await db.Autores.Include(a => a.Livros).FirstOrDefaultAsync(a => a.Id == id);
        return autor is null ? NotFound() : Ok(AutorResponse.From(autor));
    }

    /// <summary>Lista os livros de um autor.</summary>
    [HttpGet("{id:int}/livros")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<LivroResponse>>> ListarLivros(int id)
    {
        var autor = await db.Autores.Include(a => a.Livros).FirstOrDefaultAsync(a => a.Id == id);
        return autor is null ? NotFound() : Ok(autor.Livros.OrderBy(l => l.Id).Select(LivroResponse.From));
    }

    /// <summary>Cria um novo autor.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AutorResponse>> Criar(AutorRequest request)
    {
        var autor = new Autor
        {
            Nome = request.Nome,
            Nacionalidade = request.Nacionalidade,
            DataNascimento = request.DataNascimento
        };
        db.Autores.Add(autor);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Detalhar), new { id = autor.Id }, AutorResponse.From(autor));
    }

    /// <summary>Atualiza totalmente um autor.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponse>> Atualizar(int id, AutorRequest request)
    {
        var autor = await db.Autores.Include(a => a.Livros).FirstOrDefaultAsync(a => a.Id == id);
        if (autor is null) return NotFound();

        autor.Nome = request.Nome;
        autor.Nacionalidade = request.Nacionalidade;
        autor.DataNascimento = request.DataNascimento;
        await db.SaveChangesAsync();
        return Ok(AutorResponse.From(autor));
    }

    /// <summary>Atualiza parcialmente um autor.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AutorResponse>> AtualizarParcial(int id, AutorPatchRequest request)
    {
        var autor = await db.Autores.Include(a => a.Livros).FirstOrDefaultAsync(a => a.Id == id);
        if (autor is null) return NotFound();

        if (request.Nome is not null) autor.Nome = request.Nome;
        if (request.Nacionalidade is not null) autor.Nacionalidade = request.Nacionalidade;
        if (request.DataNascimento is not null) autor.DataNascimento = request.DataNascimento;
        await db.SaveChangesAsync();
        return Ok(AutorResponse.From(autor));
    }

    /// <summary>Remove um autor (e seus livros).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        var autor = await db.Autores.FindAsync(id);
        if (autor is null) return NotFound();

        db.Autores.Remove(autor);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
