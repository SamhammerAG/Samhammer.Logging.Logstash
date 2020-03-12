using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Configuration;
using Serilog.Formatting.Elasticsearch;
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
            var index = BuildElasticIndex(logstashOptions.ElasticIndex, placeholders);

            return sinkConfiguration.Http(
                $"{logstashOptions.Url}/{index}",
                batchFormatter: new ArrayBatchFormatter(),
                textFormatter: new ElasticsearchJsonFormatter(),
                httpClient: new BasicAuthenticatedHttpClient(logstashOptions.UserName, logstashOptions.Password));
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

            // ATTENTION this validation is required to ensure elastic gets an valid index name, otherwise no logs are written
            var indexRegex = new Regex(@"^((([a-z]))-?)*$");

            if (!indexRegex.IsMatch(index))
            {
                throw new ArgumentException($"elastic index {index} contains invalid characters", nameof(index));
            }

            return index.ToLower();
        }

        private static LogstashOptions LoadLogstashOptions(IConfiguration configuration)
        {
            var logstashOptions = new LogstashOptions();
            configuration.GetSection("serilog:logstash").Bind(logstashOptions);
            return logstashOptions;
        }
    }
}
