using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.Http;

namespace Samhammer.Logging.Logstash
{
    public class BasicAuthenticatedHttpClient : IHttpClient
    {
        private readonly HttpClient client;

        public BasicAuthenticatedHttpClient(string username, string password)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            client = CreateHttpClient(username, password);
        }

        public void Configure(IConfiguration configuration)
        {
            // Only called if IConfiguration parameter is added to the httpSink configuration.
            // This is not the case with our extension setup in LoggerSinkConfigurationExtensions
            throw new NotImplementedException();
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream contentStream)
        {
            using (var content = new StreamContent(contentStream))
            {
                return await client.PostAsync(requestUri, content);
            }
        }

        public void Dispose() => client?.Dispose();

        private static HttpClient CreateHttpClient(string username, string password)
        {
            var client = new HttpClient();

            var schema = "Basic";
            var parameter = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(schema, parameter);

            return client;
        }
    }
}
