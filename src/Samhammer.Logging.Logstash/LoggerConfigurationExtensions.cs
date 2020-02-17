using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Samhammer.Logging.Logstash
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WriteToLogstash(this LoggerConfiguration serilogConfig, IConfiguration configuration, IDictionary<string, string> placeholders = null)
        {
            var logstashOptions = LoadLogstashOptions(configuration);

            var index = BuildElasticIndex(logstashOptions.ElasticIndex, placeholders);

            serilogConfig.WriteTo.Http(
                $"{logstashOptions.Url}/{index}",
                httpClient: new BasicAuthenticatedHttpClient(logstashOptions.UserName, logstashOptions.Password));
            return serilogConfig;
        }

        private static LogstashOptions LoadLogstashOptions(IConfiguration configuration)
        {
            var logstashOptions = new LogstashOptions();
            configuration.GetSection("serilog:logstash").Bind(logstashOptions);
            return logstashOptions;
        }

        public static string BuildElasticIndex(string indexTemplate, IDictionary<string, string> placeholders)
        {
            // ATTENTION this validation is required to ensure elastic gets an valid index name, otherwise no logs are written
            var indexRegex = new Regex(@"^((([a-z]))-?)*$");

            if (!indexRegex.IsMatch(indexTemplate))
            {
                throw new ArgumentException($"elastic index {indexTemplate} contains invalid characters", nameof(indexTemplate));
            }

            var retVal = indexTemplate;

            foreach (var placeholder in placeholders)
            {
                retVal = retVal.Replace(placeholder.Key, placeholder.Value);
            }

            return retVal;
        }
    }
}
