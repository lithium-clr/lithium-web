using Lithium.Snowflake;
using Lithium.Snowflake.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Lithium.Web.Components.Layout;

public partial class NavMenu : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IIdGenerator IdGenerator { get; set; } = null!;

    private List<Models.Server> _servers = [];
    private SnowflakeId _activeServerId;
    private string _activeNavigation = "overview";
    
    private Models.Server? ActiveServer => _servers.FirstOrDefault(s => s.Id == _activeServerId);

    protected override void OnInitialized()
    {
        // Simulate fetching servers from a service

        long id1 = IdGenerator.CreateId();
        long id2 = IdGenerator.CreateId();
        long id3 = IdGenerator.CreateId();
        
        Console.WriteLine("Id1: " + id1);
        Console.WriteLine("Id2: " + id2);
        Console.WriteLine("Id3: " + id3);
        
        _servers =
        [
            new Models.Server { Id = new SnowflakeId(1384530019614720), Name = "Main" },
            new Models.Server { Id = new SnowflakeId(1384530019614721), Name = "Dev" },
            new Models.Server { Id = new SnowflakeId(1384530019614722), Name = "Test" }
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

        var hasServerId = long.TryParse(segments[1], out var serverId);
        var hasNavigation = segments.Length > 2;
        
        if (segments.Length > 1 && segments[0] is "servers" && hasServerId)
        {
            _activeServerId = new SnowflakeId(serverId);
            _activeNavigation = hasNavigation ? segments[2] : "overview";
        }
        else if (_servers.Count is not 0)
        {
            // Fallback to the first server if the URL doesn't match
            _activeServerId = _servers.First().Id;
            _activeNavigation = "overview";
            
            NavigationManager.NavigateTo($"/servers/{_activeServerId}/overview");
        }
    }
    
    private void OnNavigationChanged()
    {
        UpdateActiveServer(NavigationManager.Uri);
    }

    void IDisposable.Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}