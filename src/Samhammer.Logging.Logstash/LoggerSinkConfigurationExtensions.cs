using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Http;
using Serilog.Sinks.Http.BatchFormatters;

namespace Samhammer.Logging.Logstash
{
    public static class LoggerSinkConfigurationExtensions
    {
        public static LoggerConfiguration Logstash(this LoggerSinkConfiguration sinkConfiguration, IConfiguration configuration, IDictionary<string, string> placeholders = null)
        {
            var logstashOptions = LoadLogstashOptions(configuration);
            return sinkConfiguration.Logstash(logstashOptions, placeholders);
        }

        public static LoggerConfiguration Logstash(this LoggerSinkConfiguration sinkConfiguration, LogstashOptions logstashOptions, IDictionary<string, string> placeholders = null)
        {
            return
                sinkConfiguration.Logstash(logstashOptions.Url, logstashOptions.UserName, logstashOptions.Password, logstashOptions.ElasticIndex, placeholders);
        }

        public static LoggerConfiguration Logstash(this LoggerSinkConfiguration sinkConfiguration, string url, string username, string password, string elasticIndex, IDictionary<string, string> placeholders = null)
        {
            var index = BuildElasticIndex(elasticIndex, placeholders);

            return sinkConfiguration.Http(
                $"{url}/{index}",
                50 * ByteSize.MB,
                batchFormatter: new ArrayBatchFormatter(),
                textFormatter: new ElasticsearchJsonFormatter(),
                httpClient: new BasicAuthenticatedHttpClient(username, password));
        }

        public static string BuildElasticIndex(string indexTemplate, IDictionary<string, string> placeholders)
        {
            var index = indexTemplate;

            if (string.IsNullOrWhiteSpace(index))
            {
                return string.Empty;
            }

            if (placeholders != null)
            {
                foreach (var placeholder in placeholders)
                {
                    index = index.Replace(placeholder.Key, placeholder.Value);
                }
            }

            // Ensure that the entire index is written in lower case.
            // Other issues like special chars will lead to the exception below
            index = index.ToLower();

            // ATTENTION this validation is required to ensure elastic gets an valid index name, otherwise no logs are written
            var indexRegex = new Regex(@"^((([a-z]))-?)*$");

            if (!indexRegex.IsMatch(index))
            {
                throw new ArgumentException($"elastic index {index} contains invalid characters", nameof(index));
            }

            return index;
        }

        private static LogstashOptions LoadLogstashOptions(IConfiguration configuration)
        {
            var logstashOptions = new LogstashOptions();
            configuration.GetSection("serilog:logstash").Bind(logstashOptions);
            return logstashOptions;
        }
    }
}
