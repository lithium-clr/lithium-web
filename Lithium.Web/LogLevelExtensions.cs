namespace Lithium.Web;

public static class LogLevelExtensions
{
    public static string ToColor(this LogLevel level) => level switch
    {
        LogLevel.Debug or LogLevel.Trace => "#71717a",
        LogLevel.Information => "#FFFFFF",
        LogLevel.Warning => "#FF9900",
        LogLevel.Error or LogLevel.Critical => "#FF0000",
        _ => ""
    };

    public static string ToShortName(this LogLevel level) => level switch
    {
        LogLevel.Debug => "DBG",
        LogLevel.Trace => "TRA",
        LogLevel.Information => "INF",
        LogLevel.Warning => "WRN",
        LogLevel.Error or LogLevel.Critical => "ERR",
        _ => ""
    };
}