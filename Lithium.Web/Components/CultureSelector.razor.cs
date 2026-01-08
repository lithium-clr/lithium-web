using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace Lithium.Web.Components;

public partial class CultureSelector : ComponentBase
{
    protected override void OnInitialized()
    {
        Culture = CultureInfo.CurrentCulture;
    }

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
}