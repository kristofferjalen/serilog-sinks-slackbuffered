using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.SlackBuffered;

public class SlackBufferedFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        var baseException = logEvent.Exception?.GetBaseException().Message;

        var host = Unwrap(logEvent, "Host");
        var requestPath = Unwrap(logEvent, "RequestPath");
        var queryString = Unwrap(logEvent, "QueryString");
        
        if (!string.IsNullOrEmpty(baseException))
        {
            output.Write(baseException);
        }

        var meta = string.Join(", ", host, $"{requestPath}{queryString}");

        if (!string.IsNullOrEmpty(meta))
        {
            output.Write($" ({meta})");
        }
        
        output.WriteLine();
    }

    private static string Unwrap(LogEvent logEvent, string property)
    {
        var unwrapped = "";

        if (logEvent.Properties.TryGetValue(property, out var value) &&
            value is ScalarValue { Value: string rawValue })
        {
            unwrapped = rawValue;
        }

        return unwrapped;
    }
}