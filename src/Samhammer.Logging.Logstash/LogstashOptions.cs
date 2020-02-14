namespace Samhammer.Logging.Logstash
{
    public class LogstashOptions
    {
        public string Url { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string ElasticIndex { get; set; }
    }
}
