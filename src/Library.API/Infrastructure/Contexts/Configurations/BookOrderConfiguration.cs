using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Library.API.Domain.Models;

namespace Library.API.Infrastructure.Contexts.Configurations;

public class BookOrderConfiguration : IEntityTypeConfiguration<BookOrder>
{
    public void Configure(EntityTypeBuilder<BookOrder> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.Id)
            .IsUnique();
        builder.Property(b => b.CheckoutDate)
            .IsRequired();
        builder.HasMany(b => b.Items)
            .WithOne()
            .HasForeignKey(i => i.BookOrderId);
    }
}
