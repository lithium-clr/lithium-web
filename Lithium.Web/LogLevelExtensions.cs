namespace Lithium.Web;

public static class LogLevelExtensions
{
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