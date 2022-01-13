using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Serilog.Sinks.SlackBuffered;

public static class SlackBufferedSinkExtensions
{
    private const string DefaultOutputTemplate =
        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}";

    public static LoggerConfiguration SlackBufferedSink(
        this LoggerSinkConfiguration loggerConfiguration,
        string webhookUrl,
        string outputTemplate = DefaultOutputTemplate,
        IFormatProvider? formatProvider = null,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
    {
        ArgumentNullException.ThrowIfNull(webhookUrl);

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Argument cannot be null or whitespace only.", nameof(webhookUrl));
        }

        var formatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);

        var slackBufferedSink = new SlackBufferedSink(webhookUrl, formatter);

        return loggerConfiguration.Sink(slackBufferedSink, restrictedToMinimumLevel);
    }

    public static LoggerConfiguration SlackBufferedSink(
        this LoggerSinkConfiguration loggerConfiguration,
        string webhookUrl,
        ITextFormatter formatter,
        LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum)
    {
        ArgumentNullException.ThrowIfNull(webhookUrl);
        ArgumentNullException.ThrowIfNull(formatter);

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Argument cannot be null or whitespace only.", nameof(webhookUrl));
        }

        var slackBufferedSink = new SlackBufferedSink(webhookUrl, formatter);

        return loggerConfiguration.Sink(slackBufferedSink, restrictedToMinimumLevel);
    }
}