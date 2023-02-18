namespace TrackerEmulator;

public class ClientParams
{
    public string? ScriptPath { get; set; }
    public string? InputPath { get; set; }
    public string? OutputPath { get; set; }
    public int RepeatCount { get; set; } = 1;
}