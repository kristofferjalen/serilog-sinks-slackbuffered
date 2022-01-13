using System.Reactive.Linq;

namespace Serilog.Sinks.SlackBuffered;

internal static class SlackWebhookExtensions
{
    /// <summary>
    ///     Group observable sequence into buffers separated by periods of calm.
    /// </summary>
    /// <param name="source">Observable to buffer.</param>
    /// <param name="calmDuration">Duration of calm after which to close buffer.</param>
    /// <param name="maxCount">Max size to buffer before returning.</param>
    /// <param name="maxDuration">Max duration to buffer before returning.</param>
    public static IObservable<IList<T>> BufferUntilCalm<T>(this IObservable<T> source, TimeSpan calmDuration,
        int? maxCount = null, TimeSpan? maxDuration = null)
    {
        ArgumentNullException.ThrowIfNull(source);

        var closes = source.Throttle(calmDuration);

        if (maxCount != null)
        {
            var overflows = source.Where((_, index) => index + 1 >= maxCount);

            closes = closes.Amb(overflows);
        }

        if (maxDuration == null)
        {
            return source.Window(() => closes).SelectMany(window => window.ToList());
        }

        var ages = source.Delay(maxDuration.Value);

        closes = closes.Amb(ages);

        return source.Window(() => closes).SelectMany(window => window.ToList());
    }

    /// <summary>
    ///     Groups messages by text.
    /// </summary>
    /// <param name="collection">A collection of messages.</param>
    /// <returns></returns>
    public static IEnumerable<(string webhookUrl, string message, int count)> ByText(
        this IEnumerable<(string webhookUrl, IEnumerable<SlackWebhookMessage> messages)> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        return collection.SelectMany(x =>
            x.messages.GroupBy(m => m.Text, m => m.Text, (key, g) => (x.webhookUrl, key, g.Count())));
    }

    /// <summary>
    ///     Groups messages by Slack Webhook URL.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection">A collection of messages.</param>
    /// <returns></returns>
    public static IEnumerable<(string webhookUrl, IEnumerable<T> messages)> ByWebhookUrl<T>(
        this IEnumerable<(string @string, T @object)> collection)
    {
        ArgumentNullException.ThrowIfNull(collection);

        return collection.GroupBy(m => m.@string, m => m.@object, (key, g) => (key, g));
    }
}