using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Repositories.Sites;
using LetPortal.CMS.Core.Repositories.Themes;
using LetPortal.Core.Utils;

namespace LetPortal.CMS.Core.Providers
{
    public class ThemeProvider : IThemeProvider
    {
        private readonly ISiteManifestRepository _siteManifestRepository;

        private readonly IThemeRepository _themeRepository;

        public ThemeProvider(
            ISiteManifestRepository siteManifestRepository,
            IThemeRepository themeRepository)
        {
            _siteManifestRepository = siteManifestRepository;
            _themeRepository = themeRepository;
        }

        public async Task<T> LoadAsync<T>(T target, string siteId)
        {
            var typeOfT = typeof(T);

            if (target == null)
            {
                target = (T)Activator.CreateInstance(typeof(T));
            }

            if (target is IThemeManifest)
            {
                var properties = typeOfT.GetProperties();
                var list = new List<Tuple<string, object, PropertyInfo>>();
                foreach (var prop in properties)
                {
                    // Ensure this prop has Setter method
                    if (prop.GetSetMethod() != null
                        && prop.GetCustomAttribute<NotManifestAttribute>() == null)
                    {
                        var key = prop.Name;
                        var nameAttribute = prop.GetCustomAttribute<ManifestNameAttribute>();
                        if (nameAttribute != null)
                        {
                            key = nameAttribute.GetName();
                        }

                        list.Add(new Tuple<string, object, PropertyInfo>(key, null, prop));
                    }
                }

                if (list.Count > 0)
                {
                    var siteManifests = await _siteManifestRepository.GetSiteManifestsAsync(list.Select(a => a.Item1), siteId);

                    foreach (var siteManifest in siteManifests)
                    {
                        var propReflector = list.First(a => a.Item1 == siteManifest.Key);
                        var foundValue = siteManifest.ConfigurableValue;
                        var prop = propReflector.Item3;
                        if (foundValue != null)
                        {
                            if (prop.PropertyType == typeof(string)
                                || prop.PropertyType == typeof(decimal) 
                                || prop.PropertyType.IsValueType 
                                || prop.PropertyType.IsEnum)
                            {
                                var typeConverter = TypeDescriptor.GetConverter(prop.PropertyType);
                                var typeConverterObj = typeConverter.ConvertFromString(foundValue);
                                prop.SetValue(target, typeConverterObj);
                            }
                            else
                            {
                                var convertingObj = ConvertUtil.DeserializeObject(foundValue, prop.PropertyType);
                                prop.SetValue(target, convertingObj);
                            }

                        }
                    }
                }

                return target;
            }

            throw new NotImplementedException();
        }

        public async Task<Theme> LoadTheme(string themeId)
        {
            return await _themeRepository.GetOneAsync(themeId);
        }
    }
}
