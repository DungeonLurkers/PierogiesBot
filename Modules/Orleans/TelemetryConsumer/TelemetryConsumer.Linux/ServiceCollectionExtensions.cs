using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Statistics;

namespace TelemetryConsumer.Linux
{
    public static class ServiceCollectionExtensions
    {
        public static ISiloHostBuilder AddLinuxTelemetryConsumer(this ISiloHostBuilder builder) => builder.UseLinuxEnvironmentStatistics();
        public static ISiloBuilder AddLinuxTelemetryConsumer(this ISiloBuilder builder) => builder.UseLinuxEnvironmentStatistics();
    }
}