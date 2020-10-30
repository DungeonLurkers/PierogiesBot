// ReSharper disable InconsistentNaming

using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Module.Discord.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddDiscordClient(this IServiceCollection services)
        {

            services.AddSingleton<IDiscordClient>(provider =>
            {
                var logger = provider.GetService<ILoggerFactory>().CreateLogger(nameof(DiscordSocketClient));

                var client = new DiscordSocketClient
                (
                    new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose
                    }
                );
                client.Log += message =>  ClientOnLog(message, logger);

                return client;
            });
        }

        private static Task ClientOnLog(LogMessage message, ILogger logger)
        {
            var logLevel = message.Severity switch
            {
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.None
            };

            if (message.Exception != null)
            {
                logger.LogError(message.Exception, message.Message);
            }
            else
            {
                logger.Log(logLevel, message.Message);
            }

            return Task.CompletedTask;
        }
    }
}