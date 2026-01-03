using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Lithium.Web.Components;

public partial class DashboardNavLink : ComponentBase
{
    [Parameter] public string Href { get; set; } = string.Empty;
    [Parameter] public string Icon { get; set; } = string.Empty;
    [Parameter] public string Text { get; set; } = string.Empty;

    [Parameter] public NavLinkMatch Match { get; set; } = NavLinkMatch.All;
}