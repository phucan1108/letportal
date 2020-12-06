using System.Collections.Generic;
using LetPortal.Core.Persistences;
using LetPortal.Core.Persistences.Attributes;

namespace LetPortal.CMS.Core.Entities
{
    [EntityCollection(Name = "modules")]
    public class Module : Entity
    {
        /// <summary>
        /// It represents a set of view components. 
        /// Each theme must contain all RequiredSections
        /// For example: BlogModule requires 'BlogDetail', 'BlogList'
        /// Basically LetPortal doesn't offer Liquid template (execute at runtime)
        /// So we force a Theme to wrap all logic and UI itself
        /// </summary>
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// It is a name of ViewComponent that one Theme must be implemented
        /// </summary>
        public List<string> RequiredSections { get; set; }
    }
}
