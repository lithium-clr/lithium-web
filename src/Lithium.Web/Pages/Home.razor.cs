using Microsoft.AspNetCore.Components;

namespace Lithium.Web.Pages;

public partial class Home : ComponentBase
{
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;

    private void Login()
    {
        NavigationManager.NavigateTo($"auth/login?returnUrl={Uri.EscapeDataString(NavigationManager.Uri)}",
            forceLoad: true);
    }

    private void Logout()
    {
        NavigationManager.NavigateTo("auth/logout", forceLoad: true);
    }
}