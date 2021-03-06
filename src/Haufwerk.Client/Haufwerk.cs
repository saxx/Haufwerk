﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AsyncFriendlyStackTrace;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

namespace Haufwerk.Client
{
    public class Haufwerk : IHaufwerk
    {
        public Haufwerk([NotNull] HaufwerkOptions options)
        {
            Options = options;
        }


        public Haufwerk([NotNull] string source, [NotNull] string uri)
        {
            Options = new HaufwerkOptions(source, uri);
        }


        public Haufwerk([NotNull] string source, [NotNull] Uri uri)
        {
            Options = new HaufwerkOptions(source, uri);
        }


        public HaufwerkOptions Options { get; }


        public async Task Post(
            string message,
            string source = null,
            string user = null,
            string stackTrace = null,
            string additionalInfo = null)
        {
            try
            {
                source = source ?? Options.Source;

                var client = new HttpClient();

                var content = new Dictionary<string, string>
                {
                    ["Source"] = source,
                    ["Message"] = message
                };

                if (user != null)
                {
                    content["User"] = user;
                }
                if (stackTrace != null)
                {
                    content["StackTrace"] = stackTrace;
                }
                if (additionalInfo != null)
                {
                    content["AdditionalInfo"] = additionalInfo;
                }

                var response = await client.PostAsync(Options.InstanceUri, new FormUrlEncodedContent(content));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new HaufwerkException(ex);
            }
        }


        public async Task Post(string message, string source = null, string user = null, Exception exception = null, string additionalInfo = null)
        {
            await Post(message, source, user, ToAsyncStringSafe(exception), additionalInfo);
        }


        public async Task Post(Exception exception, string source = null, string user = null, string additionalInfo = null)
        {
            await Post(exception.Message, source, user, ToAsyncStringSafe(exception), additionalInfo);
        }


        internal static bool IsLocalRequest([CanBeNull] HttpRequest request)
        {
            if (request != null && request.Host.HasValue)
            {
                var host = request.Host.Value.ToLower();
                if (host.Contains("localhost") || host.Contains("127.0.0.1"))
                {
                    return true;
                }
            }
            return false;
        }


        [CanBeNull]
        internal static string ToAsyncStringSafe([CanBeNull] Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            try
            {
                return exception.ToAsyncString();
            }
            catch
            {
                return exception.ToString();
            }
        }
    }
}
