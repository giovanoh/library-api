using FluentAssertions;

using Library.API.Domain.Models;
using Library.API.DTOs;
using Library.API.Extensions;

namespace Library.API.Tests.Mapping;

public class BookOrderMappingTests : MappingTestsBase
{
    [Fact]
    public void Should_Map_BookOrder_To_BookOrderDto()
    {
        // Arrange
        var order = new BookOrder
        {
            Id = 1,
            Status = BookOrderStatus.Placed,
            CheckoutDate = DateTime.UtcNow,
            Items = new List<BookOrderItem>
            {
                new BookOrderItem
                {
                    Id = 1,
                    BookId = 1,
                    Book = new Book
                    {
                        Id = 1,
                        Title = "Book 1",
                        Description = "Book 1 is a book about software engineering."
                    },
                    Quantity = 1
                }
            }
        };

        // Act
        var result = Mapper.Map<BookOrderDto>(order);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
        result.Status.Should().Be(order.Status.ToDescription());
        result.CheckoutDate.Should().Be(order.CheckoutDate);
        result.Items.Should().HaveCount(order.Items.Count);
        result.Items[0].BookId.Should().Be(order.Items[0].BookId);
        result.Items[0].Quantity.Should().Be(order.Items[0].Quantity);
        result.Items[0].Title.Should().Be(order.Items[0].Book.Title);
    }

    [Fact]
    public void Should_Map_SaveBookOrderDto_To_BookOrder()
    {
        // Arrange
        var dto = new SaveBookOrderDto
        {
            Items = new List<SaveBookOrderItemDto>
            {
                new SaveBookOrderItemDto
                {
                    BookId = 1,
                    Quantity = 1,
                }
            }
        };

        // Act
        var result = Mapper.Map<BookOrder>(dto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(0);
        result.Status.Should().Be(BookOrderStatus.Placed);
        result.CheckoutDate.Should().Be(default);
        result.Items.Should().HaveCount(dto.Items.Count);
        result.Items[0].Book.Should().BeNull();
        result.Items[0].BookOrderId.Should().Be(0);
        result.Items[0].BookId.Should().Be(dto.Items[0].BookId);
        result.Items[0].Quantity.Should().Be(dto.Items[0].Quantity);
    }
}
