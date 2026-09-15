using BibliotecaApi.Data;
using BibliotecaApi.Dtos;
using BibliotecaApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Controllers;

[ApiController]
[Route("api/livros")]
[Produces("application/json")]
public class LivrosController(AppDbContext db) : ControllerBase
{
    /// <summary>Lista todos os livros.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LivroResponse>>> Listar()
    {
        var livros = await db.Livros.Include(l => l.Autor).OrderBy(l => l.Id).ToListAsync();
        return Ok(livros.Select(LivroResponse.From));
    }

    /// <summary>Detalha um livro pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponse>> Detalhar(int id)
    {
        var livro = await db.Livros.Include(l => l.Autor).FirstOrDefaultAsync(l => l.Id == id);
        return livro is null ? NotFound() : Ok(LivroResponse.From(livro));
    }

    /// <summary>Cria um novo livro.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LivroResponse>> Criar(LivroRequest request)
    {
        if (!await db.Autores.AnyAsync(a => a.Id == request.AutorId))
            return AutorInexistente(request.AutorId);

        var livro = new Livro
        {
            Titulo = request.Titulo,
            Isbn = request.Isbn,
            AnoPublicacao = request.AnoPublicacao,
            Paginas = request.Paginas,
            AutorId = request.AutorId
        };
        db.Livros.Add(livro);
        await db.SaveChangesAsync();
        await db.Entry(livro).Reference(l => l.Autor).LoadAsync();
        return CreatedAtAction(nameof(Detalhar), new { id = livro.Id }, LivroResponse.From(livro));
    }

    /// <summary>Atualiza totalmente um livro.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponse>> Atualizar(int id, LivroRequest request)
    {
        var livro = await db.Livros.FindAsync(id);
        if (livro is null) return NotFound();
        if (!await db.Autores.AnyAsync(a => a.Id == request.AutorId))
            return AutorInexistente(request.AutorId);

        livro.Titulo = request.Titulo;
        livro.Isbn = request.Isbn;
        livro.AnoPublicacao = request.AnoPublicacao;
        livro.Paginas = request.Paginas;
        livro.AutorId = request.AutorId;
        await db.SaveChangesAsync();
        await db.Entry(livro).Reference(l => l.Autor).LoadAsync();
        return Ok(LivroResponse.From(livro));
    }

    /// <summary>Atualiza parcialmente um livro.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LivroResponse>> AtualizarParcial(int id, LivroPatchRequest request)
    {
        var livro = await db.Livros.FindAsync(id);
        if (livro is null) return NotFound();
        if (request.AutorId is not null && !await db.Autores.AnyAsync(a => a.Id == request.AutorId))
            return AutorInexistente(request.AutorId.Value);

        if (request.Titulo is not null) livro.Titulo = request.Titulo;
        if (request.Isbn is not null) livro.Isbn = request.Isbn;
        if (request.AnoPublicacao is not null) livro.AnoPublicacao = request.AnoPublicacao.Value;
        if (request.Paginas is not null) livro.Paginas = request.Paginas.Value;
        if (request.AutorId is not null) livro.AutorId = request.AutorId.Value;
        await db.SaveChangesAsync();
        await db.Entry(livro).Reference(l => l.Autor).LoadAsync();
        return Ok(LivroResponse.From(livro));
    }

    /// <summary>Remove um livro.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id)
    {
        var livro = await db.Livros.FindAsync(id);
        if (livro is null) return NotFound();

        db.Livros.Remove(livro);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private ActionResult AutorInexistente(int autorId)
    {
        ModelState.AddModelError(nameof(LivroRequest.AutorId), $"Autor {autorId} não existe.");
        return ValidationProblem(ModelState);
    }
}
