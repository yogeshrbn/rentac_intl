using BAL.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ReportViewer.Helpers
{
    public static class BillingConfigViewHelper
    {
        public static IEnumerable<ConfigDTO> AsEnumerable(object configData)
        {
            return configData as IEnumerable<ConfigDTO> ?? Enumerable.Empty<ConfigDTO>();
        }

        public static string GetValue(IEnumerable<ConfigDTO> config, string key, string defaultValue = "")
        {
            if (config == null || string.IsNullOrEmpty(key))
                return defaultValue ?? string.Empty;
            var item = config.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
            if (item == null || string.IsNullOrWhiteSpace(item.Value))
                return defaultValue ?? string.Empty;
            return item.Value.Trim();
        }

        /// <summary>Parses 1/0, true/false, yes/no (case-insensitive). Missing/invalid uses defaultValue.</summary>
        public static bool GetBool(IEnumerable<ConfigDTO> config, string key, bool defaultValue = true)
        {
            if (config == null || string.IsNullOrEmpty(key))
                return defaultValue;
            var item = config.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase));
            if (item == null || string.IsNullOrWhiteSpace(Convert.ToString(item.Value)))
                return defaultValue;
            var s = Convert.ToString(item.Value).Trim().ToLowerInvariant();
            if (s == "1" || s == "true" || s == "yes")
                return true;
            if (s == "0" || s == "false" || s == "no")
                return false;
            return defaultValue;
        }

        /// <summary>Normalized: yes | no | ifexists. Invalid/missing returns defaultMode (yes).</summary>
        public static string GetContactShowMode(IEnumerable<ConfigDTO> config, string key, string defaultMode = "yes")
        {
            var v = GetValue(config, key, defaultMode).ToLowerInvariant();
            if (v == "no" || v == "ifexists" || v == "yes")
                return v;
            return "yes";
        }
        public static bool GetGroupItemsOnPrint(IEnumerable<ConfigDTO> config, string key, bool defaultMode = false)
        {
            var v = GetValue(config, key).ToLowerInvariant();
            if (v == "1" || v == "true")
                return true;
            return false;
        }
        /// <summary>yes: always show; no: hide; ifexists: show only when field has a value.</summary>
        public static bool ShouldShowContactField(string mode, object fieldValue)
        {
            var m = (mode ?? "").Trim().ToLowerInvariant();
            if (m == "no")
                return false;
            if (m == "yes")
                return true;
            if (fieldValue == null || fieldValue == DBNull.Value)
                return false;
            return !string.IsNullOrWhiteSpace(Convert.ToString(fieldValue));
        }
        public static bool ShouldShowStringField(string mode, string fieldValue)
        {
            var m = (mode ?? "").Trim().ToLowerInvariant();
            if (m == "no")
                return false;
            if (m == "yes")
                return true;
            return !string.IsNullOrWhiteSpace(Convert.ToString(fieldValue));
        }
    }
}
