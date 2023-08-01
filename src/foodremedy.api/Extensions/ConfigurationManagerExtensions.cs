namespace foodremedy.api.Extensions;

public static class ConfigurationManagerExtensions
{
    public static void AddJsonFiles(this ConfigurationManager configurationManager)
    {
        configurationManager.AddJsonFile("appsettings.json");
        
        configurationManager.AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");
    }
}
