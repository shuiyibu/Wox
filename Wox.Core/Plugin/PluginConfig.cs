﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Wox.Infrastructure.Exception;
using Wox.Infrastructure.Logger;
using Wox.Plugin;

namespace Wox.Core.Plugin
{

    internal abstract class PluginConfig
    {
        private const string pluginConfigName = "plugin.json";
        private static List<PluginMetadata> pluginMetadatas = new List<PluginMetadata>();

        /// <summary>
        /// Parse plugin metadata in giving directories
        /// </summary>
        /// <param name="pluginDirectories"></param>
        /// <returns></returns>
        public static List<PluginMetadata> Parse(string[] pluginDirectories)
        {
            pluginMetadatas.Clear();
            foreach (string pluginDirectory in pluginDirectories)
            {
                ParsePluginConfigs(pluginDirectory);
            }

            return pluginMetadatas;
        }

        private static void ParsePluginConfigs(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory)) return;

            string[] directories = Directory.GetDirectories(pluginDirectory);
            foreach (string directory in directories)
            {
                if (File.Exists((Path.Combine(directory, "NeedDelete.txt"))))
                {
                    try
                    {
                        Directory.Delete(directory, true);
                        continue;
                    }
                    catch (Exception e)
                    {
                        Log.Fatal(e);
                    }
                }
                PluginMetadata metadata = GetPluginMetadata(directory);
                if (metadata != null)
                {
                    pluginMetadatas.Add(metadata);
                }
            }
        }

        private static PluginMetadata GetPluginMetadata(string pluginDirectory)
        {
            string configPath = Path.Combine(pluginDirectory, pluginConfigName);
            if (!File.Exists(configPath))
            {
                Log.Warn($"parse plugin {configPath} failed: didn't find config file.");
                return null;
            }

            PluginMetadata metadata;
            try
            {
                metadata = JsonConvert.DeserializeObject<PluginMetadata>(File.ReadAllText(configPath));
                metadata.PluginDirectory = pluginDirectory;
                // for plugins which doesn't has ActionKeywords key
                metadata.ActionKeywords = metadata.ActionKeywords ?? new List<string> { metadata.ActionKeyword };
                // for plugin still use old ActionKeyword
                metadata.ActionKeyword = metadata.ActionKeywords?[0];
            }
            catch (Exception e)
            {
                string msg = $"Parse plugin config {configPath} failed: json format is not valid";
                Log.Error(new WoxException(msg));
                return null;
            }


            if (!AllowedLanguage.IsAllowed(metadata.Language))
            {
                string msg = $"Parse plugin config {configPath} failed: invalid language {metadata.Language}";
                Log.Error(new WoxException(msg));
                return null;
            }

            if (!File.Exists(metadata.ExecuteFilePath))
            {
                string msg = $"Parse plugin config {configPath} failed: ExecuteFile {metadata.ExecuteFilePath} didn't exist";
                Log.Error(new WoxException(msg));
                return null;
            }

            return metadata;
        }
    }
}