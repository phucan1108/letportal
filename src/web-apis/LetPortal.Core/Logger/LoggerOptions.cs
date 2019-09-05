using System.Collections.Generic;

namespace LetPortal.Core.Logger
{
    public class LoggerOptions
    {
        public LoggerNotifyOptions NotifyOptions { get; set; }

        public LoggerStorageType Type { get; set; }

        public LoggerMongoOptions MongoOptions { get; set; }
    }

    public class LoggerNotifyOptions
    {
        public bool Enable { get; set; }
        public List<int> StatusCodes { get; set; }
        public List<LoggerNotifyUrl> Urls { get; set; } = new List<LoggerNotifyUrl>();
    }

    public class LoggerNotifyUrl
    {
        public bool Enable { get; set; }
        public string UrlPath { get; set; }

        public List<int> StatusCodes { get; set; }
    }

    public class LoggerMongoOptions
    {
        public string ConnectionString { get; set; }

        public string CollectionName { get; set; }
    }

    public enum LoggerStorageType
    {
        Mongodb
    }
}
