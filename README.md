# Samhammer.Logging.Logstash

## Usage

#### How to add this to your project:
- reference this package to your project: https://www.nuget.org/packages/Samhammer.Logging.Logstash/
- call WriteToLogstash on the LoggerConfiguration from Serilog
- add the logstash configuration to the appsettings

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

## Contribute

#### How to publish package
- set package version in Samhammer.DependencyInjection.csproj
- add information to changelog
- create git tag
- dotnet pack -c Release
- nuget push .\bin\Release\Samhammer.DependencyInjection.*.nupkg NUGET_API_KEY -src https://api.nuget.org/v3/index.json
- (optional) nuget setapikey NUGET_API_KEY -source https://api.nuget.org/v3/index.json
