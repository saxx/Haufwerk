using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Haufwerk.Client
{
    public class Haufwerk : IHaufwerk
    {
        private readonly string _haufwerkInstanceUrl;

        public Haufwerk(string haufwerkInstanceUrl)
        {
            _haufwerkInstanceUrl = haufwerkInstanceUrl;
        }


        public async Task Post(
            string source,
            string message,
            string user = null,
            string stackTrace = null,
            string additionalInfo = null)
        {
            try
            {
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

                var response = await client.PostAsync(_haufwerkInstanceUrl, new FormUrlEncodedContent(content));
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                throw new HaufwerkException(ex);
            }
        }


        public async Task Post(string source, string message, string user = null, Exception exception = null, string additionalInfo = null)
        {
            await Post(source, message, user, exception?.ToString(), additionalInfo);
        }
    }
}
