// <copyright file="PirateSettings.cs">
//      ahal@itu.dk
// </copyright>
// <summary>
//      A simplistic implementation of a settings controller.
// </summary>
// <author>Andreas Hallberg Kjeldsen (ahal@itu.dk)</author>

namespace PirateSpadesGame.Settings {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Xml;

    /// <summary>
    ///  A simplistic implementation of a settings controller.
    /// </summary>
    public static class PirateSettings {
        /// <summary>
        /// The name of the file to load/save settings from/to.
        /// </summary>
        private const string File = "piratesettings.xml";

        /// <summary>
        /// A dictionary containing the settings loaded.
        /// </summary>
        private static Dictionary<string, string> settings;

        /// <summary>
        /// Get the string value of a setting with the given key as the name.
        /// </summary>
        /// <param name="key">The name of the setting.</param>
        /// <returns>The value for the specified key or an empty string if the key is invalid.</returns>
        public static string GetString(string key) {
            if(settings == null) LoadSettings();
            if(settings != null && settings.ContainsKey(key)) return settings[key];
            return string.Empty;
        }

        /// <summary>
        /// Get the float value of a setting with the given key as the name.
        /// </summary>
        /// <param name="key">The name of the setting.</param>
        /// <returns>The value parsed as a float or 0.00 if parsing wasn't possible.</returns>
        public static float GetFloat(string key) {
            var f = 0.00f;
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
            float.TryParse(GetString(key).Replace(",","."),NumberStyles.Float, culture, out f);
            return f;
        }

        /// <summary>
        /// Set the value of the setting with key as its name.
        /// </summary>
        /// <param name="key">The name of the setting.</param>
        /// <param name="value">The value to set.</param>
        public static void Set(string key, object value) {
            if(settings == null) {
                LoadSettings();
                Set(key, value);
            }else {
                settings[key] = value.ToString();
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
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

        /// <summary>
        /// Load the settings.
        /// </summary>
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
