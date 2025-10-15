namespace WorkersService.Options;

public class WorkerProgressNotifierOptions
{
    public string GatewayUrl { get; set; } = string.Empty;
    
    public WorkerProgressNotifierOptions() { }
    
    public WorkerProgressNotifierOptions(string gatewayUrl)
    {
        GatewayUrl = gatewayUrl;
    }
}