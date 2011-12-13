using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PirateSpadesGame.Settings {
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Xml;

    public static class PirateSettings {
        private const string File = "piratesettings.xml";

        private static Dictionary<string, string> settings;

        public static string GetString(string key) {
            if(settings == null) LoadSettings();
            if(settings != null && settings.ContainsKey(key)) return settings[key];
            return string.Empty;
        }

        public static float GetFloat(string key) {
            var f = 0.00f;
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            float.TryParse(GetString(key).Replace(",","."),NumberStyles.Float, culture, out f);
            return f;
        }

        public static void Set(string key, object value) {
            if(settings == null) {
                LoadSettings();
                Set(key, value);
            }else {
                settings[key] = value.ToString();
            }
        }

        public static void Save() {
            var x = new XmlDocument();
            x.AppendChild(x.CreateXmlDeclaration("1.0", "utf-8", null));

            var xSettings = x.CreateElement("settings");
            foreach(var kvp in settings) {
                var xSetting = x.CreateElement("setting");
                
                var xName = x.CreateElement("name");
                xName.InnerText = kvp.Key;

                var xValue = x.CreateElement("value");
                xValue.InnerText = kvp.Value;

                xSetting.AppendChild(xName);
                xSetting.AppendChild(xValue);

                xSettings.AppendChild(xSetting);
            }
            x.AppendChild(xSettings);

            x.Save(File);
        }

        private static void LoadSettings() {
            Contract.Ensures(settings != null);
            var d = new Dictionary<string, string>();
            var x = new XmlDocument();
            try {
                x.Load(File);
                var xSettings = x.SelectNodes("settings/setting");
                if(xSettings != null) {
                    foreach(XmlNode xSetting in xSettings) {
                        var key = xSetting.SelectSingleNode("name");
                        var value = xSetting.SelectSingleNode("value");

                        if(key != null && value != null) {
                            d[key.InnerText] = value.InnerText;
                        }
                    }
                }
            } catch(Exception ex) {
                Console.WriteLine(ex);
            }
            settings = d;
        }
    }
}
