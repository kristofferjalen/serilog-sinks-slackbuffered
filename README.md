# Serilog.Sinks.SlackBuffered

**Serilog.Sinks.SlackBuffered** is a Serilog sink that can be used to send buffered logs to Slack.

This sink takes the approach of sending as few messages as possible, by grouping them by identical texts, and sending these groups in a condensed format in a single lines.

Messages are not sent immediately, but are added to an observable sequence with identical messages. This buffer is released after a period of calm where no new identical messages have been added.

## Example

Given a list of logs emitted during a period of 20 seconds:
```
Attempted to divide by zero
Attempted to divide by zero
Attempted to divide by zero

System.InvalidOperationException: Argument "age" cannot be negative
```

Will result in two messages being sent to Slack:

```
Received 3 items last 20 s: Attempted to divide by zero

Received 1 item last 20 s: Argument "age" cannot be negative
```

## Installation

1. [Create a Slack incoming webhook](https://api.slack.com/messaging/webhooks)

2. Add package:

```
$ dotnet add package serilog-sinks-slackbuffered
```

3. Add logger in `Program.cs`:

```csharp
var log = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.SlackBufferedSink("https://hooks.slack.com/services/AAA/BBB/CCC",
                new SlackBufferedFormatter(), LogEventLevel.Error)
    .CreateLogger();
```

## Configuration
To avoid messages being sent from multiple handlers, only write logs from `ExceptionHandlerMiddleware`:
```csharp
var log = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Logger(lc =>
        lc.Filter.ByIncludingOnly(le =>
                le.Properties.GetValueOrDefault("SourceContext") is ScalarValue sv &&
                sv.Value?.ToString() == typeof(ExceptionHandlerMiddleware).FullName)
            .WriteTo.SlackBufferedSink(
                "https://hooks.slack.com/services/AAA/BBB/CCC",
                new SlackBufferedFormatter(), LogEventLevel.Error)
    )
    .CreateLogger();
```

## License

[MIT License](https://github.com/kristofferjalen/serilog-sinks-slackbuffered/blob/main/LICENSE)