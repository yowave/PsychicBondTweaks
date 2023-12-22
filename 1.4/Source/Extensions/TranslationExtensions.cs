using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PsychicBondTweaks
{
    internal static partial class TranslationExtensions
    {
        public static string PBTranslate(this string key, string searchReplace = "")
        {
            key = $"PBTweaks.{key}";
            StringBuilder translation = new StringBuilder(key.SafeTranslate(searchReplace));

            int line = 0;
            string lineKey = $"{key}_{line}";
            while (lineKey.CanTranslate())
            {
                translation.Append("\n" + lineKey.SmartTranslate(searchReplace));

                line += 1;
                lineKey = $"{key}_{line}";
            }

            if (translation.Length == 0)
            {
                translation.AppendLine(key);
            }

            /*if (key == "tear_bond_days_for_consequences_example")
            {
                Utils.LogM($"{translation}");
            }*/

            return translation.ToString();
        }

        /*public static string PBTranslate(this string key, string searchReplace = "")
        {
            key = $"PBTweaks.{key}";
            string translation = key.SafeTranslate(searchReplace);

            int line = 0;
            string lineKey = $"{key}_{line}";
            while (lineKey.CanTranslate())
            {
                if (translation.Length > 0)
                {
                    translation += "\n";
                }
                translation += lineKey.SmartTranslate(searchReplace);

                line += 1;
                lineKey = $"{key}_{line}";
            }

            if (translation.Length == 0)
            {
                translation = key;
            }

            return translation;
        }*/

        private static string SmartTranslate(this string key, string searchReplace = "")
        {
            string translation = key.Translate();
            translation = translation.SearchReplace(searchReplace);
            //if (translation.Length == 0) translation = "\n";
            return translation;
        }

        private static string SafeTranslate(this string key, string searchReplace)
        {
            string translation = "";
            if (key.CanTranslate())
            {
                translation = key.SmartTranslate(searchReplace);
            }
            return translation;
        }

        private static string SearchReplace(this string str, string searchReplace)
        {
            if (str.Length > 0 && searchReplace.Trim().Length > 0)
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                string[] searchReplaceSplit = searchReplace.Split(',');
                string[] kv;
                foreach (string keyValue in searchReplaceSplit)
                {
                    kv = keyValue.Split('|');
                    if (kv.Length == 2)
                    {
                        keyValuePairs[kv[0]] = kv[1];
                    }
                }

                StringBuilder sb = new StringBuilder();
                StringBuilder currentParam = new StringBuilder();
                int strLength = str.Length;
                string currentParamStr;
                char c;
                for (int i = 0; i < strLength; i++)
                {
                    c = str[i];
                    if (c == ':')
                    {
                        currentParam.Clear().Append(c);
                        for (int k = i + 1; k < strLength; k++)
                        {
                            c = str[k];
                            if (Char.IsLetterOrDigit(c) || c == '_')
                            {
                                currentParam.Append(c);
                            }
                            else
                            {
                                i = k - 1;
                                k = strLength;
                            }

                            if ((k + 1) == strLength)
                            {
                                i = k + 1;
                            }
                        }

                        currentParamStr = currentParam.ToString();
                        if (keyValuePairs.ContainsKey(currentParamStr))
                        {
                            currentParamStr = keyValuePairs[currentParamStr];
                        }
                        sb.Append(currentParamStr);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                return sb.ToString();
            }

            return str;
        }
    }
}
