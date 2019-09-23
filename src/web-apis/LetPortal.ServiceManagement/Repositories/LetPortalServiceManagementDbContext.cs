using LetPortal.Core.Persistences;
using LetPortal.Core.Services.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;

namespace LetPortal.ServiceManagement.Repositories
{
    public class LetPortalServiceManagementDbContext : DbContext
    {
        public DbSet<LogEvent> LogEvents { get; set; }

        public DbSet<MonitorCounter> MonitorCounters { get; set; }

        public DbSet<Service> Services { get; set; }

        public DbSet<ServiceHardwareInfo> ServiceHardwareInfos { get; set; }

        public DbSet<HttpCounter> HttpCounters { get; set; }

        public DbSet<HardwareCounter> HardwareCounters { get; set; }

        private readonly DatabaseOptions _options;

        public LetPortalServiceManagementDbContext(DatabaseOptions options)
        {
            _options = options;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var serviceBuilder = modelBuilder.Entity<Service>();
            serviceBuilder.HasKey(a => a.Id);

            serviceBuilder.HasOne(a => a.ServiceHardwareInfo).WithOne().OnDelete(DeleteBehavior.Cascade);

            var logEventBuilder = modelBuilder.Entity<LogEvent>();
            logEventBuilder.HasKey(a => a.Id);

            var jsonStackTracesConverter = new ValueConverter<List<string>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<List<string>>(v));
            logEventBuilder.Property(a => a.StackTrace).HasConversion(jsonStackTracesConverter);

            var monitorCounterBuilder = modelBuilder.Entity<MonitorCounter>();
            monitorCounterBuilder.HasKey(a => a.Id);

            monitorCounterBuilder.HasOne(a => a.HardwareCounter).WithOne().OnDelete(DeleteBehavior.Cascade);
            monitorCounterBuilder.HasOne(a => a.HttpCounter).WithOne().OnDelete(DeleteBehavior.Cascade);
            monitorCounterBuilder.HasOne(a => a.Service).WithOne();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_options.ConnectionType == ConnectionType.SQLServer)
            {
                optionsBuilder.UseSqlServer(_options.ConnectionString);
            }
            else if(_options.ConnectionType == ConnectionType.PostgreSQL)
            {
                optionsBuilder.UseNpgsql(_options.ConnectionString);
            }
        }
    }
}
