using System;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Haufwerk.Client
{
    public class HaufwerkErrorLoggerProvider : ILoggerProvider
    {
        #region Constructor
        [NotNull]
        private readonly IHaufwerk _haufwerk;
        [NotNull]
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HaufwerkErrorLoggerProvider(
            [NotNull] IHaufwerk haufwerk,
            [NotNull] IHttpContextAccessor httpContextAccessor)
        {
            if (haufwerk == null)
            {
                throw new ArgumentNullException(nameof(haufwerk), "Make sure you added Haufwerk to your dependency injection container (ie. via AddHaufwerk()).");
            }

            _haufwerk = haufwerk;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion


        public void Dispose()
        {
            // nothing to dispose here           
        }

        [NotNull]
        public ILogger CreateLogger(string categoryName)
        {
            return new HaufwerkErrorLogger(_haufwerk, _httpContextAccessor, categoryName);
        }
    }
}