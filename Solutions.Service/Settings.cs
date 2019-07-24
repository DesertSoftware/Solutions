//
//  Copyright 2015,2019, Desert Software Solutions Inc.
//    Settings.cs: 
//      https://github.com/DesertSoftware/Solutions
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Gel;
using DesertSoftware.Solutions.Service.Cron;

public sealed class Settings
{
    private XElement config;

    static private string defaultSettings = "<default frequency='every hour' runOnStartup='yes' />";

    static private string LoadDefaults() { return defaultSettings; }

    static public Settings Load(string configFile, Func<string> defaults = null) {
        var settings = new Settings();

        defaults = defaults ?? LoadDefaults;    // ensure we have some sort of defaults resolver

        try {
            settings.config = System.IO.File.Exists(configFile)
                    ? XElement.Load(configFile)
                    : XElement.Parse(defaults());   // Properties.Resources.Config);
        } catch (Exception ex) {
            Logger.Log("service").Warn("Error loading settings '{0}'", ex.Message);
            Logger.Log("service").Debug(ex);
            settings.config = XElement.Parse(defaultSettings);
        }

        return settings;
    }

    public string Frequency {
        get {
            if (this.config == null) {
                Logger.Log("service").Warn("Configuration file not defined.");
                Logger.Log("service").Debug("Defaulting to HOURLY");
                return CronSchedule.Parse("every hour");
            }

            try {
                if (this.config.HasAttributes && this.config.Attribute("frequency") != null)
                    return CronSchedule.Parse(this.config.Attribute("frequency").Value.Trim());
            } catch (Exception ex) {
                Logger.Log("service").Warn("Error determining frequency '{0}'", ex.Message);
                Logger.Log("service").Debug(ex);
            }

            return CronSchedule.Parse("every hour");
        }
    }

    public bool RunOnStartup {
        get {
            if (this.config == null)
                return false;

            try {
                if (this.config.HasAttributes && this.config.Attribute("runOnStartup") != null) {
                    bool runOnStartup = false;
                    string runOnStartupValue = config.Attribute("runOnStartup").Value.Trim().ToLower()
                        .Replace("yes", "true")
                        .Replace("no", "false");

                    bool.TryParse(runOnStartupValue, out runOnStartup);

                    return runOnStartup;
                }
            } catch (Exception ex) {
                Logger.Log("service").Warn("Error determining RunOnStartup '{0}'", ex.Message);
                Logger.Log("service").Debug(ex);
            }

            return false;
        }
    }
}
