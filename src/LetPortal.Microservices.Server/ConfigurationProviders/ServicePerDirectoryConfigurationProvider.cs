using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LetPortal.Core.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace LetPortal.Microservices.Server.ConfigurationProviders
{
    public class ServicePerDirectoryConfigurationProvider : ConfigurationProvider
    {
        private readonly string _directoryPath;

        private readonly string _currentEnvironment;

        private readonly string _sharedFolder;

        private readonly string _ignoreCombineSharedServices;

        public ServicePerDirectoryConfigurationProvider(
            string directoryPath,
            string env,
            string sharedFolder,
            string ignoreCombineSharedServices)
        {
            _directoryPath = directoryPath;
            _currentEnvironment = env;
            _sharedFolder = sharedFolder;
            _ignoreCombineSharedServices = ignoreCombineSharedServices;
        }

        public override void Load()
        {
            Console.WriteLine("Start getting all files");
            var keyValues = GetAllAvailableConfigurations(
                _directoryPath,
                _currentEnvironment,
                _sharedFolder,
                _ignoreCombineSharedServices);
            foreach (var keyValue in keyValues)
            {
                Data.Add(keyValue.Key, keyValue.Value);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetAllAvailableConfigurations(
            string directoryPath,
            string environment,
            string sharedFolder,
            string ignoreCombinedServices)
        {
            var filesList = from serviceDirectory in new DirectoryInfo(directoryPath).EnumerateDirectories()
                            from versionDirectory in serviceDirectory.EnumerateDirectories()
                            from file in versionDirectory.EnumerateFiles("*.json")
                            where ValidFileName(file, environment)
                            select new
                            {
                                ServiceName = serviceDirectory.Name,
                                Key = serviceDirectory.Name + ":" + versionDirectory.Name,
                                File = file
                            };

            var serviceConfigs = from file in filesList
                                 group file by file.Key into mergingFile
                                 select new
                                 {
                                     mergingFile.First().ServiceName,
                                     Kvp = MergeJsonDataOfFiles(mergingFile.Key, mergingFile.Select(a => a.File).OrderBy(a => a.Name.Length))
                                 };

            var groupedByServiceName = from versionConfig in serviceConfigs
                                       group versionConfig by versionConfig.ServiceName;
            var allSharedConfigs = groupedByServiceName.Where(a => a.Key.Equals(sharedFolder, StringComparison.OrdinalIgnoreCase)).ToList();
            groupedByServiceName = groupedByServiceName.Where(a => !a.Key.Equals(sharedFolder, StringComparison.OrdinalIgnoreCase)).ToList();


            var versionedConfigs = from mergeConfig in groupedByServiceName
                                   select MergeJsonDataByVersions(
                                       mergeConfig.Key,
                                       sharedFolder,
                                       ignoreCombinedServices.Split(";").ToList(),
                                       allSharedConfigs.First().Select(a => a.Kvp),
                                       mergeConfig.Select(con => con.Kvp));

            // Enhancement for Shared
            var returnedConfigs = versionedConfigs.SelectMany(a => a);

            return returnedConfigs;
        }

        /// <summary>
        /// File Name should be follow the pattern {fileName}.{env}.json, env is optional
        /// Env should be equal current env
        /// We accept only {fileName}.json and {fileName}.{env}.json
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static bool ValidFileName(FileInfo fileInfo, string environment)
        {
            var arrays = fileInfo.Name.Split(".");
            return arrays.Length == 3 ?
                arrays[1].Equals(environment, StringComparison.CurrentCultureIgnoreCase) :
                arrays.Length == 2;
        }

        /// <summary>
        /// Merge all json data based on fileVersionKey. Ex: fileVersionKey = Portal:v1.0
        /// </summary>
        /// <param name="fileVersionKey"></param>
        /// <param name="mergingFiles"></param>
        /// <returns></returns>
        private static KeyValuePair<string, string> MergeJsonDataOfFiles(
            string fileVersionKey,
            IEnumerable<FileInfo> mergingFiles)
        {
            var firstObject = JObject.Parse("{}");

            foreach (var file in mergingFiles)
            {
                var parsedObject = JObject.Parse(File.ReadAllText(file.FullName));

                firstObject.Merge(parsedObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
            }

            return new KeyValuePair<string, string>(fileVersionKey, ConvertUtil.SerializeObject(firstObject));
        }

        private static IEnumerable<KeyValuePair<string, string>> MergeJsonDataByVersions(
            string serviceName,
            string sharedFolder,
            IEnumerable<string> ignoreServices,
            IEnumerable<KeyValuePair<string, string>> sharedKeyValues,
            IEnumerable<KeyValuePair<string, string>> versionsKeyValues)
        {
            List<KeyValuePair<string, string>> sortedByKeys = new List<KeyValuePair<string, string>>();
            if (!ignoreServices.Any(a => a == serviceName))
            {
                // Sorted Key such as Portal:v1.0 Portal:v1.1
                // We need to append shared values before any
                var sortedBySharedKeys = sharedKeyValues
                    .OrderBy(a => a.Key)
                    .Select(b => new KeyValuePair<string, string>(b.Key.Replace(sharedFolder, serviceName, StringComparison.OrdinalIgnoreCase), b.Value))
                    .ToList();

                // We will have "Portal:v1.0" from Shared and "Portal:v1.0" from original
                // We need to group and then merging two json
                sortedByKeys = versionsKeyValues
                    .Union(sortedBySharedKeys)
                    .OrderBy(a => a.Key)
                    .GroupBy(b => b.Key)
                    .Select(c =>
                        new KeyValuePair<string, string>(c.Key, MergeMultipleKvp(c.Select(d => d)))
                    )
                    .ToList();
            }
            else
            {
                sortedByKeys = versionsKeyValues.OrderBy(a => a.Key).ToList();
            }

            var cloningKeyValues = new List<KeyValuePair<string, string>>();

            for (var i = 0; i < sortedByKeys.Count; i++)
            {
                var jsonObject = JObject.Parse(sortedByKeys[i].Value);

                for (var j = 0; j < i; j++)
                {
                    var tempJsonObject = JObject.Parse(sortedByKeys[j].Value);
                    tempJsonObject.Merge(jsonObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
                    jsonObject = tempJsonObject;
                }

                cloningKeyValues.Add(new KeyValuePair<string, string>(sortedByKeys[i].Key, ConvertUtil.SerializeObject(jsonObject)));
            }

            return cloningKeyValues;
        }

        private static string MergeMultipleKvp(IEnumerable<KeyValuePair<string, string>> pairs)
        {
            var firstObject = JObject.Parse("{}");

            foreach (var pair in pairs)
            {
                var parsedObject = JObject.Parse(pair.Value);

                firstObject.Merge(parsedObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Merge });
            }

            return ConvertUtil.SerializeObject(firstObject);
        }
    }
}
