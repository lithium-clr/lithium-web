using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lithium.Web.Components;

public partial class ServerConsole : ComponentBase
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private HubConnection? _hubConnection;
    private readonly List<(DateTimeOffset, int, string)> _logs = [];
    private string _connectionStatus = "Connecting...";

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7144/hub/console")
            .WithAutomaticReconnect()
            .Build();

        _logs.Add((DateTime.Now, 0, "My \"apple\" is big"));
        _logs.Add((DateTime.Now, 1, "Click on https://youtube.com/ to open the link"));
        
        _hubConnection.On<DateTimeOffset, int, string>("ReceiveLog", (timestamp, level, message) =>
        {
            // var date = DateTimeOffset.Now - timestamp;
            _logs.Add((timestamp, level, message));
            InvokeAsync(StateHasChanged);
        });

        _hubConnection.Reconnecting += error =>
        {
            _connectionStatus = "Connection lost. Reconnecting...";
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            _connectionStatus = "Connected.";
            InvokeAsync(StateHasChanged);
            return Task.CompletedTask;
        };

        try
        {
            await _hubConnection.StartAsync();
            _connectionStatus = "Connected.";
        }
        catch (Exception ex)
        {
            _connectionStatus = $"Connection failed: {ex.Message}";
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }
}