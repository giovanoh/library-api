using FluentAssertions;

using Library.API.Domain.Models;
using Library.API.Extensions;
using Library.Events.Messages;

namespace Library.API.Tests.Mapping;

public class OrderPlacedEventTests : MappingTestsBase
{
    [Fact]
    public void Should_Map_BookOrder_To_OrderPlacedEvent()
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
                    Book = new Book
                    {
                        Id = 1,
                        Title = "Book 1",
                        Description = "Book 1 is a book about software engineering."
                    },
                    BookId = 1,
                    Quantity = 1
                }
            }
        };

        // Act
        var result = Mapper.Map<OrderPlacedEvent>(order);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be(order.Id);
        result.Status.Should().Be(order.Status.ToDescription());
        result.CheckoutDate.Should().Be(order.CheckoutDate);
        result.Items.Should().HaveCount(order.Items.Count);
        result.Items[0].BookId.Should().Be(order.Items[0].BookId);
        result.Items[0].Quantity.Should().Be(order.Items[0].Quantity);
    }
}