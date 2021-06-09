using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Statistics;

namespace TelemetryConsumer.Windows
{
    public static class ServiceCollectionExtensions
    {
        public static ISiloHostBuilder AddWindowsTelemetryConsumer(this ISiloHostBuilder siloBuilder) => siloBuilder.UsePerfCounterEnvironmentStatistics();
        
        public static ISiloBuilder AddWindowsTelemetryConsumer(this ISiloBuilder siloBuilder) => siloBuilder.UsePerfCounterEnvironmentStatistics();
    }
}