using LetPortal.Core.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LetPortal.ServiceManagementApis.ConfigurationProviders
{
    public class ServicePerDirectoryConfigurationProvider : ConfigurationProvider
    {
        private readonly string _directoryPath;

        private readonly string _currentEnvironment;

        public ServicePerDirectoryConfigurationProvider(string directoryPath, string env)
        {
            _directoryPath = directoryPath;
            _currentEnvironment = env;
        }

        public override void Load()
        {
            Console.WriteLine("Start getting all files");
            var keyValues = GetAllAvailableConfigurations(_directoryPath, _currentEnvironment);
            foreach(var keyValue in keyValues)
            {
                Data.Add(keyValue.Key, keyValue.Value);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetAllAvailableConfigurations(string directoryPath, string environment)
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

            foreach(var a in filesList)
            {     
                Console.WriteLine("Files List: " + a.File.FullName);
            }
            var serviceConfigs = from file in filesList
                                 group file by file.Key into mergingFile
                                 select new
                                 {
                                     mergingFile.First().ServiceName,
                                     Kvp = MergeJsonDataOfFiles(mergingFile.Key, mergingFile.Select(a => a.File).OrderBy(a => a.Name.Length))
                                 };

            var versionedConfigs = from versionConfig in serviceConfigs
                                   group versionConfig by versionConfig.ServiceName into mergeConfig
                                   select MergeJsonDataByVersions(mergeConfig.Select(con => con.Kvp));

            Console.WriteLine("all versioned files: " + ConvertUtil.SerializeObject(versionedConfigs.ToList()));
            return versionedConfigs.SelectMany(a => a);
        }

        /// <summary>
        /// File Name should be follow the pattern {fileName}.{env}.json, env is optional
        /// Env should be equal current env
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        private static bool ValidFileName(FileInfo fileInfo, string environment)
        {
            var arrays = fileInfo.Name.Split(".");
            return arrays.Length > 2 && arrays.Length <= 3 ? arrays[1].Equals(environment, StringComparison.CurrentCultureIgnoreCase) : true;
        }

        private static KeyValuePair<string, string> MergeJsonDataOfFiles(string fileKey, IEnumerable<FileInfo> mergingFiles)
        {
            JObject firstObject = JObject.Parse("{}");

            foreach(var file in mergingFiles)
            {
                JObject parsedObject = JObject.Parse(File.ReadAllText(file.FullName));

                firstObject.Merge(parsedObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
            }

            return new KeyValuePair<string, string>(fileKey, ConvertUtil.SerializeObject(firstObject));
        }

        private static IEnumerable<KeyValuePair<string, string>> MergeJsonDataByVersions(IEnumerable<KeyValuePair<string, string>> versionsKeyValues)
        {
            var sortedByKeys = versionsKeyValues.OrderBy(a => a.Key).ToList();

            var cloningKeyValues = new List<KeyValuePair<string, string>>();

            for(int i = 0; i < sortedByKeys.Count; i++)
            {
                var jsonObject = JObject.Parse(sortedByKeys[i].Value);

                for(int j = 0; j < i; j++)
                {
                    var tempJsonObject = JObject.Parse(sortedByKeys[j].Value);
                    tempJsonObject.Merge(jsonObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
                    jsonObject = tempJsonObject;
                }

                cloningKeyValues.Add(new KeyValuePair<string, string>(sortedByKeys[i].Key, ConvertUtil.SerializeObject(jsonObject)));
            }

            return cloningKeyValues;
        }
    }
}
