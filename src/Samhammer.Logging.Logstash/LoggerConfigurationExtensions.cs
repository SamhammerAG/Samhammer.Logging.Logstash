using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace Samhammer.Logging.Logstash
{
    public static class LoggerConfigurationExtensions
    {
        public static LoggerConfiguration WriteToLogstash(this LoggerConfiguration serilogConfig, IConfiguration configuration, string environment)
        {
            var logstashOptions = LoadLogstashOptions(configuration);

            var index = BuildElasticIndex(logstashOptions.ElasticIndex, environment);

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

        public static string BuildElasticIndex(string indexTemplate, string environment)
        {
            // ATTENTION this validation is required to ensure elastic gets an valid index name, otherwise no logs are written
            var indexRegex = new Regex(@"^((([a-z])|({date})|({environment})|({brand}))-?)*$");

            if (!indexRegex.IsMatch(indexTemplate))
            {
                throw new ArgumentException($"elastic index {indexTemplate} contains invalid characters", nameof(indexTemplate));
            }

            var retVal = indexTemplate
                .Replace("{environment}", environment.ToLower())
                .Replace("{brand}", Environment.GetEnvironmentVariable("Brand"))
                .Replace("{date}", "{0:yyyy.MM}");

            return retVal;
        }
    }
}
