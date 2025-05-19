namespace Library.Checkout.Consumers;

public static class ConsumerRegistry
{
    private static readonly Dictionary<string, Type> _registry = new(StringComparer.OrdinalIgnoreCase)
        {
            { "order-placed", typeof(OrderPlacedConsumer) },
            { "payment-confirmed", typeof(PaymentConfirmedConsumer) },
            { "order-processing", typeof(OrderProcessingConsumer) },
            { "order-shipped", typeof(OrderShippedConsumer) },
            { "order-delivered", typeof(OrderDeliveredConsumer) },
        };

    public static bool TryGetConsumer(string step, out Type? consumerType) =>
        _registry.TryGetValue(step, out consumerType);

    public static IEnumerable<string> ValidSteps => _registry.Keys;
}