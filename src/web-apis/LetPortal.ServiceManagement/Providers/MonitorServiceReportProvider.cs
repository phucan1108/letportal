﻿using LetPortal.ServiceManagement.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.ServiceManagement.Providers
{
    public class MonitorServiceReportProvider : IMonitorServiceReportProvider
    {
        private readonly IMonitorHardwareReportRepository _monitorHardwareReportRepository;

        private readonly IMonitorHttpReportRepository _monitorHttpReportRepository;

        public MonitorServiceReportProvider(IMonitorHardwareReportRepository monitorHardwareReportRepository, IMonitorHttpReportRepository monitorHttpReportRepository)
        {
            _monitorHardwareReportRepository = monitorHardwareReportRepository;
            _monitorHttpReportRepository = monitorHttpReportRepository;
        }

        public async Task CollectAndReportHardware(string[] serviceIds, int duration, bool roundMinute = true)
        {
            await _monitorHardwareReportRepository.CollectDataAsync(serviceIds, DateTime.UtcNow, duration, roundMinute);
        }

        public async Task CollectAndReportHttp(string[] serviceIds, int duration, bool roundMinute = true)
        {
            await _monitorHttpReportRepository.CollectDataAsync(serviceIds, DateTime.UtcNow, duration);
        }
    }
}
