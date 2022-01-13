using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.SlackBuffered;

public class SlackBufferedFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        var baseException = logEvent.Exception?.GetBaseException().Message;
        var requestId = logEvent.Properties.GetValueOrDefault("RequestId");
        var requestPath = logEvent.Properties.GetValueOrDefault("RequestPath");
        var host = logEvent.Properties.GetValueOrDefault("Host");

        if (!string.IsNullOrEmpty(baseException))
        {
            output.Write(baseException);
        }

        var meta = string.Join(", ", host, requestPath, requestId);

        if (!string.IsNullOrEmpty(meta))
        {
            output.Write($" ({meta})");
        }
        
        output.WriteLine();
    }
}