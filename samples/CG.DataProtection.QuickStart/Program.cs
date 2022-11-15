
try
{
    BootstrapLogger.Instance()
        .LogInformation("BootstrapLogger can log before there is a host!");

    var builder = new HostBuilder().ConfigureLogging(options =>
    {
        options.SetMinimumLevel(LogLevel.Information);
        options.AddSimpleConsole();
    });

    var host = builder.Build();

    host.RunDelegate((host, token) =>
    {
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("This is normal .NET logging inside the host.");
    });
}
finally
{
    BootstrapLogger.Instance()
        .LogInformation("BootstrapLogger can log after the host is gone!");
}

