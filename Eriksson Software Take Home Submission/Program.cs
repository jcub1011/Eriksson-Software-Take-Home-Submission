using System.Text.RegularExpressions;

namespace Eriksson_Software_Take_Home_Submission
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? logPath = null;
            while (logPath == null)
            {
                Console.WriteLine("Enter path to log file: ");
                logPath = Console.ReadLine();
            }

            var logParser = new LogParser();
            logParser.ParseLog(logPath);

            LogAnalyzer.OutputLogCounts(logParser.LogEntries);

            string? eventType = null;
            while (eventType == null)
            {
                Console.WriteLine($"Enter event type to get top 3 most frequent associated message phrases: ");
                eventType = Console.ReadLine();
            }
            LogAnalyzer.OutputMostCommonMessagePhrases(logParser.LogEntries, eventType);
        }
    }

    public readonly struct LogEntry(DateTime timestamp, string eventType, string message)
    {
        public readonly DateTime Timestamp = timestamp;
        public readonly string EventType = eventType.ToUpper(); // Keeps event types consistent regardless of capitalization.
        public readonly string Message = message;

        public override string ToString()
        {
            return $"[{Timestamp}] [{EventType}] [{Message}]";
        }
    }

    public class LogParser
    {
        #region Properties
        readonly List<LogEntry> _logEntries = new();
        public IReadOnlyList<LogEntry> LogEntries => _logEntries.AsReadOnly();
        #endregion

        /// <summary>
        /// Parses the log data. Log data should be formatted as one log per line, where each log has three entries: [TIMESTAMP] [EVENT_TYPE] [MESSAGE]
        /// </summary>
        /// <param name="logData"></param>
        /// <exception cref="LogEntryCountException">Thrown when a line in the log data does not have the required amount of entries. [TIMESTAMP] [EVENT_TYPE] [MESSAGE].</exception>
        /// <exception cref="LogTimestampParseException">Thrown when a log entry has a timestamp that cannot be parsed.</exception>
        public void ParseLog(string logFile)
        {
            using StreamReader reader = new(logFile.Trim('"'));
            _logEntries.Clear();

            string? line = reader.ReadLine();
            while (line != null)
            {
                // This regex allows any combination of characters encased in square brackets to be extracted.
                var timestampMatch = Regex.Match(line, "\\[[^\\[\\]]*\\]");
                if (!timestampMatch.Success)
                {
                    throw new LogTimestampParseException($"Line {_logEntries.Count + 1} in log data has a timestamp that cannot be parsed.");
                }

                if (!DateTime.TryParse(timestampMatch.Value.Trim('[', ']'), out var timestamp))
                {
                    // Assuming exception throwing is preferred over skipping or logging errors. Can be easily changed.
                    throw new LogTimestampParseException($"Line {_logEntries.Count + 1} in log data has a timestamp that cannot be parsed.");
                }

                var entries = line[(timestampMatch.Length + 1)..].Split(' ', 2);

                // Verify entry count.
                if (entries.Length != 2)
                {
                    // Assuming exception throwing is preferred over skipping or logging errors. Can be easily changed.
                    throw new LogEntryCountException($"Line {_logEntries.Count} in log data does not have three entries formatted as [TIMESTAMP] [EVENT_TYPE] [MESSAGE].");
                }

                _logEntries.Add(new LogEntry(timestamp, entries[0], entries[1].Trim()));

                line = reader.ReadLine();
            }
        }
    }

    public static class LogAnalyzer
    {
        public static void OutputLogCounts(IReadOnlyList<LogEntry> logEntries)
        {
            Dictionary<string, int> eventTypeCounts = new();
            foreach (var entry in logEntries)
            {
                if (eventTypeCounts.TryGetValue(entry.EventType, out int value))
                {
                    eventTypeCounts[entry.EventType] = ++value;
                }
                else
                {
                    eventTypeCounts[entry.EventType] = 1;
                }
            }

            Console.WriteLine($"---- Event_Type Occurances ----");
            foreach ((var type, var count) in eventTypeCounts)
            {
                Console.WriteLine($"{type}: {count}");
            }
        }

        public static void OutputMostCommonMessagePhrases(IReadOnlyList<LogEntry> logEntries, string eventType)
        {
            // I'm assuming that the message phrase is the entire message.
            Dictionary<string, int> messageCounts = new();

            foreach (var log in logEntries.Where(x => x.EventType.Equals(eventType, StringComparison.InvariantCultureIgnoreCase)))
            {
                if (messageCounts.TryGetValue(log.Message, out int value))
                {
                    messageCounts[log.Message] = ++value;
                }
                else
                {
                    messageCounts[log.Message] = 1;
                }
            }

            Console.WriteLine($"---- Most Frequent Message Phrase For {eventType.ToUpper()} Events ----");
            foreach ((var message, var count) in messageCounts.OrderByDescending(x => x.Value).Take(3))
            {
                Console.WriteLine($"{count} occurance(s) of: \"{message}\"");
            }
        }
    }

    #region Custom Exceptions
    [Serializable]
    public class LogEntryCountException : Exception
    {
        public LogEntryCountException()
        { }

        public LogEntryCountException(string message)
            : base(message)
        { }

        public LogEntryCountException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }

    [Serializable]
    public class LogTimestampParseException : Exception
    {
        public LogTimestampParseException()
        { }

        public LogTimestampParseException(string message)
            : base(message)
        { }

        public LogTimestampParseException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
    #endregion
}
