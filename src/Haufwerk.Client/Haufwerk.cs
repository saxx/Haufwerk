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
            if (user != null)
            {
                content["StackTrace"] = stackTrace;
            }
            if (user != null)
            {
                content["AdditionalInfo"] = additionalInfo;
            }

            var response = await client.PostAsync(_haufwerkInstanceUrl, new FormUrlEncodedContent(content));
            response.EnsureSuccessStatusCode();
        }
    }
}
