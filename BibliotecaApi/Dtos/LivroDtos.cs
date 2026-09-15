using System.ComponentModel.DataAnnotations;
using BibliotecaApi.Models;

namespace BibliotecaApi.Dtos;

public record LivroResponse(int Id, string Titulo, string? Isbn, int AnoPublicacao, int Paginas, int AutorId, string? AutorNome)
{
    public static LivroResponse From(Livro l) =>
        new(l.Id, l.Titulo, l.Isbn, l.AnoPublicacao, l.Paginas, l.AutorId, l.Autor?.Nome);
}

/// <summary>Dados para criação ou atualização total (PUT) de um livro.</summary>
public record LivroRequest(
    [Required, StringLength(200)] string Titulo,
    [StringLength(20)] string? Isbn,
    [Range(0, 3000)] int AnoPublicacao,
    [Range(1, 100000)] int Paginas,
    [Range(1, int.MaxValue)] int AutorId);

/// <summary>Atualização parcial (PATCH): apenas os campos enviados são alterados.</summary>
public record LivroPatchRequest(
    [StringLength(200, MinimumLength = 1)] string? Titulo,
    [StringLength(20)] string? Isbn,
    [Range(0, 3000)] int? AnoPublicacao,
    [Range(1, 100000)] int? Paginas,
    [Range(1, int.MaxValue)] int? AutorId);
