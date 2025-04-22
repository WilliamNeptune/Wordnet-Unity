using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;
using TMPro;
using DG.Tweening;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
#if LC_SDK_IAP
using UnityEngine.Purchasing;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Threading;
#endif
using Debug = UnityEngine.Debug;

public static class Functions
{

    // #if UNITY_EDITOR
    private static List<String> langArray = new List<string>();
    // #else
    //     private static String[] langArray;
    // #endif

    private static Dictionary<string, int> strDict = new Dictionary<string, int>();

    //     public static void loadLangPackage(string strFileName)
    //     {
    //         // 1
    // #if UNITY_EDITOR
    //         langArray.Clear();
    //         string path = string.Format("{0}/Assets/Multi_Lang/{1}.txt", Application.dataPath, strFileName);
    //         StreamReader sr = new StreamReader(path, Encoding.Default);
    //         String line;
    //         while ((line = sr.ReadLine()) != null)
    //         {
    //             langArray.Add(line.ToString());
    //         }
    //         sr.Close();
    // #else
    //         string path = string.Format("Assets/Multi_Lang/{0}.txt", strFileName);
    //         AsyncLoadResult result = LuaHelper.GetResManager().SyncLoadAsset(path);
    //         var textAsset = result.m_oAssetObject as TextAsset;
    //         char[] splitter = new char[] {'\r', '\n'};
    //         langArray = textAsset.text.Split(splitter, StringSplitOptions.RemoveEmptyEntries); 
    // #endif

    //         // 2
    //         strDict.Clear();
    // #if UNITY_EDITOR
    //         path = string.Format("{0}/Assets/Multi_Lang/{1}.txt", Application.dataPath, "StringEnums");
    //         sr = new StreamReader(path, Encoding.Default);
    //         int i = 0;
    //         while ((line = sr.ReadLine()) != null)
    //         {
    //             strDict.Add(line.ToString(), i++);
    //         }
    //         sr.Close();
    // #else
    //         path = string.Format("Assets/Multi_Lang/{0}.txt", "StringEnums");
    //         result = LuaHelper.GetResManager().SyncLoadAsset(path);
    //         textAsset = result.m_oAssetObject as TextAsset;
    //         var strArray = textAsset.text.Split(splitter, StringSplitOptions.RemoveEmptyEntries); 
    //         for (int i = 0; i < strArray.Length; ++i)
    //         {
    //             strDict.Add(strArray[i], i);
    //         }
    // #endif
    //     }

    private static Dictionary<string, string> langDict = new Dictionary<string, string>();
    private static Dictionary<string, int> strEnumDict = new Dictionary<string, int>();
    public static event Action OnColorModeChanged;
    public static event Action OnLangChanged;
    public static void InvokeColorModeChanged()
    {
        OnColorModeChanged?.Invoke();
    }
    // public static void loadLangPackage(int langIndex)
    // {
    //     langDict.Clear();
    //     strEnumDict.Clear();
    //     langArray.Clear();

    //     // Load JSON file
    //     string jsonPath = Path.Combine(Application.dataPath, "Resources/all_lang.json");
    //     string jsonContent = File.ReadAllText(jsonPath);
    //     JObject jsonObj = JObject.Parse(jsonContent);

    //     // Convert langIndex to string (1-based index for JSON)
    //     string langCode = (langIndex + 1).ToString();

    //     // Parse JSON content
    //     var languageData = jsonObj["language"] as JObject;
    //     int index = 0;
    //     foreach (var item in languageData)
    //     {
    //         string key = item.Key;
    //         string value = item.Value["_descSid"][langCode]?.ToString() ?? "";

    //         if (!string.IsNullOrEmpty(value))
    //         {
    //             langDict[key] = value;
    //             strEnumDict[key] = index;
    //             langArray.Add(value);
    //             index++;
    //         }
    //     }
    // }

    public static void InvokeLangChanged()
    {
        OnLangChanged?.Invoke();
    }

    public static void loadLangPackage(int langIndex)
    {
        langDict.Clear();
        strEnumDict.Clear();
        langArray.Clear();

        // Load JSON file using Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("all_lang");
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found!");
            return;
        }

        // Parse JSON content
        JObject jsonObj = JObject.Parse(jsonFile.text);

        // Convert langIndex to string (1-based index for JSON)
        string langCode = (langIndex + 1).ToString();

        // Parse JSON content
        var languageData = jsonObj["language"] as JObject;
        int index = 0;
        foreach (var item in languageData)
        {
            string key = item.Key;
            string value = item.Value["_descSid"][langCode]?.ToString() ?? "";

            if (!string.IsNullOrEmpty(value))
            {
                langDict[key] = value;
                strEnumDict[key] = index;
                langArray.Add(value);
                index++;
            }
        }
    }

    public static string getTextStr(int index)
    {
        return index >= 0 && index < langArray.Count ? langArray[index] : "unknown";
    }

    public static string getTextStr(string str)
    {
        if (strEnumDict.TryGetValue(str, out int index))
        {
            return langArray[index];
        }
        return "unknown";
    }

    public static void setTextStr(UnityEngine.UI.Text text, string str)
    {
        if (strEnumDict.TryGetValue(str, out int index))
        {
            text.text = langArray[index];
        }
        else
        {
            text.text = "unknown";
        }
    }

    public static void setTextStrS(UnityEngine.UI.Text text, string str, string id)
    {
        if (strEnumDict.TryGetValue(str, out int index))
        {
            text.text = replaceStrS(langArray[index], "{1}", id);
        }
        else
        {
            text.text = "unknown";
        }
    }

    public static void setTextStr(TextMeshProUGUI text, string str)
    {
        if (strEnumDict.TryGetValue(str, out int index))
        {
            text.text = langArray[index];
        }
        else
        {
            text.text = "unknown";
        }
    }

    public static void setTextStr(UnityEngine.UI.Text text, int index)
    {
        string str = langArray[index];
        text.text = str;
    }
    public static void setTextStrWithNewLine(UnityEngine.UI.Text text, int index)
    {
        string str = langArray[index];
        str = str.Replace("\\n", "\n");
        text.text = str;
    }

    public static void setTextStr(UnityEngine.UI.Text text, int index, int id1)
    {
        string str = langArray[index];
        str = replaceStr(str, "{1}", id1);
        text.text = str;
    }
    public static void setTextStrWithNewLine(UnityEngine.UI.Text text, int index, int id1)
    {
        string str = langArray[index];
        str = replaceStr(str, "{1}", id1);
        str = str.Replace("\\n", "\n");
        text.text = str;
    }

    public static void setTextStrS(UnityEngine.UI.Text text, int index, string id1)
    {
        string str = langArray[index];
        str = replaceStrS(str, "{1}", id1);
        text.text = str;
    }
    public static void setTextStrWithNewLineS(UnityEngine.UI.Text text, int index, string id1)
    {
        string str = langArray[index];
        str = replaceStrS(str, "{1}", id1);
        str = str.Replace("\\n", "\n");
        text.text = str;
    }

    public static void setTextStr(UnityEngine.UI.Text text, int index, int id1, int id2)
    {
        string str = langArray[index];
        str = replaceStr(str, "{1}", id1);
        str = replaceStr(str, "{2}", id2);
        text.text = str;
    }
    public static void setTextStrWithNewLine(UnityEngine.UI.Text text, int index, int id1, int id2)
    {
        string str = langArray[index];
        str = replaceStr(str, "{1}", id1);
        str = replaceStr(str, "{2}", id2);
        str = str.Replace("\\n", "\n");
        text.text = str;
    }

    public static void setTextStrS(UnityEngine.UI.Text text, int index, string id1, string id2)
    {
        string str = langArray[index];
        str = replaceStrS(str, "{1}", id1);
        str = replaceStrS(str, "{2}", id2);
        text.text = str;
    }
    public static void setTextStrWithNewLineS(UnityEngine.UI.Text text, int index, string id1, string id2)
    {
        string str = langArray[index];
        str = replaceStrS(str, "{1}", id1);
        str = replaceStrS(str, "{2}", id2);
        str = str.Replace("\\n", "\n");
        text.text = str;
    }


    public static void setTextStr(UnityEngine.UI.Text text, int index, int id1, int id2, int id3)
    {
        string str = langArray[index];
        str = replaceStr(str, "{1}", id1);
        str = replaceStr(str, "{2}", id2);
        str = replaceStr(str, "{3}", id3);
        text.text = str;
    }
    public static void setTextStrWithNewLine(UnityEngine.UI.Text text, int index, int id1, int id2, int id3)
    {
        string str = langArray[index];
        str = replaceStr(str, "{1}", id1);
        str = replaceStr(str, "{2}", id2);
        str = replaceStr(str, "{3}", id3);
        str = str.Replace("\\n", "\n");
        text.text = str;
    }

    public static void setTextStrS(UnityEngine.UI.Text text, int index, string id1, string id2, string id3)
    {
        string str = langArray[index];
        str = replaceStrS(str, "{1}", id1);
        str = replaceStrS(str, "{2}", id2);
        str = replaceStrS(str, "{3}", id3);
        text.text = str;
    }
    public static void setTextStrWithNewLineS(UnityEngine.UI.Text text, int index, string id1, string id2, string id3)
    {
        string str = langArray[index];
        str = replaceStrS(str, "{1}", id1);
        str = replaceStrS(str, "{2}", id2);
        str = replaceStrS(str, "{3}", id3);
        str = str.Replace("\\n", "\n");
        text.text = str;
    }

    private static string replaceStr(string str, string holder, int id)
    {
        if (id < 0)
        {
            string val = langArray[-id];
            str = str.Replace(holder, val);
        }
        else
        {
            string val = id.ToString();
            str = str.Replace(holder, val);
        }

        return str;
    }

    private static string replaceStrS(string str, string holder, string id)
    {
        int i;
        if (int.TryParse(id, out i))
        {
            return replaceStr(str, holder, i);
        }
        else
        {
            return str.Replace(holder, id);
        }
    }

    public static bool wordValidalityCheck(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return false;
        }

        if (System.Text.RegularExpressions.Regex.IsMatch(text, @"[!@#$%^&*(),.?""':;{}|<>]"))
        {
            return false;
        }

        text = WordNetData.GetLemmaOfWord(text);
        if (text == "No results found")
        {
            return false;
        }

        return true;
    }

    public static int LoadColorModePreference()
    {
        return PlayerPrefs.GetInt("ColorMode", 0);
    }

    public static Color getBackgroundColor()
    {
        int colorIndex = LoadColorModePreference();
        return colorIndex == 0 ? Define.ColorWhite : Define.ColorBlack;
    }

    public static Color getProspectColor()
    {
        int colorIndex = LoadColorModePreference();
        return colorIndex == 0 ? Define.ColorBlack : Define.ColorWhite;
    }

    public static void DoFadeIn(Text text, float duration = 0.5f)
    {
        Color textColor = text.color;
        textColor.a = 0;
        text.color = textColor;
        text.DOFade(1, duration);
    }

    public static void ChangeAllFonts(Transform parent)
    {
        Font font = Resources.Load<Font>("Fonts/georgia");

        Text[] allTextComponents = parent.GetComponentsInChildren<Text>();
        foreach (Text textComponent in allTextComponents)
        {
            textComponent.font = font;
        }
    }
}
