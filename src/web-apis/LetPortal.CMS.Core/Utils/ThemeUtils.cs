using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.CMS.Core.Annotations;
using LetPortal.CMS.Core.Entities;
using LetPortal.CMS.Core.Exceptions;
using LetPortal.CMS.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace LetPortal.CMS.Core.Utils
{
    public static class ThemeUtils
    {
        /// <summary>
        /// This method allows us to gather all theme manifests in target assembly
        /// </summary>
        /// <param name="target">Theme Assembly</param>
        /// <returns>All available theme manifests</returns>
        public static IEnumerable<ThemeManifest> GatherAllManifests(Assembly target)
        {
            // Ensure they have the implemented theme registration class
            if (target.GetTypes().Any(a => a.GetInterface(typeof(IThemeRegistration).Name) != null))
            {
                var themeManifests = new List<ThemeManifest>();
                var allImplementedManifestTypes = target.GetTypes().Where(a => a.GetInterface(typeof(IThemeManifest).Name) != null);

                foreach (var type in allImplementedManifestTypes)
                {
                    var allProperties = type.GetProperties();
                    foreach (var prop in allProperties)
                    {
                        if ((prop.GetSetMethod() != null || prop.CanWrite) && prop.GetCustomAttribute<NotManifestAttribute>() == null)
                        {
                            if (!themeManifests.Any(a => a.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                themeManifests.Add(new ThemeManifest
                                {
                                    Key = prop.Name,
                                    EditorType = GetManifestEditorType(prop)
                                });
                            }
                        }
                    }
                }

                return themeManifests;
            }

            throw new CMSException(ErrorCodes.TheAssemblyNotIncludeThemeRegistration);
        }


        public static IThemeRegistration GetThemeRegistration(Assembly target)
        {
            var deliveredType = target.GetTypes().First(a => a.GetInterface(typeof(IThemeRegistration).Name) != null);
            return Activator.CreateInstance(deliveredType) as IThemeRegistration;
        }

        public static IEnumerable<ThemePart> GetSectionParts(Assembly target)
        {
            var allViewComponents = target.GetTypes().Where(a => a.BaseType == typeof(ViewComponent));
            var themeSectionParts = new List<ThemePart>();
            foreach (var viewComponent in allViewComponents)
            {
                var themePartAttribute = viewComponent.GetCustomAttribute<ThemePartAttribute>();
                if (themePartAttribute != null)
                {
                    var themSectionPart = new ThemePart
                    {
                        Name = viewComponent.Name.Replace("ViewComponent", ""),
                        ConfigurableManifests = new List<ThemeConfigurableManifest>(),
                        BindingType = themePartAttribute.BindingType
                    };

                    if(themePartAttribute.BindingType != BindingType.None)
                    {
                        var allProperties = themePartAttribute.Type.GetProperties();
                        foreach (var prop in allProperties)
                        {
                            if ((prop.GetSetMethod() != null || prop.CanWrite) && prop.GetCustomAttribute<NotManifestAttribute>() == null)
                            {
                                if (!themSectionPart.ConfigurableManifests.Any(a => a.Key.Equals(prop.Name, StringComparison.OrdinalIgnoreCase)))
                                {
                                    themSectionPart.ConfigurableManifests.Add(new ThemeConfigurableManifest
                                    {
                                        Key = prop.Name,
                                        EditorType = GetManifestEditorType(prop)
                                    });
                                }
                            }
                        }
                    }                       

                    themeSectionParts.Add(themSectionPart);
                }
            }

            return themeSectionParts;
        }

        private static ManifestEditorType GetManifestEditorType(PropertyInfo property)
        {
            if (property.GetCustomAttribute<RichTextEditorAttribute>() != null)
            {
                return ManifestEditorType.RichTextEditor;
            }

            if (property.GetCustomAttribute<EmailEditorAttribute>() != null)
            {
                return ManifestEditorType.Email;
            }

            if (property.GetCustomAttribute<KeyValueListEditorAttribute>() != null)
            {
                return ManifestEditorType.KeyValueEditor;
            }

            if (property.GetCustomAttribute<MediaEditorAttribute>() != null)
            {
                return ManifestEditorType.MediaEditor;
            }

            if (property.GetCustomAttribute<JsonEditorAttribute>() != null)
            {
                return ManifestEditorType.JsonEditor;
            }

            if (property.PropertyType == typeof(LinkModel))
            {
                return ManifestEditorType.LinkEditor;
            }

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(float) || property.PropertyType == typeof(double))
            {
                return ManifestEditorType.Number;
            }

            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(TimeSpan))
            {
                return ManifestEditorType.DatePicker;
            }

            return ManifestEditorType.Textbox;
        }
    }
}
