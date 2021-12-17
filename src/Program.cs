using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using Serilog;

using Vibe;
using Vibe.Util.AspNet;

IHostBuilder builder = Host.CreateDefaultBuilder();

builder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.UseSerilog();

builder.ConfigureContainer<ContainerBuilder>(builder =>
{
    builder.RegisterType<VibeConfig>();

    builder.RegisterType<VibeRouter>().AsSelf().SingleInstance();

    builder.Register<Serilog.ILogger>(c =>
    {
        var logLevel = c.Resolve<VibeConfig>().LogEventLevel;
        var logCfg = new LoggerConfiguration();
        logCfg.MinimumLevel.Is(logLevel);
        logCfg.WriteTo.Async(c => c.Console(logLevel));
        return logCfg.CreateLogger();
    });

    builder.Register(c => new LoggerFactory().AddSerilog(c.Resolve<Serilog.ILogger>()))
        .As<ILoggerFactory>()
        .SingleInstance();
});

builder.ConfigureWebHost(wh =>
{
    wh.UseKestrel();

    wh.Configure(appb =>
    {
        appb.UseCors(opts => opts.AllowAnyMethod().AllowAnyOrigin().WithHeaders("Content-Type", "Authorization"));
        appb.UseWebSockets(new() { KeepAliveInterval = TimeSpan.FromSeconds(120) });

        var router = appb.ApplicationServices.GetRequiredService<VibeRouter>();
        router.Initialize(appb);

        // this probably shouldn't be here lol
        var config = appb.ApplicationServices.GetRequiredService<VibeConfig>();
        wh.UseUrls(new[] { $"http://${config.ListenHost}" });
    });

    wh.ConfigureServices(c =>
    {
        c.AddCors();
        c.AddRouting();
        c.AddAutofac();
    });
});

var app = builder.Build();

await app.RunAsync();