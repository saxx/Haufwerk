using System;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Extensions;
using Microsoft.Extensions.Logging;

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

        public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
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
            try
            {
                _haufwerk.Post(
                    _haufwerk.Options.Source,
                    message,
                    exception: exception,
                    additionalInfo: $"Log Level: {logLevel}\nRequest URL: {requestUrl}\nCategory: {_categoryName}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (exception != null)
                {
                    Console.WriteLine ("====================== Exception that should have been logged ======================");
                    Console.WriteLine(exception.ToString());
                }
            }
        }


        public bool IsEnabled(LogLevel logLevel)
        {
            return _haufwerk.Options.LogLevelsToLog.Any(x => x == logLevel);
        }


        [CanBeNull]
        public IDisposable BeginScopeImpl(object state)
        {
            // no support for scopes
            return null;
        }
    }
}