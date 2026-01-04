using Lithium.Web.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Lithium.Web.Components.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    private List<Models.Server> _servers = [];
    private Guid _activeServerId;
    
    private Models.Server? ActiveServer => _servers.FirstOrDefault(s => s.Id == _activeServerId);

    protected override void OnInitialized()
    {
        // Simulate fetching servers from a service
        _servers =
        [
            new Models.Server { Id = Guid.NewGuid(), Name = "Main" },
            new Models.Server { Id = Guid.NewGuid(), Name = "Dev" },
            new Models.Server { Id = Guid.NewGuid(), Name = "Test" }
        ];

        NavigationManager.LocationChanged += OnLocationChanged;
        UpdateActiveServer(NavigationManager.Uri);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        UpdateActiveServer(e.Location);
        StateHasChanged();
    }

    private void UpdateActiveServer(string url)
    {
        var uri = new Uri(url);
        var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 1 && segments[0] == "servers" && Guid.TryParse(segments[1], out var serverId))
        {
            _activeServerId = serverId;
        }
        else if (_servers.Any())
        {
            // Fallback to the first server if the URL doesn't match
            _activeServerId = _servers.First().Id;
            NavigationManager.NavigateTo($"/servers/{_activeServerId}/overview");
        }
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}