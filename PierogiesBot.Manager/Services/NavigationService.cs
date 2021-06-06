using Autofac;
using ReactiveUI;

namespace PierogiesBot.Manager.Services
{
    public class NavigationService : INavigationService
    {
        private readonly ILifetimeScope _container;

        public NavigationService(ILifetimeScope container)
        {
            _container = container;
        }

        public RoutingState Router { get; } = new();

        public void NavigateTo<T>() where T : IRoutableViewModel
        {
            var next = _container.Resolve<T>();
            Router.Navigate.Execute(next);
        }

        public void NavigateToAndReset<T>() where T : IRoutableViewModel
        {
            var next = _container.Resolve<T>();
            Router.NavigateAndReset.Execute(next);
        }

        public void NavigateBack()
        {
            Router.NavigateBack.Execute();
        }
    }
}