using System.ComponentModel;

namespace Library.API.Domain.Models;

public enum BookOrderStatus
{
    [Description("Order Placed")]
    Placed,
    [Description("Payment Failed")]
    PaymentFailed,
    [Description("Payment Confirmed")]
    PaymentConfirmed,
    [Description("Order Processing")]
    Processing,
    [Description("Order Shipped")]
    Shipped,
    [Description("Order Delivered")]
    Delivered,
    [Description("Order Completed")]
    Completed
}
