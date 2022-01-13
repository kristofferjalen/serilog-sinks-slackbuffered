using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;

namespace Serilog.Sinks.SlackBuffered;

internal class SlackWebhookService
{
    private const int MaxCount = 100;
    private readonly TimeSpan _calmDuration = TimeSpan.FromSeconds(20);
    private readonly HttpClient _client;
    private readonly Subject<(string, SlackWebhookMessage)> _messages;

    public SlackWebhookService()
    {
        _client = new HttpClient();
        _messages = new Subject<(string, SlackWebhookMessage)>();
        _messages
            .BufferUntilCalm(_calmDuration, MaxCount, _calmDuration)
            .Subscribe(ProcessBuffer);
    }

    public void BufferMessage(string webhookUrl, SlackWebhookMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(webhookUrl);

        if (string.IsNullOrWhiteSpace(webhookUrl))
        {
            throw new ArgumentException("Argument cannot be null or whitespace only.", nameof(webhookUrl));
        }

        _messages.OnNext((webhookUrl, message));
    }

    private void ProcessBuffer(IList<(string webhookUrl, SlackWebhookMessage message)> buffer) => Task.WhenAll(
        buffer.ByWebhookUrl().ByText().Select(x => SendMessageToSlack(x.webhookUrl, x.count, x.message)));

    private Task<HttpResponseMessage> SendMessageToSlack(string webhookUrl, int count, string text)
    {
        var requestUri = new Uri(webhookUrl);

        var items = count > 1 ? "items" : "item";

        var message =
            $"Received {count} {items} last {_calmDuration.TotalSeconds} s: {text}";

        var payload = new { text = message };

        var json = JsonSerializer.Serialize(payload);

        var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

        var task = _client.PostAsync(requestUri, stringContent);

        return task;
    }
}