using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Lithium.Web.Shared.Components;

public partial class CultureSelector : ComponentBase
{
    private bool _isDropdownOpen;

    private CultureInfo Culture
    {
        get => CultureInfo.CurrentCulture;
        set
        {
            if (Equals(CultureInfo.CurrentCulture, value)) return;

            var uri = new Uri(Navigation.Uri)
                .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
            var cultureEscaped = Uri.EscapeDataString(value.Name);
            var uriEscaped = Uri.EscapeDataString(uri);
            var fullUri = $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}";

            Navigation.NavigateTo(fullUri, forceLoad: true);
        }
    }

    protected override void OnInitialized()
    {
        Culture = CultureInfo.CurrentCulture;
    }

    private void ToggleDropdown()
    {
        _isDropdownOpen = !_isDropdownOpen;
    }

    private void ChangeCulture(string culture)
    {
        var uri = new Uri(Navigation.Uri)
            .GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var cultureEscaped = Uri.EscapeDataString(culture);
        var uriEscaped = Uri.EscapeDataString(uri);

        var fullUri = $"Culture/Set?culture={cultureEscaped}&redirectUri={uriEscaped}";
        Navigation.NavigateTo(fullUri, forceLoad: true);
    }
}