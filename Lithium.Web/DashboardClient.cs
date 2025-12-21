using Lithium.Server.Dashboard;
using Microsoft.AspNetCore.SignalR.Client;

namespace Lithium.Web;

public sealed class DashboardClient
{
    private readonly HubConnection _connection;

    public event Action? Heartbeat;

    public DashboardClient(string url)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(url)
            .WithAutomaticReconnect()
            .Build();

        RegisterHandlers();
    }

    private void RegisterHandlers()
    {
        _connection.On(nameof(IServerHub.Heartbeat), () =>
        {
            Heartbeat?.Invoke();
        });
    }

    public async Task ConnectAsync()
    {
        await _connection.StartAsync();
    }

    public async Task DisconnectAsync()
    {
        await _connection.StopAsync();
        await _connection.DisposeAsync();
    }
}