using System;
using System.Collections.Generic;
using System.Text;
using LetPortal.Core.Utils;
using LetPortal.Portal.Entities.SectionParts.Controls;

namespace LetPortal.Portal.Extensions
{
    public static class PageControlExtension
    {
        public static void HideSensitive(this PageControl control)
        {
            if (control.AsyncValidators != null && control.AsyncValidators.Count > 0)
            {
                foreach (var validator in control.AsyncValidators)
                {
                    if (validator.AsyncValidatorOptions.ValidatorType == Entities.Components.Controls.AsyncValidatorType.DatabaseValidator)
                    {
                        validator.AsyncValidatorOptions.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(validator.AsyncValidatorOptions.DatabaseOptions.Query, true));
                    }
                }
            }

            if (control.Type == Entities.SectionParts.Controls.ControlType.Select
                || control.Type == Entities.SectionParts.Controls.ControlType.AutoComplete)
            {
                if (control.DatasourceOptions.Type == Entities.Shared.DatasourceControlType.Database)
                {
                    control.DatasourceOptions.DatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(control.DatasourceOptions.DatabaseOptions.Query, true));
                }
            }

            foreach (var controlEvent in control.PageControlEvents)
            {
                if (controlEvent.EventActionType == Entities.Components.Controls.EventActionType.QueryDatabase)
                {
                    controlEvent.EventDatabaseOptions.Query = string.Join(';', StringUtil.GetAllDoubleCurlyBraces(controlEvent.EventDatabaseOptions.Query, true));
                }
            }
        }
    }
}
