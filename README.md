﻿[![Build Status](https://travis-ci.com/SamhammerAG/Samhammer.Logging.Logstash.svg?branch=master)](https://travis-ci.com/SamhammerAG/Samhammer.Logging.Logstash)

# Samhammer.Logging.Logstash

## Usage
This package is a implementation of the serilog http sink https://github.com/FantasticFiasco/serilog-sinks-http to use logstash.

#### How to add this to your project:
- reference this package to your project: https://www.nuget.org/packages/Samhammer.Logging.Logstash/
- call WriteToLogstash on the LoggerConfiguration from Serilog
- add the logstash configuration to the appsettings

#### Example:
```csharp
    protected virtual void WriteToLogstash()
    {
        var placeholders = BuildElasticIndexPlaceholders();
        SerilogConfig.WriteToLogstash(Configuration, placeholders);
    }

    protected virtual Dictionary<string, string> BuildElasticIndexPlaceholders()
    {
        return new Dictionary<string, string>
        {
            {
                "{environment}", EnvironmentName
            },
            {
                "{brand}", Environment.GetEnvironmentVariable("Brand")
            }
        };
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
- set package version in Samhammer.Logging.Logstash.csproj
- add information to changelog
- create git tag
- dotnet pack -c Release
- nuget push .\bin\Release\Samhammer.Logging.Logstash.*.nupkg NUGET_API_KEY -src https://api.nuget.org/v3/index.json
- (optional) nuget setapikey NUGET_API_KEY -source https://api.nuget.org/v3/index.json
