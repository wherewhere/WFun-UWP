using System.Collections.Generic;

namespace WFunUWP.Models.Html
{
    internal static class DictionaryExtensions
    {
        public static string GetValue(this Dictionary<string, string> dict, string attrName)
        {
            return dict.ContainsKey(attrName) ? dict[attrName] : null;
        }

        public static int GetValueInt(this Dictionary<string, string> dict, string attrName)
        {
            string value = dict.GetValue(attrName);
            if (!string.IsNullOrWhiteSpace(value))
            {
                if (int.TryParse(value, out int result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}
