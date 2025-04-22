using UnityEngine;
using System.Runtime.InteropServices;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public static class WordNetData
{
    //public static WordNetData instance;

    [StructLayout(LayoutKind.Sequential)]
    private struct RawSenseComponents {
        public IntPtr words;     // char** for array of strings
        public int word_count;
        public IntPtr definition;  // char*
        public IntPtr example;     // char*
        public int pos;
        public uint synsetId;
    }

    public struct SenseComponents {
        public List<string> words;  // Actual strings
        public int word_count;
        public string definition;   // Actual string
        public string example;      // Actual string
        public int pos;
        public uint synsetId;
    }

    public static List<SenseComponents> synsetGroupsByPos;

    public static int[] GetAllPosTypes()
    {
        return synsetGroupsByPos.Select(group => group.pos).Distinct().ToArray();
    }

    public static int CountSynsetsByPos(int posIndex)
    {
        var allPosTypes = GetAllPosTypes();
        if (posIndex < 0 || posIndex >= allPosTypes.Length)
            throw new ArgumentOutOfRangeException(nameof(posIndex), "POS index is out of range.");
        int targetPos = allPosTypes[posIndex];
        return synsetGroupsByPos.Count(group => group.pos == targetPos);
    }

    public static SenseComponents GetSynsetByPosIndexAndSenseIndex(int posIndex, int senseIndex)
    {
        var allPosTypes = GetAllPosTypes();
        if (posIndex < 0 || posIndex >= allPosTypes.Length)
            throw new ArgumentOutOfRangeException(nameof(posIndex), "POS index is out of range.");

        int targetPos = allPosTypes[posIndex];
        var synsetGroup = synsetGroupsByPos.Where(group => group.pos == targetPos).ToList();

        if (senseIndex < 0 || senseIndex >= synsetGroup.Count())
            throw new ArgumentOutOfRangeException(nameof(senseIndex), "Sense index is out of range.");
        
        var synset = synsetGroup[senseIndex];
        return synset;
    }

    private static List<string> GetSynonymWords(RawSenseComponents sense)
    {
        List<string> result = new List<string>();
        if (sense.word_count <= 0 || sense.words == IntPtr.Zero) return result;

        IntPtr[] wordPointers = new IntPtr[sense.word_count];
        Marshal.Copy(sense.words, wordPointers, 0, sense.word_count);
        
        for (int i = 0; i < sense.word_count; i++)
        {
            if (wordPointers[i] != IntPtr.Zero)
            {
                string word = Marshal.PtrToStringAnsi(wordPointers[i]);
                if (!string.IsNullOrEmpty(word))
                {
                    result.Add(word);
                }
            }
        }
        return result;
    }

    public static List<string> GetSynonymWords(SenseComponents sense)
    {
        return sense.words ?? new List<string>();
    }

    public static bool isCustomLemmaExistsInSynset(uint synsetId, string lemma)
    {
        // Check if any synset exists with the given synsetId
        bool synsetExists = synsetGroupsByPos.Any(group => group.synsetId == synsetId);
        if (!synsetExists)
            return false;

        var synsetGroup = synsetGroupsByPos.First(group => group.synsetId == synsetId);
        if (synsetGroup.words == null || synsetGroup.words.Count == 0)
            return false;
        
        return synsetGroup.words.Contains(lemma);
    }

    public static void UpdateSynsetsByLemma(string lemma)
    {
        int totalSenses;
        IntPtr componentsArrayPtr = getAllSynsets(lemma, out totalSenses);

        Debug.Log($"Lemma: {lemma}");
        Debug.Log($"Total synset groups: {totalSenses}");

        if (componentsArrayPtr == IntPtr.Zero || totalSenses == 0)
        {
            Debug.Log("No synset groups found!");
            return;
        }

        synsetGroupsByPos = new List<SenseComponents>();  //be careful.. the reason why the sense had repeated words because of this
        // Read the array of pointers
        IntPtr[] componentPtrs = new IntPtr[totalSenses];
        Marshal.Copy(componentsArrayPtr, componentPtrs, 0, totalSenses);

        for (int i = 0; i < totalSenses; i++)
        {
            var rawGroup = Marshal.PtrToStructure<RawSenseComponents>(componentPtrs[i]);
            
            // Convert to storage-safe structure
            var safeGroup = new SenseComponents {
                words = GetSynonymWords(rawGroup),  // Store actual strings
                word_count = rawGroup.word_count,
                definition = Marshal.PtrToStringAnsi(rawGroup.definition),
                example = rawGroup.example != IntPtr.Zero ? Marshal.PtrToStringAnsi(rawGroup.example) : null,
                pos = rawGroup.pos,
                synsetId = rawGroup.synsetId
            };

            synsetGroupsByPos.Add(safeGroup);
        }

        freeAllSynsets(componentsArrayPtr, totalSenses);
    }
#region DllImport

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("libwordnet")]
#endif
    private static extern IntPtr getAllSynsets(string word, out int totalSenses);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("libwordnet")]
#endif
    private static extern void freeAllSynsets(IntPtr components, int totalSenses);


#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("libwordnet")]
#endif
    public static extern void setDataPath(string path);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("libwordnet")]
#endif
    private static extern IntPtr searchWordNet(string word);

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("libwordnet")]
#endif
    private static extern IntPtr getFirstMeaning(string lemma);
#endregion

    public static string GetFirstMeaningOfWord(string lemma)
    {
        IntPtr resultPtr = getFirstMeaning(lemma);
        if (resultPtr == IntPtr.Zero)
            return null;
        
        return Marshal.PtrToStringAnsi(resultPtr);
    }

    public static string GetLemmaOfWord(string word)
    {
        IntPtr resultPtr = searchWordNet(word);
        if (resultPtr == IntPtr.Zero)
            return null;
        
        return Marshal.PtrToStringAnsi(resultPtr);
    }

}
