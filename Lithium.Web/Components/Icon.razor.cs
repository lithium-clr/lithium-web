using Microsoft.AspNetCore.Components;

namespace Lithium.Web.Components;

public partial class Icon : ComponentBase
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool Filled { get; set; }
}