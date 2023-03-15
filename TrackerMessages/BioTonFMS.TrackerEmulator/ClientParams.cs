namespace BioTonFMS.TrackerEmulator;

public class ClientParams
{
    public string? ScriptPath { get; set; }
    public string? MessagePath { get; set; }
    public string? ResultPath { get; set; }
    public int RepeatCount { get; set; } = 1;
}