namespace Serilog.Sinks.SlackBuffered;

internal class SlackWebhookMessage
{
    public SlackWebhookMessage(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Argument cannot be null or whitespace only.", nameof(text));
        }

        Text = text;
    }

    public string Text { get; }
}