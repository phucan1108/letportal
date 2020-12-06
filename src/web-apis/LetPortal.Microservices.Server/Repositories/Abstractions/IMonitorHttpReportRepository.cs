﻿using System;
using System.Threading.Tasks;
using LetPortal.Core.Persistences;
using LetPortal.Microservices.Server.Entities;

namespace LetPortal.Microservices.Server.Repositories.Abstractions
{
    public interface IMonitorHttpReportRepository : IGenericRepository<MonitorHttpReport>
    {
        Task CollectDataAsync(string[] collectServiceIds, DateTime reportDate, int duration, bool roundDate = true);
    }
}
