using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.SlackBuffered;

internal class SlackBufferedSink : ILogEventSink
{
    private readonly SlackWebhookService _slackWebhookService;
    private readonly ITextFormatter _textFormatter;
    private readonly string _webhookUrl;

    public SlackBufferedSink(string webhookUrl, ITextFormatter textFormatter)
    {
        ArgumentNullException.ThrowIfNull(webhookUrl);
        ArgumentNullException.ThrowIfNull(textFormatter);

        _webhookUrl = webhookUrl;
        _textFormatter = textFormatter;
        _slackWebhookService = new SlackWebhookService();
    }

    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        var payload = new StringWriter();

        _textFormatter.Format(logEvent, payload);

        var text = payload.ToString();

        var slackWebhookMessage = new SlackWebhookMessage(text);

        _slackWebhookService.BufferMessage(_webhookUrl, slackWebhookMessage);
    }
}