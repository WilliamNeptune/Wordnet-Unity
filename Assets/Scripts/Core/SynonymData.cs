using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using UnityEngine;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

public static class SynonymData
{
    public static List<SynonymStruct.TrendingWord> storedTrendingWords = new List<SynonymStruct.TrendingWord>();
    private static string currentVersion = "1.0.1";
    public static string GetCurrentVersion()
    {
        return currentVersion;
    }
}

#region Custom Lemma
[System.Serializable]
public class CustomLemmaStorage
{
    private Dictionary<uint, List<string>> customLemmas;
    public CustomLemmaStorage()
    {
        customLemmas = new Dictionary<uint, List<string>>();
    }

    public bool AddCustomLemma(uint synsetId, string lemma)
    {
        if (!customLemmas.ContainsKey(synsetId))
            customLemmas[synsetId] = new List<string>();
        if (WordNetData.isCustomLemmaExistsInSynset(synsetId, lemma))
            return false;
        if (customLemmas[synsetId].Contains(lemma))
            return false;
        customLemmas[synsetId].Add(lemma);
        saveCustomLemmaToPlayerPrefs();
        return true;
    }

    public bool AddCustomLemma(uint synsetId, List<string> lemmas)
    {
        if (!customLemmas.ContainsKey(synsetId))
            customLemmas[synsetId] = new List<string>();
        foreach (string lemma in lemmas)
        {
            customLemmas[synsetId].Add(lemma);
        }
        saveCustomLemmaToPlayerPrefs();
        return true;
    }
    public void RemoveCustomLemma(uint synsetId, string lemma)
    {
        if (customLemmas.ContainsKey(synsetId))
            customLemmas[synsetId].Remove(lemma);
        if (customLemmas[synsetId].Count == 0)
        {
            customLemmas.Remove(synsetId);
        }
        saveCustomLemmaToPlayerPrefs();
    }
    public List<string> GetCustomLemmas(uint synsetId)
    {
        if (customLemmas.ContainsKey(synsetId))
            return customLemmas[synsetId].ToList();
        return new List<string>();
    }
    public void ClearCustomLemma()
    {
        customLemmas.Clear();
        saveCustomLemmaToPlayerPrefs();
    }
    public void saveCustomLemmaToPlayerPrefs()
    {
        string json = JsonConvert.SerializeObject(customLemmas);
        PlayerPrefs.SetString("CustomLemmas", json);
        PlayerPrefs.Save();
    }
    public static CustomLemmaStorage LoadCustomLemmaFromPlayerPrefs()
    {
        CustomLemmaStorage store = new CustomLemmaStorage();
        string json = PlayerPrefs.GetString("CustomLemmas", "");
        if (!string.IsNullOrEmpty(json))
            store.customLemmas = JsonConvert.DeserializeObject<Dictionary<uint, List<string>>>(json);
        return store;
    }

    public static bool isCustomLemmasContainsWord(uint synsetId, string word)
    {
        CustomLemmaStorage customLemmaStorage = LoadCustomLemmaFromPlayerPrefs();
        List<string> customLemmas = customLemmaStorage.GetCustomLemmas(synsetId);
        return customLemmas.Contains(word);
    }
}
#endregion

#region Bookmark
public class Bookmark
{
    public Dictionary<int, string> bookmarkNames;
    public Dictionary<int, List<string>> bookmarkLemmas;
    public Bookmark()
    {
        Debug.Log("Bookmark constructor");
        bookmarkNames = new Dictionary<int, string>() { { 0, "Default" } };
        bookmarkLemmas = new Dictionary<int, List<string>>();
    }

    public Bookmark(Dictionary<int, string> bookmarkNames)
    {
        this.bookmarkNames = bookmarkNames;
        bookmarkNames[0] = "Default";
        if (PlayerPrefs.GetString("BookmarkNames", "") == "")
        {
            bookmarkLemmas = new Dictionary<int, List<string>>();
        }
        else
        {
            bookmarkLemmas = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(PlayerPrefs.GetString("BookmarkLemmas", ""));
        }
    }

    public void AddBookmark(int id, string name)
    {
        bookmarkNames[id] = name;
        bookmarkLemmas[id] = new List<string>();
    }

    public void DeleteBookmark(int id)
    {
        bookmarkNames.Remove(id);
        bookmarkLemmas.Remove(id);
    }

    public void ChangeBookmarkName(int id, string name)
    {
        bookmarkNames[id] = name;
    }

    public Dictionary<int, string> GetAllBookmarks()
    {
        return bookmarkNames;
    }
    //for client
    public void AddLemmaToBookmark(int id, string lemma)
    {
        if (!bookmarkLemmas.ContainsKey(id))
            bookmarkLemmas[id] = new List<string>();

        if (bookmarkLemmas[id].Contains(lemma))
            return;
        bookmarkLemmas[id].Add(lemma);
    }

    //for Server
    public void SetLemmaToBookmark(int id, List<string> lemmas)
    {
        if (!bookmarkLemmas.ContainsKey(id))
            bookmarkLemmas[id] = lemmas;
        else
            bookmarkLemmas[id] = lemmas;
    }
    public void DeleteLemmaFromBookmark(int id, string lemma)
    {
        if (!bookmarkLemmas.ContainsKey(id))
            return;
        bookmarkLemmas[id].Remove(lemma);
    }

    public List<string> GerAllLemmasInBookmark(int id)
    {
        if (!bookmarkLemmas.ContainsKey(id))
            return new List<string>();
        return bookmarkLemmas[id].ToList();
    }

    public void SaveBookmarkToPlayerPrefs()
    {
        string json = JsonConvert.SerializeObject(bookmarkNames);
        PlayerPrefs.SetString("BookmarkNames", json);
        json = JsonConvert.SerializeObject(bookmarkLemmas);
        PlayerPrefs.SetString("BookmarkLemmas", json);
        PlayerPrefs.Save();
    }

    public static Bookmark LoadBookmarkFromPlayerPrefs()
    {
        Bookmark bookmark = new Bookmark();
        string json = PlayerPrefs.GetString("BookmarkNames", "");
        if (json != null && json != "")
        {
            bookmark.bookmarkNames = JsonConvert.DeserializeObject<Dictionary<int, string>>(json);
        }

        json = PlayerPrefs.GetString("BookmarkLemmas", "");
        if (json != null && json != "")
        {
            bookmark.bookmarkLemmas = JsonConvert.DeserializeObject<Dictionary<int, List<string>>>(json);
        }

        if (bookmark.bookmarkNames == null)
            Debug.LogError("BookmarkNames is null");
        return bookmark;
    }
}
#endregion

#region Phonetic Transcription
public class AIPhoneticTranscription
{
    private Dictionary<string, string> phoneticTranscription;

    public AIPhoneticTranscription()
    {
        phoneticTranscription = new Dictionary<string, string>();
    }

    public void AddAIPhoneticTranscription(string word, string transcription)
    {
        phoneticTranscription[word] = transcription;
        SaveAIPhoneticTranscriptionToPlayerPrefs();
    }

    public string GetAIPhoneticTranscription(string word)
    {
        if (phoneticTranscription.ContainsKey(word))
            return phoneticTranscription[word];
        return "";
    }

    public void SaveAIPhoneticTranscriptionToPlayerPrefs()
    {
        string json = JsonConvert.SerializeObject(phoneticTranscription);
        PlayerPrefs.SetString("AIPhoneticTranscription", json);
        PlayerPrefs.Save();
    }

    public static AIPhoneticTranscription LoadAIPhoneticTranscriptionFromPlayerPrefs()
    {
        AIPhoneticTranscription aiPhoneticTranscription = new AIPhoneticTranscription();
        string json = PlayerPrefs.GetString("PhoneticTranscription", "");
        if (json != null && json != "")
            aiPhoneticTranscription.phoneticTranscription = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        return aiPhoneticTranscription;
    }
}
#endregion

#region AI Comparsion
public class AIComparsion
{
    private Dictionary<string, string> aiComparsion;
    public AIComparsion()
    {
        aiComparsion = new Dictionary<string, string>();
    }

    public void AddAIComparsion(string word1, string word2, string comparision)
    {
        aiComparsion[word1 + "_" + word2] = comparision;
        SaveAIComparsionToPlayerPrefs();
    }

    public string GetAIComparsion(string word)
    {
        if (aiComparsion.ContainsKey(word))
            return aiComparsion[word];
        return "";
    }

    public string GetAIComparsion(string word1, string word2)
    {
        string savedComparsion = GetAIComparsion(word1 + "_" + word2);
        if (savedComparsion != "")
            return savedComparsion;
        savedComparsion = GetAIComparsion(word2 + "_" + word1);
        return savedComparsion;
    }

    public void SaveAIComparsionToPlayerPrefs()
    {
        string json = JsonConvert.SerializeObject(aiComparsion);
        PlayerPrefs.SetString("AIComparsion", json);
        PlayerPrefs.Save();
    }

    public static AIComparsion LoadAIComparsionFromPlayerPrefs()
    {
        AIComparsion aiComparsion = new AIComparsion();
        string json = PlayerPrefs.GetString("AIComparsion", "");
        if (json != null && json != "")
            aiComparsion.aiComparsion = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        return aiComparsion;
    }
}
#endregion