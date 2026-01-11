namespace Lithium.Web;

public static partial class Bootstrap
{
    private static IServiceCollection SetupLocalization(this IServiceCollection services)
    {
        return services.AddLocalization(options => options.ResourcesPath = "Resources");
    }

    private static IApplicationBuilder SetupLanguages(this IApplicationBuilder app)
    {
        var supportedCultures = new[]
        {
            "fr-FR",
            "af-ZA",
            "ar-SA",
            "ca-ES",
            "zh-CN",
            "zh-TW",
            "cs-CZ",
            "da-DK",
            "nl-NL",
            "en-US",
            "fi-FI",
            "de-DE",
            "el-GR",
            "he-IL",
            "hu-HU",
            "it-IT",
            "ja-JP",
            "ko-KR",
            "no-NO",
            "pl-PL",
            "pt-PT",
            "pt-BR",
            "ro-RO",
            "ru-RU",
            "es-ES",
            "sv-SE",
            "tr-TR",
            "uk-UA",
            "vi-VN"
        };

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);
        return app;
    }
}