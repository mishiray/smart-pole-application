namespace DigitalTwinFramework.DTOs.Enums
{
    public enum OperationalStatus
    {
        Offline, Running, Idle, StandBy, Error, Warning, Busy
    }

    public enum PowerStatus
    {
        Off, On, NoPower
    }
    public enum MaintenanceStatus
    {
        Scheduled, Completed, Overdue, Required, NotRequired, PartsReceived, PartsOnOrder
    }
    public enum PerformanceStatus
    {
        Slow, Normal, Fast, Overloaded, LowAccuracy, HighAccuracy, Unresponsive
    }
    public enum HealthStatus
    {
        Offline, Normal, Warning, Critical, Degraded, Overheating
    }
    public enum ConfigurationStatus
    {
        Default, Current, Outdated, Inconsistent, Misconfigured, Saved, NotSaved
    }
}
