using Microsoft.Extensions.DependencyInjection;

namespace foodremedy.database.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDatabase(this IServiceCollection collection)
    {
        collection.AddDbContext<FoodRemedyDbContext>();
    }
}
