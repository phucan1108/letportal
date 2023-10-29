using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Core.Persistences.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property)]
    public class MongoTTLIndexAttribute : Attribute
    {
        public int TTLSeconds { get; set; }
    }
}
