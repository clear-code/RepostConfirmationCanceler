﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepostConfirmationCanceler
{
    internal class Section
    {
        public string Name;
        public bool Disabled = false;
        public bool WarningWhenCloseDialog = false;
        public StringBuilder ExcludeGroups = new StringBuilder();
        public StringBuilder Excludes = new StringBuilder();
        public StringBuilder Patterns = new StringBuilder();

        public Section(string name)
        {
            Name = name;
        }
    }

    internal class Config
    {
        public bool IgnoreQueryString = false;
        public bool OnlyMainFrame = false;
        public List<Section> SectionList = new List<Section>();

        public Section GetEdgeSection()
        {
            return SectionList.FirstOrDefault(_ => _.Name.ToLowerInvariant() == "[edge]");
        }
    }

    internal static class ConfigLoader
    {
        internal static Config LoadConfig()
        {
            var ruleFilePath = GetRulefilePath();
            if (string.IsNullOrEmpty(ruleFilePath))
            {
                Console.Error.WriteLine("Rulefile path is not set in the registry.");
                return new Config();
            }
            using (var fileStream = new FileStream(ruleFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 1024, true))
            {   
                string data = streamReader.ReadToEnd();
                return ParseConf(data);
            }
        }

        internal static string GetRulefilePath()
        {
            const string registryPath = @"SOFTWARE\RepostConfirmationCanceler";
            const string valueName = "Rulefile";
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryPath))
                {
                    if (key == null)
                    {
                        Console.Error.WriteLine($"cannot read {registryPath}: key not found");
                        return null;
                    }
                    object value = key.GetValue(valueName);
                    if (value is string rulefile)
                    {
                        return rulefile;
                    }
                    else
                    {
                        Console.Error.WriteLine($"cannot read {registryPath}: 'Rulefile' not found or not string");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"cannot read {registryPath}: {ex.Message}");
                return null;
            }
        }

        internal static Config ParseConf(string data)
        {
            var conf = new Config();
            bool global = false;
            Section section = null;

            var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (var rawLine in lines)
            {
                string line = rawLine.Trim();
                if (string.IsNullOrEmpty(line))
                    continue;

                switch (line[0])
                {
                    case ';':
                    case '#':
                        // コメント行
                        break;
                    case '[':
                        global = false;
                        if (line == "[GLOBAL]")
                        {
                            global = true;
                        }
                        else
                        {
                            // 新しいセクション追加
                            section = new Section(line);
                            conf.SectionList.Add(section);
                        }
                        break;
                    case '@':
                        if (global)
                        {
                            if (line == "@TOP_PAGE_ONLY")
                            {
                                conf.IgnoreQueryString = true;
                            }
                            else if (line == "@ONLY_MAIN_FRAME")
                            {
                                conf.OnlyMainFrame = true;
                            }
                        }
                        else if (section != null)
                        {
                            if (line == "@DISABLED")
                            {
                                section.Disabled = true;
                            }
                            else if (line == "@WARNING_WHEN_CLOSE_DIALOG")
                            {
                                section.WarningWhenCloseDialog = true;
                            }
                            else if (line.StartsWith("@EXCLUDE_GROUP:"))
                            {
                                section.ExcludeGroups.Append(line.Substring("@EXCLUDE_GROUP:".Length));
                                section.ExcludeGroups.Append('\n');
                            }
                        }
                        break;
                    case '-':
                        if (section != null && !line.StartsWith("-#"))
                        {
                            section.Excludes.AppendLine(line);
                        }
                        break;
                    default:
                        if (section != null)
                        {
                            section.Patterns.AppendLine(line);
                        }
                        break;
                }
            }
            return conf;
        }
    }
}
