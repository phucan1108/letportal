using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Core.Services.Models;
using LetPortal.Core.Utils;
using LetPortal.ServiceManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        public DbSet<MonitorHttpReport> MonitorHttpReports { get; set; }

        public DbSet<MonitorHardwareReport> MonitorHardwareReports { get; set; }

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
            serviceBuilder.HasMany(a => a.MonitorCounters).WithOne(b => b.Service).OnDelete(DeleteBehavior.Cascade);

            if (_options.ConnectionType == ConnectionType.MySQL)
            {
                serviceBuilder.Property(a => a.HealthCheckNotifyEnable).HasColumnType("BIT");
                serviceBuilder.Property(a => a.LoggerNotifyEnable).HasColumnType("BIT");
            }

            var logEventBuilder = modelBuilder.Entity<LogEvent>();
            logEventBuilder.HasKey(a => a.Id);

            var jsonStackTracesConverter = new ValueConverter<IEnumerable<string>, string>(
                v => ConvertUtil.SerializeObject(v, true),
                v => ConvertUtil.DeserializeObject<IEnumerable<string>>(v));
            logEventBuilder.Property(a => a.StackTrace).HasConversion(jsonStackTracesConverter);

            var monitorCounterBuilder = modelBuilder.Entity<MonitorCounter>();
            monitorCounterBuilder.HasKey(a => a.Id);

            monitorCounterBuilder.HasOne(a => a.HardwareCounter).WithOne().OnDelete(DeleteBehavior.Cascade);
            monitorCounterBuilder.HasOne(a => a.HttpCounter).WithOne().OnDelete(DeleteBehavior.Cascade);

            var hardwareBuilder = modelBuilder.Entity<HardwareCounter>();

            if (_options.ConnectionType == ConnectionType.MySQL)
            {
                hardwareBuilder.Property(a => a.IsCpuBottleneck).HasColumnType("BIT");
                hardwareBuilder.Property(a => a.IsMemoryThreshold).HasColumnType("BIT");
            }

            var monitorHardwareBuilder = modelBuilder.Entity<MonitorHardwareReport>();
            if (_options.ConnectionType == ConnectionType.MySQL)
            {
                monitorHardwareBuilder.Property(a => a.IsCpuBottleneck).HasColumnType("BIT");
                monitorHardwareBuilder.Property(a => a.IsMemoryThreshold).HasColumnType("BIT");
            }

            var monitorHttpBuilder = modelBuilder.Entity<MonitorHttpReport>();

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(ToCamelCase(property.GetColumnName()));
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_options.ConnectionType == ConnectionType.SQLServer)
            {
                optionsBuilder.UseSqlServer(_options.ConnectionString);
            }
            else if (_options.ConnectionType == ConnectionType.PostgreSQL)
            {
                optionsBuilder.UseNpgsql(_options.ConnectionString);
            }
            else if (_options.ConnectionType == ConnectionType.MySQL)
            {
                optionsBuilder.UseMySql(_options.ConnectionString);
            }
        }

        private string ToCamelCase(string column)
        {
            return char.ToLowerInvariant(column[0]) + column.Substring(1);
        }
    }
}
