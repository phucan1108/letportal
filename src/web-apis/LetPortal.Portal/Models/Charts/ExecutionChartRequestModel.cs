﻿using LetPortal.Portal.Entities.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace LetPortal.Portal.Models.Charts
{
    public class ExecutionChartRequestModel
    {
        public string ChartId { get; set; }

        public List<ChartParameterValue> ChartParameterValues { get; set; }

        public List<ChartFilterValue> ChartFilterValues { get; set; }
    }

    public class ChartParameterValue
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public bool ReplaceDQuotes { get; set; }
    }

    public class ChartFilterValue
    {
        public string Name { get; set; }

        public FilterType FilterType { get; set; }

        public bool IsMultiple { get; set; }

        public string Value { get; set; }
    }
}