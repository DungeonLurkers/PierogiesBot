using System.Linq;
using Autofac;
using PierogiesBot.Manager.Services;
using ReactiveUI;
using Splat;
using Splat.Autofac;

namespace PierogiesBot.Manager
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                .Where(t => t.GetInterfaces()
                    .Any(
                        i => i.IsGenericType
                             && i.GetGenericTypeDefinition() == typeof(IViewFor<>)))
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                .Where(t => t.Name.Contains("ViewModel") && t.GetInterfaces().All(x => x != typeof(IScreen)))
                .AsSelf()
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(Startup).Assembly)
                .Where(t => t.GetInterfaces().Any(x => x == typeof(IScreen)))
                .AsSelf()
                .As<IScreen>()
                .SingleInstance();

            builder.RegisterType<PierogiesBotService>().AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof(Factory<>)).AsImplementedInterfaces();
            builder.RegisterType<SettingsService>().AsImplementedInterfaces();
            builder.RegisterType<NavigationService>().AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<AutofacViewLocator>().As<IViewLocator>();

            base.Load(builder);

            builder.UseAutofacDependencyResolver();
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();

            Locator.CurrentMutable.RegisterLazySingleton(() => new AutofacViewLocator(), typeof(IViewLocator));
        }
    }
}