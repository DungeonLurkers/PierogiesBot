using System;
using Microsoft.Extensions.DependencyInjection;

namespace PierogiesBot.Manager.Services
{
    public class Factory<T> : IFactory<T>
    {
        private readonly IServiceProvider _provider;

        public Factory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public T? Create()
        {
            return _provider.GetService<T>();
        }
    }
}