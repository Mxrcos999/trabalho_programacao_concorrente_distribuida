using BibliotecaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BibliotecaApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Livro> Livros => Set<Livro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Autor>(e =>
        {
            e.ToTable("autores");
            e.Property(a => a.Nome).HasMaxLength(150).IsRequired();
            e.Property(a => a.Nacionalidade).HasMaxLength(80);
        });

        modelBuilder.Entity<Livro>(e =>
        {
            e.ToTable("livros");
            e.Property(l => l.Titulo).HasMaxLength(200).IsRequired();
            e.Property(l => l.Isbn).HasMaxLength(20);
            e.HasOne(l => l.Autor)
                .WithMany(a => a.Livros)
                .HasForeignKey(l => l.AutorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
