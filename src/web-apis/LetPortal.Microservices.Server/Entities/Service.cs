using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.Microservices.Server.Entities
{
    [EntityCollection(Name = "services")]
    [Table("services")]
    public class Service : Entity
    {
        public string Name { get; set; }

        public int InstanceNo { get; set; }

        public ServiceState ServiceState { get; set; }

        public string RunningVersion { get; set; }

        public string IpAddress { get; set; }

        public ServiceHardwareInfo ServiceHardwareInfo { get; set; }

        public bool LoggerNotifyEnable { get; set; }

        public bool HealthCheckNotifyEnable { get; set; }

        public DateTime LastCheckedDate { get; set; }

        public string TotalRunningTime { get; set; }

        public DateTime RegisteredDate { get; set; }

        public IList<MonitorCounter> MonitorCounters { get; set; }

        public void CalculateRunningTime()
        {
            var duration = LastCheckedDate.Subtract(RegisteredDate);
            var dayStr = string.Empty;
            var hourStr = string.Empty;
            var minuteStr = string.Empty;
            if (duration.Days > 0)
            {
                dayStr = duration.Days == 1
                            ? string.Format("{0} day", duration.Days)
                            : string.Format("{0} days", duration.Days);
            }

            if (duration.Hours > 0)
            {
                hourStr = duration.Hours == 1
                            ? string.Format("{0} hour", duration.Hours)
                            : string.Format("{0} hours", duration.Hours);
            }

            if (duration.Minutes > 0)
            {
                minuteStr = duration.Minutes == 1
                            ? string.Format("{0} minute", duration.Minutes)
                            : string.Format("{0} minutes", duration.Minutes);
            }

            var presentStr = string.Format("{0} {1} {2}", dayStr, hourStr, minuteStr).Trim();
            TotalRunningTime = presentStr;
        }
    }

    public enum ServiceState
    {
        Start,
        Run,
        Shutdown,
        Lost
    }
}
