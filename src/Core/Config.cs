using Serilog.Events;

namespace Vibe;

public class VibeConfig
{
    public string ListenHost { get; set; } = "127.0.0.1:5000";

    public Deployment Deployment { get; set; } = Deployment.SingleProcess;

    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Debug;
}

public enum Deployment
{
    SingleProcess,
    Requests,
    Streams,
    REST
}