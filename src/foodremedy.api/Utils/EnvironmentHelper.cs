namespace foodremedy.api.Utils;

internal static class EnvironmentHelper
{
    public static string GetEnvironment()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
               ?? "LocalDevelopment";
    }

    public static bool IsDevelopment()
    {
        return GetEnvironment() == "Development"
               || GetEnvironment() == "LocalDevelopment";
    }
}