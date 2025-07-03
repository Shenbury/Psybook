using Microsoft.Extensions.DependencyInjection;

namespace Psybook.Shared.Extensions
{
    public static class SharedServicesExtensions
    {
        public static IServiceCollection ClientAndServerRegistrations(this IServiceCollection serviceCollection)
        {

            return serviceCollection;
        }
    }
}
