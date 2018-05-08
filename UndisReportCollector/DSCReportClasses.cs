using System;
using System.Collections.Generic;


namespace UndisReportCollector
{
    public class DSCReport
    {
        public string JobId { get; set; }
        public string OperationType { get; set; }
        public string RefreshMode { get; set; }
        public string Status { get; set; }
        public string ReportFormatVersion { get; set; }
        public string ConfigurationVersion { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool RebootRequested { get; set; }
        public string[] Errors { get; set; }
        public string[] StatusData { get; set; }
        public Dictionary<string, string>[] AdditionalData { get; set; }
    }

    public class StatusData
    {
        public DateTime StartDate { get; set; }
        public string[] IPV6Addresses { get; set; }
        public int DurationInSeconds { get; set; }
        public string CurrentChecksum { get; set; }
        public string MetaData { get; set; }
        public bool RebootRequested { get; set; }
        public string Status { get; set; }
        public string[] IPV4Addresses { get; set; }
        public string LCMVersion { get; set; }
        public int NumberOfResources { get; set; }
        public string Type { get; set; }
        public string Hostname { get; set; }
        public string[] MACAddresses { get; set; }
        public string Locale { get; set; }
        public string Mode { get; set; }
        public Resource[] ResourcesInDesiredState { get; set; }
        public Resource[] ResourcesNotInDesiredState { get; set; }
        public MetaConfiguration MetaConfiguration { get; set; }
    }

    public class Resource
    {
        public string SourceInfo { get; set; }
        public string ModuleName { get; set; }
        public string DurationInSeconds { get; set; }
        public string InstanceName { get; set; }
        public DateTime StartDate { get; set; }
        public string ResourceName { get; set; }
        public string ModuleVersion { get; set; }
        public bool RebootRequested { get; set; }
        public string ResourceId { get; set; }
        public string ConfigurationName { get; set; }
        public bool InDesiredState { get; set; }
    }

    public class MetaConfiguration
    {
        public string AgentId { get; set; }
        public Manager[] ConfigurationDownloadManagers { get; set; }
        public string ActionAfterReboot { get; set; }
        public string[] LCMCompatibleVersions { get; set; }
        public string LCMState { get; set; }
        public Manager[] ResourceModuleManagers { get; set; }
        public Manager[] ReportManagers { get; set; }
        public int StatusRetentionTimeInDays { get; set; }
        public string LCMVersion { get; set; }
        public int MaximumDownloadSizeMB { get; set; }
        public string ConfigurationMode { get; set; }
        public int RefreshFrequencyMins { get; set; }
        public bool RebootNodeIfNeeded { get; set; }
        public string SignatureValidationPolicy { get; set; }
        public string RefreshMode { get; set; }
        public string[] DebugMode { get; set; }
        public string LCMStateDetail { get; set; }
        public bool AllowModuleOverwrite { get; set; }
        public int ConfigurationModeFrequencyMins { get; set; }
        public string[] SignatureValidations { get; set; }
    }

    public class Manager
    {
        public string RegistrationKey { get; set; }
        public string ServerURL { get; set; }
        public string ResourceId { get; set; }
        public string[] ConfigurationNames { get; set; }
    }
}