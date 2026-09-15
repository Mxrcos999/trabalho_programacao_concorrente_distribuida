using System.ComponentModel.DataAnnotations;
using BibliotecaApi.Models;

namespace BibliotecaApi.Dtos;

public record AutorResponse(int Id, string Nome, string? Nacionalidade, DateOnly? DataNascimento, int QuantidadeLivros)
{
    public static AutorResponse From(Autor a) =>
        new(a.Id, a.Nome, a.Nacionalidade, a.DataNascimento, a.Livros.Count);
}

/// <summary>Dados para criação ou atualização total (PUT) de um autor.</summary>
public record AutorRequest(
    [Required, StringLength(150)] string Nome,
    [StringLength(80)] string? Nacionalidade,
    DateOnly? DataNascimento);

/// <summary>Atualização parcial (PATCH): apenas os campos enviados são alterados.</summary>
public record AutorPatchRequest(
    [StringLength(150, MinimumLength = 1)] string? Nome,
    [StringLength(80)] string? Nacionalidade,
    DateOnly? DataNascimento);
