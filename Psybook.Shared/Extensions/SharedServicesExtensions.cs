using Microsoft.Extensions.DependencyInjection;
using Psybook.Objects.Dictionary;

namespace Psybook.Shared.Extensions
{
    public static class SharedServicesExtensions
    {
        public static IServiceCollection ClientAndServerRegistrations(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<ExperienceDictionary>();
            return serviceCollection;
        }
    }
}
