using Autofac;
using PierogiesBot.Data.Services;
using PierogiesBot.Discord.Settings;

namespace PierogiesBot
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly, typeof(IRepository<>).Assembly, typeof(DiscordSettings).Assembly)
                .Where(x => x.Name.EndsWith("Service", System.StringComparison.OrdinalIgnoreCase))
                .AsImplementedInterfaces()
                .AsSelf()
                .SingleInstance();

            base.Load(builder);
        }
    }
}