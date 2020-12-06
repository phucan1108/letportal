using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Exceptions;
using LetPortal.Core.Utils;

namespace LetPortal.CMS.Core.Services.Pages
{
    public class PageService : IPageService
    {
        public Task<List<Entities.VersionValue>> InitManifests(PageTemplate pageTemplate, Theme chosenTheme)
        {
            if (pageTemplate.Sections?.Any() == true)
            {
                var pageManifests = new List<VersionValue>();
                foreach (var section in pageTemplate.Sections)
                {
                    var themeSection = chosenTheme.SectionParts.FirstOrDefault(a => a.Name == section.ThemePartRef);
                    if (themeSection != null)
                    {
                        foreach (var manifest in themeSection.ConfigurableManifests)
                        {
                            pageManifests.Add(new VersionValue
                            {
                                Key = manifest.Key,
                                Value = null,
                                EditorType = manifest.EditorType
                            });
                        }
                    }
                }

                return Task.FromResult(pageManifests);
            }

            return Task.FromResult(Enumerable.Empty<VersionValue>().ToList());
        }

        public Task<T> LoadModel<T>(T target, string propertyName, ISiteRequestAccessor siteRequest)
        {
            //if (target == null)
            //{
            //    target = (T)Activator.CreateInstance(typeof(T));
            //}
            //// Check it is any available version 
            //if (siteRequest.Current.PageVersion != null)
            //{
            //    var pageVersion = siteRequest.Current.PageVersion;
            //    var targetType = typeof(T);
            //    var allVailableProperies = targetType
            //        .GetProperties()
            //        .Where(a => a.CanWrite && a.GetSetMethod() != null && a.PropertyType.GetCustomAttribute<NotManifestAttribute>() == null)
            //        .ToList();

            //    var allVailableNames = allVailableProperies.Select(a => a.Name);
            //    if (targetType.GetInterface(typeof(IPageManifest).Name) != null)
            //    {
            //        var allFoundPropertyManifests = pageVersion.Manifests.Where(a => a.GroupKey.Equals(propertyName, StringComparison.OrdinalIgnoreCase) && allVailableNames.Contains(a.Key));
            //        foreach (var propertyManifest in allFoundPropertyManifests)
            //        {
            //            var prop = allVailableProperies.First(a => a.Name == propertyManifest.Key);
            //            var foundValue = propertyManifest.ConfigurableValue;
            //            if (!string.IsNullOrEmpty(foundValue))
            //            {
            //                if (prop.PropertyType == typeof(string)
            //                    || prop.PropertyType == typeof(decimal)
            //                    || prop.PropertyType.IsValueType
            //                    || prop.PropertyType.IsEnum)
            //                {
            //                    var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
            //                    var typeConverterObj = typeConverter.ConvertFromString(foundValue);
            //                    prop.SetValue(target, typeConverterObj);
            //                }
            //                else
            //                {
            //                    var convertingObj = ConvertUtil.DeserializeObject(foundValue, prop.PropertyType);
            //                    prop.SetValue(target, convertingObj);
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        throw new CMSException(ErrorCodes.NotImplementPageManifest);
            //    }
            //}

            return Task.FromResult(target);
        }

        public Task<object> LoadModel<T>(string templateKey, ISiteRequestAccessor siteRequest)
        {
            var pageVersion = siteRequest.Current.PageVersion;
            var partVersion = pageVersion.Manifests.First(a => a.TemplateKey == templateKey);
            switch (partVersion.BindingType)
            {
                case BindingType.Object:
                    return Task.FromResult((object)Convert<T>(partVersion.ValuesList.First().Values));
                case BindingType.Array:
                    var list = new List<T>();
                    foreach(var valueList in partVersion.ValuesList)
                    {
                        list.Add(Convert<T>(valueList.Values));
                    }
                    return Task.FromResult((object)list);
                case BindingType.Datasource:
                    return Task.FromResult(default(object));
                default:
                    return Task.FromResult(default(object));
            }
        }

        private T Convert<T>(IEnumerable<VersionValue> versionValues)
        {
            var defaultValue = Activator.CreateInstance<T>();

            var targetType = typeof(T);
            var allVailableProperies = targetType
                .GetProperties()
                .Where(a => a.CanWrite && a.GetSetMethod() != null && a.PropertyType.GetCustomAttribute<NotManifestAttribute>() == null)
                .ToList();
            var allVailableNames = allVailableProperies.Select(a => a.Name);
            foreach (var versionValue in versionValues)
            {
                var prop = allVailableProperies.First(a => a.Name == versionValue.Key);
                var foundValue = versionValue.Value;
                if (!string.IsNullOrEmpty(foundValue))
                {
                    if (prop.PropertyType == typeof(string)
                        || prop.PropertyType == typeof(decimal)
                        || prop.PropertyType.IsValueType
                        || prop.PropertyType.IsEnum)
                    {
                        var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                        var typeConverterObj = typeConverter.ConvertFromString(foundValue);
                        prop.SetValue(defaultValue, typeConverterObj);
                    }
                    else
                    {
                        var convertingObj = ConvertUtil.DeserializeObject(foundValue, prop.PropertyType);
                        prop.SetValue(defaultValue, convertingObj);
                    }
                }
            }
            return defaultValue;
        }
    }
}
