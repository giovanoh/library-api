using MassTransit;

using Library.API.Infrastructure.Factories;

namespace Library.API.Extensions;

public static class MassTransitFluentExtensions
{
    public static ConsumerRegistrationBuilder WithEndpointSuffix(this IRegistrationConfigurator configurator, string suffix)
    {
        return new ConsumerRegistrationBuilder(configurator, suffix);
    }
}