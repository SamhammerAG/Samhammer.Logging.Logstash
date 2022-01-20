# Samhammer.Logging.Logstash

## Usage
This package is a implementation of the serilog http sink https://github.com/FantasticFiasco/serilog-sinks-http to use logstash.

#### How to add this to your project:
- reference this package to your project: https://www.nuget.org/packages/Samhammer.Logging.Logstash/
- call WriteToLogstash on the LoggerConfiguration from Serilog
- add the logstash configuration to the appsettings

#### Example:
```csharp
   public static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration logger)
   {
       var elasticIndexPlaceholders = new Dictionary<string, string>
       {
           { "{environment}", context.HostingEnvironment.EnvironmentName },
       };
           
       logger.WriteTo.Logstash(context.Configuration, elasticIndexPlaceholders)            
   }
```

#### Example appsettings configuration:
```json
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "HealthChecks.UI": "Fatal"
      }
    },
    "Logstash": {
      "Url": "<YOUR_LOGSTASH_URL>",
      "UserName": "<YOUR_LOGSTASH_USERNAME>",
      "Password": "<YOUR_LOGSTASH_PASSWORD>",
      "ElasticIndex": "<YOUR_ELASTIC_INDEX_TEMPLATE>"
    }
  },
```

#### Example elastic index template:
```
projectname-{environment}-{brand}
```
The placeholders in the elastic index template will be replaced with the values of the placeholders dictionary.

## Contribute

#### How to publish package
- Create a tag and let the github action do the publishing for you
