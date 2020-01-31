using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Entities;
using LetPortal.ServiceManagement.Repositories.Abstractions;

namespace LetPortal.ServiceManagement.Repositories.Implements
{
    public class MonitorHardwareReportEFRepository : EFGenericRepository<MonitorHardwareReport>, IMonitorHardwareReportRepository
    {
        private readonly LetPortalServiceManagementDbContext _context;
        public MonitorHardwareReportEFRepository(LetPortalServiceManagementDbContext context)
            : base(context)
        {
            _context = context;
        }

        public Task CollectDataAsync(string[] collectServiceIds, DateTime reportDate, int duration, bool roundDate = true)
        {
            duration /= 60;
            var allInsertCounters = new List<MonitorHardwareReport>();
            var nearestMinute = 0;
            var allRequiredCounters = new List<HardwareCounter>();
            var allowMinutes = new List<int>
            {
              0
            };

            for (var i = duration; i < 60; i += duration)
            {
                if (i < reportDate.Minute)
                {
                    nearestMinute = i;
                }
                allowMinutes.Add(i);
            }
            var endDate = new DateTime(
                                        reportDate.Year,
                                        reportDate.Month,
                                        reportDate.Day,
                                        reportDate.Hour,
                                        nearestMinute,
                                        1,
                                        DateTimeKind.Utc);

            var startDate = endDate.AddMinutes(duration * -1);
            var lastestRecord = _context.MonitorHardwareReports.Where(a => collectServiceIds.Contains(a.ServiceId)).OrderByDescending(a => a.ReportedDate).FirstOrDefault();
            if (lastestRecord != null)
            {
                var lastMinute = lastestRecord.ReportedDate.Minute;
                var nextStartDate = lastestRecord.ReportedDate.AddMinutes(duration);
                // Ensure report must be over last record
                if (nextStartDate < reportDate)
                {
                    startDate = new DateTime(
                                        nextStartDate.Year,
                                        nextStartDate.Month,
                                        nextStartDate.Day,
                                        nextStartDate.Hour,
                                        nextStartDate.Minute,
                                        0,
                                        DateTimeKind.Utc);

                    allRequiredCounters = _context.HardwareCounters.Where(a => collectServiceIds.Contains(a.ServiceId) && a.MeansureDate >= startDate && a.MeansureDate < endDate).OrderBy(b => b.MeansureDate).ToList();
                }
            }
            else
            {
                startDate = new DateTime(
                                        reportDate.Year,
                                        reportDate.Month,
                                        reportDate.Day,
                                        reportDate.Hour,
                                        0,
                                        0,
                                        DateTimeKind.Utc);

                allRequiredCounters = _context.HardwareCounters.Where(a => collectServiceIds.Contains(a.ServiceId) && a.MeansureDate >= startDate && a.MeansureDate < endDate).OrderBy(b => b.MeansureDate).ToList();
            }



            if (allRequiredCounters.Any())
            {
                var counter = allRequiredCounters.Count;
                var startMinute = allRequiredCounters.First().MeansureDate.Minute;

                for (var i = 0; i < allowMinutes.Count; i++)
                {
                    if (allowMinutes[i] <= startMinute && allowMinutes[i + 1] > startMinute)
                    {
                        startMinute = allowMinutes[i];
                        break;
                    }
                }

                var firstCounterDate = allRequiredCounters.First().MeansureDate;
                var startCompareDate = new DateTime(
                                        firstCounterDate.Year,
                                        firstCounterDate.Month,
                                        firstCounterDate.Day,
                                        firstCounterDate.Hour,
                                        startMinute,
                                        0,
                                        DateTimeKind.Utc);
                var endCompareDate = startCompareDate.AddMinutes(duration);
                var services = allRequiredCounters.GroupBy(a => a.ServiceId);

                while (counter > 0)
                {
                    foreach (var service in services)
                    {
                        var proceedingCounters = service.Where(a => a.MeansureDate >= startCompareDate && a.MeansureDate < endCompareDate);

                        if (proceedingCounters.Any())
                        {
                            counter -= proceedingCounters.Count();
                            var newReportCounter = new MonitorHardwareReport
                            {
                                Id = DataUtil.GenerateUniqueId(),
                                CpuUsage = Math.Round(proceedingCounters.Average(b => b.CpuUsage), 0, MidpointRounding.AwayFromZero),
                                MemoryUsed = Convert.ToInt64(proceedingCounters.Average(a => a.MemoryUsed)),
                                IsCpuBottleneck = proceedingCounters.Any(a => a.IsCpuBottleneck),
                                IsMemoryThreshold = proceedingCounters.Any(a => a.IsMemoryThreshold),
                                ReportedDate = startCompareDate,
                                ServiceId = service.Key
                            };
                            newReportCounter.MemoryUsedInMb = (int)Math.Round((double)(newReportCounter.MemoryUsed / 1024), 0, MidpointRounding.AwayFromZero);
                            allInsertCounters.Add(newReportCounter);
                        }
                    }

                    startCompareDate = startCompareDate.AddMinutes(duration);
                    endCompareDate = startCompareDate.AddMinutes(duration);
                }

                _context.MonitorHardwareReports.AddRange(allInsertCounters.ToArray());
                _context.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}
