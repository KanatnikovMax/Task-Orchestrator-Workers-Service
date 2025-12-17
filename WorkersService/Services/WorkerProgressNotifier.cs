using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using WorkersService.Options;

namespace WorkersService.Services;

public class WorkerProgressNotifier : IAsyncDisposable
{
    private readonly ILogger<WorkerProgressNotifier> _logger;
    private readonly HubConnection _hubConnection;
    private bool _started;

    public WorkerProgressNotifier(
        IOptions<WorkerProgressNotifierOptions> options, 
        ILogger<WorkerProgressNotifier> logger)
    {
        _logger = logger;
        var opts = options.Value;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl($"{opts.GatewayUrl}/hubs/task-progress")
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartAsync()
    {
        if (_started) return;

        try
        {
            await _hubConnection.StartAsync();
            _logger.LogInformation("Connected to Gateway SignalR hub");
            _started = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to Gateway SignalR hub");
        }
    }

    public async Task StopAsync()
    {
        if (!_started) return;

        try
        {
            await _hubConnection.StopAsync();
            _logger.LogInformation("Disconnected from Gateway SignalR hub");
            _started = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to disconnect from Gateway SignalR hub");
        }
    }

    public async Task NotifyProgressAsync(string taskId, int progress)
    {
        try
        {
            if (_hubConnection.State == HubConnectionState.Connected)
            {
                await _hubConnection.InvokeAsync("UpdateWorkerTaskProgress", taskId, progress);
                _logger.LogDebug("Progress updated for task {TaskId}: {Progress}%", taskId, progress);
            }
            else
            {
                _logger.LogWarning("Cannot send progress update - SignalR connection is not active");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send progress update for task {TaskId}", taskId);
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_started)
            {
                await StopAsync();
            }

            await _hubConnection.DisposeAsync();
            _logger.LogInformation("WorkerProgressNotifier disposed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing WorkerProgressNotifier");
        }
    }
}
