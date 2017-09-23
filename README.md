# UndisReportCollector

Report Collector is an IIS HTTP module that grabs reports sent to DSC Pull Server.

This module take place in the IIS request pipeline and dump informations before they are processed by the Pull Server application. Doing this way allows to collect information without opening the DSC Pull server 'black box'.

This module has 2 outputs:
- reports recording - job reports are saved 'as is' on disk for further processing.
- reports logging - main indicators of job reports are logged in a file.

# How to install

Compile a release version of the module or use a pre-compiled release.

Copy file ***UndisReportCollector.dll*** in the ***bin*** folder of your DSC Pull Server setup folder (may be *C:\inetpub\wwwroot\PSDSCPullServer\bin*).

Edit ***web.config*** file in your DSC Pull Server setup (may be *C:\inetpub\wwwroot\PSDSCPullServer\web.config*).

Add Undis configuration keys in the <appSettings> section. See Configuration chapter for possible values. Example:

    <appSettings>
      ...
      <add key="UndisLogsFolder" value="C:\Undis\Logs" />
      <add key="UndisReportsFolder" value="C:\Undis\Reports" />
    </appSettings>

Add Undis HTTP module reference in ***<modules>*** section.

    <modules>
      ...
      <add name="UndisReportCollector" type="UndisReportCollector.HTTPModule" />
    </modules>

Save web.config. This should reload IIS application pool and start grabbing DSC reports.

# Configuration
## Reports recording

In order to write DSC reports sent to Pull Server on disk the following parameter must be present in Pull Server's web.config:

    <add key="UndisReportsFolder" value="C:\Undis\Reports" />

Of course, folder can be changed.
The folder **MUST** exists. If it doesn't, report recording is disabled.

If you want to record 'in progress' DSC jobs, the following parameter must be placed in Pull Server's web.config:

    <add key="UndisReportInProgress" value="true" />

## Reports logging

In order to log DSC jobs activities, the following parameter must be present in Pull Server's web.config:

    <add key="UndisLogsFolder" value="C:\Undis\Logs" />
    
Of course, folder can be changed.
The folder **MUST** exists. If it doesn't, report logging is disabled.

There are 3 log levels:
- VERBOSE: when an agent reports an 'in progress' job.
- INFO: when an agent reports a 'successfull' job.
- ERROR: when an agent reports a 'failed' job.

By default, INFO and ERRORs are logged. If you want to log more, use the following parameter:

    <add key="UndisLogLevel" value="VERBOSE" />
    
If you want to log less, use the following parameter:

    <add key="UndisLogLevel" value="ERROR" />
    
# Reports format

Each received report is written in a file named agent_id.json (where agent_id is the ID generated when agent registered to the pull server).

Data on disk is as LCM sent it to the Pull Server; no translation is done.

TODO: find a reference for LCM reports format.

# Logging format

Logs are written in a daily log file.

Main report indicators are logged in a key=value format. This format is easy to process with a log system as Splunk for example.

    2017-09-23 20:24:25 INFO HostName=UNDIS01 OperationType=Consistency Status=Success RefreshMode=Pull RebootRequested=False DurationInSeconds=0 NumberOfResources=1 JobId=02d1e57c-9d22-11e7-80c2-000c29c6ad33
