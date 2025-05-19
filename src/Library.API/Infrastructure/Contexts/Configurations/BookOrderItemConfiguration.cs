using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Library.API.Domain.Models;

namespace Library.API.Infrastructure.Contexts.Configurations;

public class BookOrderItemConfiguration : IEntityTypeConfiguration<BookOrderItem>
{
    public void Configure(EntityTypeBuilder<BookOrderItem> builder)
    {
        builder.HasKey(b => b.Id);
        builder.HasIndex(b => b.Id)
            .IsUnique();
        builder.Property(b => b.Quantity)
            .IsRequired();
        builder.HasOne(b => b.Book)
            .WithMany()
            .HasForeignKey(b => b.BookId);
        builder.HasOne<BookOrder>()
            .WithMany(b => b.Items)
            .HasForeignKey(b => b.BookOrderId);
    }
}
