﻿using LetPortal.Core.Persistences;
using LetPortal.Portal.Entities.SectionParts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetPortal.Portal.Providers.Components
{
    public interface IStandardServiceProvider
    {
        Task<IEnumerable<StandardComponent>> GetStandardComponentsByIds(IEnumerable<string> ids);

        Task<IEnumerable<ComparisonResult>> CompareStandardComponent(IEnumerable<StandardComponent> standardComponents);
    }
}