using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using PierogiesBot.Grains;
using TelemetryConsumer.Any;

namespace PierogiesBot.Silo
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder UsePierogiesOrleans(this IHostBuilder b) => b.UseOrleans(builder =>
        {
            builder.UseLocalhostClustering()
                .AddGrainService<DiscordCommandsGrainService>()
                .AddGrainService<DiscordMessageHandlerGrainService>()
                .AddGrainService<DiscordSubscriptionsGrainService>()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "OrleansBasics";
                })
                .ConfigureApplicationParts(parts => parts.AddFromApplicationBaseDirectory().WithReferences())
                .ConfigureServices((ctx, sp) =>
                {
                    var config = ctx.Configuration;
                    sp.AddMongoDBClient(config["MongoDBOption:ConnectionString"]);
                    sp.AddMongoDBGrainStorageAsDefault(builder => builder.BindConfiguration("OrleansMongoDBOptions"));
                    sp.AddMongoDBGrainStorage("PubSubStore", builder => builder.BindConfiguration("OrleansMongoDBOptions"));

                })
                .UseDashboard(options =>
                {
                    options.Username = "USERNAME";
                    options.Password = "PASSWORD";
                    options.Host = "*";
                    options.Port = 8080;
                    options.HostSelf = true;
                    options.CounterUpdateIntervalMs = 5000;
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .AddTelemetryConsumer()
                // .AddSimpleMessageStreamProvider(StreamProviders.MessageStreamProvider)
                ;
        });
    }
}