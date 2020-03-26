using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Entities.Shared
{
    public class DatabaseExecutionChains
    {
        public List<DatabaseExecutionStep> Steps { get; set; }
    }

    public class DatabaseExecutionStep
    {
        public string DataLoopKey { get; set; }

        public string DatabaseConnectionId { get; set; }

        public string ExecuteCommand { get; set; }
    }
}
