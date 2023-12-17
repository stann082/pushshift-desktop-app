﻿using lib;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace app;

public static class Program
{
    
    #region Main Method

    public static async Task<int> Main(string[] args)
    {
        var multiplexer = await ConnectionMultiplexer.ConnectAsync("localhost");
        var config = ApplicationConfig.Load();
        var services = new ServiceCollection()
        .AddSingleton<App>()
        .AddSingleton<IConnectionMultiplexer>(multiplexer)
        .AddSingleton(config)
        .AddScoped<IRedditService>()
        .BuildServiceProvider();
        return await services.GetService<App>().RunApp(args);
    }

    #endregion

}
