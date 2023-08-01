namespace foodremedy.api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void UseLowercaseUrls(this IServiceCollection serviceCollection)
    {
        serviceCollection.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
    }
}
