using Library.API.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.API.Infrastructure.Contexts.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.Id)
            .IsUnique();
        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(100);
        builder.Property(a => a.BirthDate)
            .IsRequired(false);
        builder.Property(a => a.Biography)
            .IsRequired(false)
            .HasMaxLength(1000);
        builder.HasMany(a => a.Books)
            .WithOne(b => b.Author)
            .HasForeignKey(b => b.AuthorId);            
    }
}