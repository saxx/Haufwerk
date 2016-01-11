using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace Haufwerk.Client
{
    public class HaufwerkOptions
    {
        public HaufwerkOptions([NotNull] string source, [NotNull]Uri uri)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            InstanceUri = uri;
            Source = source;
            LogLevelsToLog = new[] { LogLevel.Error, LogLevel.Critical };
        }

        public HaufwerkOptions([NotNull] string source, [NotNull]string uri) : this(source, new Uri(uri))
        {
        }

        [NotNull]
        public Uri InstanceUri { get; set; }
        public bool LogLocalRequests { get; set; }
        [NotNull]
        public LogLevel[] LogLevelsToLog { get; set; }
        [NotNull]
        public string Source { get; set; }
    }
}
