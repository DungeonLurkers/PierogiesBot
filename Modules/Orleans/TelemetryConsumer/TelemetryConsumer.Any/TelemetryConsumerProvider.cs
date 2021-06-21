using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
#if _WINDOWS
using TelemetryConsumer.Windows;
#else
using TelemetryConsumer.Linux;
#endif


namespace TelemetryConsumer.Any
{
    public static class TelemetryConsumerProvider
    {
        public static ISiloHostBuilder AddTelemetryConsumer(this ISiloHostBuilder builder)
        {
#if _WINDOWS
            builder.AddWindowsTelemetryConsumer();
#else
            builder.AddLinuxTelemetryConsumer();
#endif
            return builder;
        }
        
        public static ISiloBuilder AddTelemetryConsumer(this ISiloBuilder builder)
        {
#if _WINDOWS
            builder.AddWindowsTelemetryConsumer();
#else
            builder.AddLinuxTelemetryConsumer();
#endif
            return builder;
        }
    }
}
