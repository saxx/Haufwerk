using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;

namespace Haufwerk.Client
{
    public class HaufwerkErrorLogger : ILogger
    {
        #region Constructor
        [NotNull]
        private readonly IHaufwerk _haufwerk;
        [CanBeNull]
        private readonly IHttpContextAccessor _httpContextAccessor;
        [CanBeNull]
        private readonly string _categoryName;

        public HaufwerkErrorLogger(
            [NotNull] IHaufwerk haufwerk,
            [CanBeNull] IHttpContextAccessor httpContextAccessor,
            [CanBeNull] string categoryName)
        {
            _haufwerk = haufwerk;
            _httpContextAccessor = httpContextAccessor;
            _categoryName = categoryName;
        }

        #endregion


        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (!_haufwerk.Options.LogLocalRequests && Haufwerk.IsLocalRequest(_httpContextAccessor?.HttpContext?.Request))
            {
                return;
            }

            var message = exception?.Message;
            if (string.IsNullOrEmpty(message) && formatter != null)
            {
                message = formatter.Invoke(state, exception);
            }
            if (string.IsNullOrEmpty(message))
            {
                message = "< no message >";
            }

            var requestUrl = _httpContextAccessor?.HttpContext?.Request?.GetDisplayUrl();
            _haufwerk.Post(
                message,
                exception: exception,
                additionalInfo: $"Log Level: {logLevel}\nRequest URL: {requestUrl}\nCategory: {_categoryName}").GetAwaiter().GetResult();
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return _haufwerk.Options.LogLevelsToLog.Any(x => x == logLevel);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}