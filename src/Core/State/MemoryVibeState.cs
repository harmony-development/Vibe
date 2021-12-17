namespace Vibe.State;

public class MemoryVibeState : IVibeState
{
    public EventHandler<(string, object)>? AuthStreamQueue { get; }
    public EventHandler<(string, object)>? ChatStreamQueue { get; }
    public EventHandler<(string, object)>? VoiceStreamQueue { get; }
}