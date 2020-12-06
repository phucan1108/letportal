using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace LetPortal.CMS.Core.Routers
{
    class ThemePageRouteModelConvention : IPageRouteModelConvention
    {
        private readonly string _areaName;

        private readonly string _pageName;

        private readonly Action<PageRouteModel> _action;

        public ThemePageRouteModelConvention(
            string areaName,
            string pageName,
            Action<PageRouteModel> action)
        {
            _areaName = areaName;
            _pageName = pageName;
            _action = action;
        }
        public void Apply(PageRouteModel model)
        {
            if (string.Equals(_areaName, model.AreaName, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(model.ViewEnginePath, _pageName, StringComparison.OrdinalIgnoreCase))
            {
                _action(model);
            }
        }
    }
}
