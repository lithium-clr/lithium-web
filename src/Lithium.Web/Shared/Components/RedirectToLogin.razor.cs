using Microsoft.AspNetCore.Components;

namespace Lithium.Web.Shared.Components;

public partial class RedirectToLogin : ComponentBase
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    
    protected override void OnInitialized()
    {
        NavigationManager.NavigateTo($"auth/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}", forceLoad: true);
    }
}