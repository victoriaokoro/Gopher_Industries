﻿using foodremedy.api.Configuration;
using foodremedy.api.Providers;
using foodremedy.api.Repositories;
using foodremedy.api.Utils;
using Microsoft.IdentityModel.Tokens;

namespace foodremedy.api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void UseLowercaseUrls(this IServiceCollection serviceCollection)
    {
        serviceCollection.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
    }

    public static void AddInternalServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
        serviceCollection.AddTransient<IAuthenticationProvider, AuthenticationProvider>();
    }

    public static void AddJwtAuthentication(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection
            .AddOptions<AuthenticationConfiguration>()
            .Bind(configuration.GetSection(AuthenticationConfiguration.ConfigurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        var authenticationConfiguration = configuration
            .GetSection(AuthenticationConfiguration.ConfigurationSection)
            .Get<AuthenticationConfiguration>();
        
        serviceCollection.AddAuthentication().AddJwtBearer(conf =>
        {
            conf.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = authenticationConfiguration!.Audience,
                ValidIssuer = authenticationConfiguration.Issuer,
                IssuerSigningKey = SigningKeyFactory.Get(authenticationConfiguration.SigningKey)
            };
        });
    }
}
