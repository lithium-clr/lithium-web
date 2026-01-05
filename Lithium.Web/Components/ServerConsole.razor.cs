using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lithium.Web.Components;

public partial class ServerConsole : ComponentBase, IAsyncDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private HubConnection? _hubConnection;
    private readonly List<(DateTimeOffset, int, string)> _logs = [];
    private string _connectionStatus = "Connecting...";
    private string _commandInput = "";

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
            InvokeAsync(() =>
            {
                _logs.Add((timestamp, level, message));
                StateHasChanged();
            });
        });

        _hubConnection.Reconnecting += error =>
        {
            InvokeAsync(() =>
            {
                _connectionStatus = "Connection lost. Reconnecting...";
                StateHasChanged();
            });
            return Task.CompletedTask;
        };

        _hubConnection.Reconnected += connectionId =>
        {
            InvokeAsync(() =>
            {
                _connectionStatus = "Connected.";
                StateHasChanged();
            });
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

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        if (e.Key is "Enter" && !string.IsNullOrWhiteSpace(_commandInput))
        {
            await SendCommand();
        }
    }

    private async Task SendCommand()
    {
        if (_hubConnection is not null && _hubConnection.State == HubConnectionState.Connected)
        {
            try
            {
                await _hubConnection.SendAsync("ExecuteCommand", _commandInput);
            }
            catch (Exception ex)
            {
                _logs.Add((DateTimeOffset.Now, (int)LogLevel.Error, $"Failed to send command: {ex.Message}"));
            }

            _commandInput = "";
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (_hubConnection is not null)
            await _hubConnection.DisposeAsync();
    }
}