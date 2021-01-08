# Serilog.Sinks.Datadog.Logs

A NLog target that send events and logs staight away to Datadog. By default the sink sends logs over HTTPS

**Package** - [Serilog.Sinks.Datadog.Logs](https://www.nuget.org/packages/NLog.Target.Datadog/0.3.0)
| **Platforms** - .NET 4.5, .NET 4.6.1, .NET 4.7.2, netstandard1.3, netstandard2.0

Note: For other .NET versions, ensure that the default TLS version used is `1.2`


By default the logs are forwarded to Datadog via **HTTPS** on port 443 to the US site.
You can change the site to EU by using the `url` property and set it to `https://http-intake.logs.datadoghq.eu`.

You can override the default behavior and use **TCP** forwarding by manually specifing the following properties (url, port, useSSL, useTCP).

You can also add the following properties: source, service, host, tags.

* Example with HTTP:

```xml
<?xml version="1.0" encoding="utf-8"?>

<configuration>
  <configSections>
    <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog" />
  </configSections>
  <nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
    <extensions>
      <add assembly="NLog.Target.Datadog" />
    </extensions>
    <targets>
      <target name="dataDogBuf" xsi:type="BufferingWrapper">
        <target xsi:type="DataDog"
                layout="${message}"
                includeAllProperties="true"
                maxRetries="666"
                apiKey="YOUR API KEY">

          <field name="ddsource" layout="${machinename}" />
          <field name="service" layout="${machinename}" />
          <field name="host" layout="${machinename}" />

          <field name="Logger" layout="${logger}" />
          <field name="ProcessID" layout="${processid}" />
          <field name="ProcessName" layout="${processname}" />
          <field name="Thread" layout="${threadid}" />
          <field name="ThreadName" layout="${threadname}" />
          <field name="Class"
                 layout="${callsite:className=true:methodName=false:fileName=false:includeSourcePath=false}" />
          <field name="Method"
                 layout="${callsite:className=false:methodName=true:fileName=false:includeSourcePath=false}" />

          <field name="sessionId" layout="12345" />
          <field name="Version" layout="${gdc:item=Version}" />
          <field name="Environment" layout="${gdc:item=Version}" />
        </target>
      </target>
    </targets>
    <rules>
      <logger name="*" minlevel="Trace" writeTo="dataDogBuf" />
    </rules>
  </nlog>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

