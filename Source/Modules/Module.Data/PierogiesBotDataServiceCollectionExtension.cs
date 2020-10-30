using Microsoft.Extensions.DependencyInjection;
using Module.Data.Storage;

namespace Module.Data
{
    public static class PierogiesBotDataServiceCollectionExtension
    {
        public static void AddPierogiesBotData(this IServiceCollection services)
        {
            AddServices(services);
        }

        private static void AddServices(IServiceCollection services)
        {
        }
    }
}