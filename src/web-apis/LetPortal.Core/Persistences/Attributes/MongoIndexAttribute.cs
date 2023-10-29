using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Core.Persistences.Attributes
{

    [System.AttributeUsage(AttributeTargets.Property)]
    public class MongoIndexAttribute : Attribute
    {
        public string Name { get; set; }

        public MongoIndexType Type { get; set; }
    }

    public enum MongoIndexType
    {
        Asc,
        Desc,
        Text
    }
}
