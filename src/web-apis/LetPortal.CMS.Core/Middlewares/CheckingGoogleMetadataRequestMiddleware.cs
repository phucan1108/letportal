using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LetPortal.CMS.Core.Abstractions;
using LetPortal.Core.Utils;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace LetPortal.CMS.Core.Middlewares
{
    public class CheckingGoogleMetadataRequestMiddleware : IMiddleware
    {
        private readonly ISiteRequestAccessor _siteRequest;

        public CheckingGoogleMetadataRequestMiddleware(ISiteRequestAccessor siteRequest)
        {
            _siteRequest = siteRequest;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var currentPage = _siteRequest.Current.Page;

            try
            {
                if (currentPage.GoogleMetadata != null)
                {
                    var titleCurlyBrackets = StringUtil.GetAllDoubleCurlyBraces(currentPage.GoogleMetadata.Title, false);
                    if (titleCurlyBrackets.Length > 0)
                    {
                        foreach (var value in titleCurlyBrackets)
                        {
                            var splited = value.Split(".");
                            var foundResolveData = _siteRequest.Current.ResolvedData[splited[0]];

                            var contractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                            };

                            var jObject = JObject.FromObject(foundResolveData, new JsonSerializer
                            {
                                ContractResolver = contractResolver,
                                Formatting = Formatting.Indented
                            });
                            var selectValue = (string)jObject.SelectToken(value.Replace(splited[0] + ".", ""));

                            currentPage.GoogleMetadata.Title = currentPage.GoogleMetadata.Title.Replace("{{" + value + "}}", selectValue);
                        }
                    }

                    var descriptionCurlyBrackets = StringUtil.GetAllDoubleCurlyBraces(currentPage.GoogleMetadata.Description, false);
                    if (descriptionCurlyBrackets.Length > 0)
                    {
                        foreach (var value in descriptionCurlyBrackets)
                        {
                            var splited = value.Split(".");
                            var foundResolveData = _siteRequest.Current.ResolvedData[splited[0]];

                            var contractResolver = new DefaultContractResolver
                            {
                                NamingStrategy = new CamelCaseNamingStrategy()
                            };

                            var jObject = JObject.FromObject(foundResolveData, new JsonSerializer
                            {
                                ContractResolver = contractResolver,
                                Formatting = Formatting.Indented
                            });
                            var selectValue = (string)jObject.SelectToken(value.Replace(splited[0] + ".", ""));

                            currentPage.GoogleMetadata.Description = currentPage.GoogleMetadata.Description.Replace("{{" + value + "}}", selectValue);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }            

            await next.Invoke(context);
        }
    }
}
