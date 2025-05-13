using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Library.API.Domain.Models;

namespace Library.API.Infrastructure.Contexts.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.Id)
            .IsUnique();
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(b => b.Description)
            .IsRequired(false)
            .HasMaxLength(1000);
        builder.Property(b => b.ReleaseDate)
            .IsRequired(false);
        builder.HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId);
    }
}
