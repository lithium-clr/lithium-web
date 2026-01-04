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
            new Models.Server { Id = Guid.Parse("891deb1e-9362-475a-a34f-2a7e61a68624"), Name = "Main" },
            new Models.Server { Id = Guid.Parse("007c9312-fe68-4c63-9f98-0e6b6c63b605"), Name = "Dev" },
            new Models.Server { Id = Guid.Parse("06fb2bdf-d2d8-47bb-8246-9b745b9bcd05"), Name = "Test" }
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

        if (segments.Length > 1 && segments[0] is "servers" && Guid.TryParse(segments[1], out var serverId))
        {
            _activeServerId = serverId;
        }
        else if (_servers.Count is not 0)
        {
            // Fallback to the first server if the URL doesn't match
            _activeServerId = _servers.First().Id;
            NavigationManager.NavigateTo($"/servers/{_activeServerId}/overview");
        }
    }

    void IDisposable.Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}