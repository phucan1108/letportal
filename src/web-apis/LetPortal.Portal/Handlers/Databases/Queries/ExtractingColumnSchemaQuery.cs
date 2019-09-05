using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Handlers.Databases.Queries
{
    public class ExtractingColumnSchemaQuery
    {
        public string DatabaseId { get; set; }

        public string QueryJsonString { get; set; }
    }
}
