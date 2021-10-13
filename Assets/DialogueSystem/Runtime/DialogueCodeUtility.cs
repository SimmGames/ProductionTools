using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DialogueSystem.Code
{
    public static class DialogueCodeUtility
    {
        public static string GenerateFunctionName(string dialogueName, string nodeGuid, string portGuid = "")
        {
            return $"{SanitizeName(dialogueName)}_{nodeGuid.Replace("-", "")}{(string.IsNullOrEmpty(portGuid) ? string.Empty : ("_" + portGuid.Replace("-", "")))}";
        }

        public static string SanitizeName(string name)
        {
            name = name.Trim();
            // Replace non alphanumerics with an '_'
            name = Regex.Replace(name, @"([^0-9a-zA-Z_])+", "_");
            // Make sure the begining isn't a number or underscore
            name = Regex.Replace(name, @"^([0-9_])+", string.Empty);
            return name;
        }
    }
}