using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace PierogiesBot.Manager
{
    public class AutofacViewLocator : IViewLocator
    {
        public IViewFor? ResolveView<T>(T viewModel, string? contract = null)
        {
            var iViewForType = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());

            return (IViewFor?) App.Container.GetService(iViewForType);
        }
    }
}