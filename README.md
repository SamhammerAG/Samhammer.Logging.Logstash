# Samhammer.Logging.Logstash

## Usage
This package is a implementation of the serilog http sink https://github.com/FantasticFiasco/serilog-sinks-http to use logstash.

#### How to add this to your project:
- reference this package to your project: https://www.nuget.org/packages/Samhammer.Logging.Logstash/
- call WriteToLogstash on the LoggerConfiguration from Serilog
- add the logstash configuration to the appsettings

```csharp
LoggerConfiguration.WriteToLogstash(Configuration, Environment);
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
projectname-{environment}-{brand}-{date}
```
The {environment} placeholder will be replaced with the environment name.
The {brand} placeholder will be replaced with the value of a environment varialbe called "Brand".
The {date} placeholder will be replaced with {0:yyyy.MM}.

## Contribute

#### How to publish package
- set package version in Samhammer.Logging.Logstash.csproj
- add information to changelog
- create git tag
- dotnet pack -c Release
- nuget push .\bin\Release\Samhammer.Logging.Logstash.*.nupkg NUGET_API_KEY -src https://api.nuget.org/v3/index.json
- (optional) nuget setapikey NUGET_API_KEY -source https://api.nuget.org/v3/index.json
