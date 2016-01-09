using Microsoft.Extensions.DependencyInjection;

namespace Haufwerk.Client
{
    public static class HaufwerkExtensions
    {
        public static void AddHaufwerk(this IServiceCollection services, string haufwerkInstanceUrl)
        {
            services.AddSingleton<IHaufwerk>(provider => new Haufwerk(haufwerkInstanceUrl));
        }
    }
}