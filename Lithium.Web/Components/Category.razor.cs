using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Lithium.Web.Components;

public partial class Category : ComponentBase
{
    [Parameter] public string Text { get; set; } = string.Empty;
    [Parameter] public bool Collapsed { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;

    private void OnClick()
    {
        Collapsed = !Collapsed;
    }
}