# NLog.Target.DataDog

A NLog target that send events and logs staight away to Datadog. By default the sink sends logs over HTTPS

**Package** - [NLog.Target.DataDog](https://www.nuget.org/packages/NLog.Target.Datadog/)
| **Platforms** - .NET 4.5, .NET 4.6.1, .NET 4.7.2, netstandard1.3, netstandard2.0

Note: For other .NET versions, ensure that the default TLS version used is `1.2`


## Parameters

### url
The url of DataDog intake service. Default: `https://http-intake.logs.datadoghq.com`

### UseTCP
If TCP instead HTTP to be used. Default: `false`

### UseSSL
if using `TCP`, if to use SSL

### DDPort
if using `TCP`, the secure (SSL) port

### DDPortNoSSL
if using `TCP`, the insecure (no SSL) port

### apiKey
Your API key.

You can also add the following properties: source, service, host, tags.

## Confifuration

* Example with HTTP (replace "YOUR API KEY" with your DataDog API key):

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
      <default-wrapper xsi:type="AsyncWrapper" overflowAction="Grow" timeToSleepBetweenBatches="1"
      />
      <target xsi:type="DataDog"
              name="dataDog"
              layout="${message}"
              includeAllProperties="true"
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
    </targets>
    <rules>
      <logger name="*" minlevel="Trace" writeTo="dataDog" />
    </rules>
  </nlog>
  <startup>
    <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.7.2" />
  </startup>
</configuration>
```

