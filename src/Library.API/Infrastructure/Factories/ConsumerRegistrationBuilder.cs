using MassTransit;

namespace Library.API.Infrastructure.Factories;

public class ConsumerRegistrationBuilder
{
    private readonly IRegistrationConfigurator _configurator;
    private readonly string _suffix;

    public ConsumerRegistrationBuilder(IRegistrationConfigurator configurator, string suffix)
    {
        _configurator = configurator;
        _suffix = suffix;
    }

    public ConsumerRegistrationBuilder Add<TConsumer>() where TConsumer : class, IConsumer
    {
        SuffixedConsumerDefinition<TConsumer>.CurrentSuffix = _suffix;
        _configurator.AddConsumer<TConsumer, SuffixedConsumerDefinition<TConsumer>>();
        return this;
    }
}

public class SuffixedConsumerDefinition<TConsumer> : ConsumerDefinition<TConsumer>
    where TConsumer : class, IConsumer
{
    public static string? CurrentSuffix;
    public SuffixedConsumerDefinition()
    {
        var consumerName = KebabCaseEndpointNameFormatter.Instance.Consumer<TConsumer>();
        var suffix = CurrentSuffix;
        EndpointName = string.IsNullOrEmpty(suffix) ? consumerName : $"{consumerName}-{suffix}";
    }
}