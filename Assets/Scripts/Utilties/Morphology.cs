using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Morphology: MonoBehaviour
{
    private static readonly string[] Suffixes = {
        // Noun suffixes
        "s", "ses", "xes", "zes", "ches", "shes", "men", "ies",
        // Verb suffixes
        "s", "ies", "es", "es", "ed", "ed", "ing", "ing",
        // Adjective suffixes
        "er", "est", "er", "est"
    };

    private static readonly string[] Endings = {
        // Noun endings
        "", "s", "x", "z", "ch", "sh", "man", "y",
        // Verb endings
        "", "y", "e", "", "e", "", "e", "",
        // Adjective endings
        "", "", "e", "e"
    };
    public string text;
    void Start()
    {
       int init = MorphInit();
       string lemma = GetLemmaOfaWord(text);
        Debug.Log("ajfagbasdg ==" + lemma);

       //Debug.Log("initException: " + init);
    }

    private static readonly string[] posTypes = {"noun", "verb", "adj", "adv"};

    private static readonly int[] Offsets = { 0, 0, 8, 16 };
    private static readonly int[] Counts = { 0, 8, 8, 4 };
    private static readonly List<Preposition> Prepositions = new List<Preposition>
    {
        new Preposition("to", 2),
        new Preposition("at", 2),
        new Preposition("of", 2),
        new Preposition("on", 2),
        new Preposition("off", 3),
        new Preposition("in", 2),
        new Preposition("out", 3),
        new Preposition("up", 2),
        new Preposition("down", 4),
        new Preposition("from", 4),
        new Preposition("with", 4),
        new Preposition("into", 4),
        new Preposition("for", 3),
        new Preposition("about", 5),
        new Preposition("between", 7)
    };

    private enum POS
    {
        NOUN = 1,
        VERB = 2,
        ADJ = 3,
        ADV = 4,
        SATELLITE = 5
    };

    //private static Dictionary<int, StreamReader> ExceptionFiles = new Dictionary<int, StreamReader>();
    private static Dictionary<int, List<string>> ExceptionFiles = new Dictionary<int, List<string>>();  
    public static string GetLemmaOfaWord(string word)
    {
        int start = (int)POS.VERB, end = (int)POS.ADV;
       for(int i = start; i <= end; i++)
       {
            string lemma = MorphStr(word, i);
            //string lemma = MorphWord(word, i);
            if(lemma != null) return lemma;
       }
       return null;
    }

    public static int MorphInit()
    {
        // Open exception list files
        for (int i = 1; i <= 4; i++) // Assuming 4 parts of speech
        {
            string filePath = Application.dataPath + "/Data/" + posTypes[i - 1] + ".exc";
            if (File.Exists(filePath))
            {
                //ExceptionFiles[i] = new StreamReader(filePath);
               ExceptionFiles[i] = new List<string>(File.ReadAllLines(filePath));
            }
            else
            {
                Debug.Log($"Error: Can't open exception file {filePath}");
                return -1;
            }
        }
        return 0;
    }

    public static int CountWords(string s, char separator)
    {
        int wordCount = 0;
        bool inWord = false;

        foreach (char c in s)
        {
            if (c == separator || c == ' ' || c == '_')
            {
                if (inWord)
                {
                    wordCount++;
                    inWord = false; // End of a word
                }
            }
            else
            {
                inWord = true; // Inside a word
            }
        }

        // If the last character was part of a word, count it
        if (inWord)
        {
            wordCount++;
        }

        return wordCount;
    }
    private static string searchstr = "";
    private static string str = "";
    private static int svcnt = 0;
    private static int svprep = 0;

    public static string MorphStr(string origstr, int pos)
    {
        string word = "";
        string tmp;
        int cnt, st_idx = 0, end_idx;
        int prep;
        string append;
        
        if (pos == (int)POS.SATELLITE)
            pos = (int)POS.ADJ;

        // First time through for this string
        if (origstr != null)
        {
            // Assume string hasn't had spaces substituted with '_'
            str = origstr.Replace(' ', '_').ToLower();
            searchstr = "";
            cnt = CountWords(str, '_');
            svprep = 0;

            // First try exception list
            tmp = ExcLookup(str, pos);
            if (tmp != null && tmp != str)
            {
                svcnt = 1;      // Force next time to pass NULL
                return tmp;
            }

            // Then try simply morph on original string
            if (pos != (int)POS.VERB && (tmp = MorphWord(str, pos)) != null && tmp != str)
                return tmp;

            if (pos == (int)POS.VERB && cnt > 1 && (prep = HasPrep(str, cnt)) > 0)
            {
                // Assume we have a verb followed by a preposition
                svprep = prep;
                return MorphPrep(str, pos);
            }
            else
            {
                svcnt = cnt = CountWords(str, '-');
                while (origstr != null && --cnt > 0)
                {
                    int end_idx1 = str.IndexOf('_', st_idx);
                    int end_idx2 = str.IndexOf('-', st_idx);
                    if (end_idx1 >= 0 && end_idx2 >= 0)
                    {
                        if (end_idx1 < end_idx2)
                        {
                            end_idx = end_idx1;
                            append = "_";
                        }
                        else
                        {
                            end_idx = end_idx2;
                            append = "-";
                        }
                    }
                    else
                    {
                        if (end_idx1 >= 0)
                        {
                            end_idx = end_idx1;
                            append = "_";
                        }
                        else
                        {
                            end_idx = end_idx2;
                            append = "-";
                        }
                    }
                    if (end_idx < 0) return null;     // Shouldn't do this
                    word = str.Substring(st_idx, end_idx - st_idx);
                    if ((tmp = MorphWord(word, pos)) != null)
                        searchstr += tmp;
                    else
                        searchstr += word;
                    searchstr += append;
                    st_idx = end_idx + 1;
                }
                
                if ((tmp = MorphWord(str.Substring(st_idx), pos)) != null)
                    searchstr += tmp;
                else
                    searchstr += str.Substring(st_idx);
                if (searchstr != str && IsDefined(searchstr, pos))
                    return searchstr;
                else
                    return null;
            }
        }
        else
        {   // Subsequent call on string
            if (svprep > 0)
            {   // If verb has preposition, no more morphs
                svprep = 0;
                return null;
            }
            else if (svcnt == 1)
                return ExcLookup(null, pos);
            else
            {
                svcnt = 1;
                if ((tmp = ExcLookup(str, pos)) != null && tmp != str)
                    return tmp;
                else
                    return null;
            }
        }
    }
    public static string MorphWord(string word, int pos)
    {
        if (word == null)
            return null;

        // First look for word in exception list
        string tmp = ExcLookup(word, pos);
        if (tmp != null)
            return tmp; // Found it in exception list

        if (pos == (int)POS.ADV)
            return null;

        string tmpBuf = word;
        string end = "";

        if (pos == (int)POS.NOUN) // Assuming 0 is for NOUN
        {
            if (StrEnd(word, "ful"))
            {
                int cnt = word.LastIndexOf('f');
                tmpBuf = word.Substring(0, cnt);
                end = "ful";
            }
            else if (StrEnd(word, "ss") || word.Length <= 2)
            {
                return null;
            }
        }

        int offset = Offsets[pos];
        int count = Counts[pos];

        for (int i = 0; i < count; i++)
        {
            string baseForm = WordBase(tmpBuf, i + offset);
            if (baseForm != tmpBuf && IsDefined(baseForm, pos))
            {
                return baseForm + end;
            }
        }
        return null;
    }

    private static bool StrEnd(string str1, string str2)
    {
        return str1.EndsWith(str2);
    }

    private static string WordBase(string word, int ender)
    {
        string copy = word;
        if (StrEnd(copy, Suffixes[ender]))
        {
            copy = copy.Substring(0, copy.Length - Suffixes[ender].Length) + Endings[ender];
        }
        return copy;
    }

    private static int HasPrep(string s, int wordCount)
    {
        for (int wdNum = 2; wdNum <= wordCount; wdNum++)
        {
            int index = s.IndexOf('_');
            if (index != -1)
            {
                s = s.Substring(index + 1);
                foreach (var prep in Prepositions)
                {
                    if (s.StartsWith(prep.Str) && (s.Length == prep.Str.Length || s[prep.Str.Length] == '_'))
                    {
                        return wdNum;
                    }
                }
            }
        }
        return 0;
    }

public static string MorphPrep(string s, int poss)
{
    string rest, excWord, lastwd = null, last;
    string word, end;
    string retval;

    // Assume that the verb is the first word in the phrase.
    // Strip it off, check for validity, then try various morphs with the
    // rest of the phrase tacked on, trying to find a match.

    int restIndex = s.IndexOf('_');
    int lastIndex = s.LastIndexOf('_');
    rest = s.Substring(restIndex);
    last = s.Substring(lastIndex);

    if (restIndex != lastIndex) // more than 2 words
    {
        lastwd = MorphWord(last.Substring(1), (int)POS.NOUN);
        if (lastwd != null)
        {
            end = rest.Substring(0, lastIndex - restIndex + 1) + lastwd;
        }
        else
        {
            end = rest;
        }
    }
    else
    {
        end = rest;
    }

    word = s.Substring(0, restIndex);
    if (!word.All(char.IsLetterOrDigit))
        return null;

    int offset = Offsets[(int)POS.VERB];
    int cnt = Counts[(int)POS.VERB];

    // First try to find the verb in the exception list
    excWord = ExcLookup(word, (int)POS.VERB);
    if (excWord != null && excWord != word)
    {
        retval = excWord + rest;
        if (IsDefined(retval, (int)POS.VERB))
            return retval;
        
        if (lastwd != null)
        {
            retval = excWord + end;
            if (IsDefined(retval, (int)POS.VERB))
                return retval;
        }
    }

    for (int i = 0; i < cnt; i++)
    {
        excWord = WordBase(word, i + offset);
        if (excWord != null && word != excWord)
        {
            retval = excWord + rest;
            if (IsDefined(retval, (int)POS.VERB))
                return retval;
            
            if (lastwd != null)
            {
                retval = excWord + end;
                if (IsDefined(retval, (int)POS.VERB))
                    return retval;
            }
        }
    }

    retval = word + rest;
    if (s != retval)
        return retval;

    if (lastwd != null)
    {
        retval = word + end;
        if (s != retval)
            return retval;
    }

    return null;
}
    private static string ExcLookup(string word, int pos)
    {
        // if (!ExceptionFiles.ContainsKey(pos))
        //     return null;

        // StreamReader reader = ExceptionFiles[pos];
        // string line;
        // while ((line = reader.ReadLine()) != null)
        // {
        //     Debug.Log("line ==" + line);
        //     if (line.StartsWith(word))
        //     {
        //         return line.Substring(word.Length).Trim(); // Return the base form
        //     }
        // }
        // return null;
        if (!ExceptionFiles.ContainsKey(pos))
            return null;

        List<string> lines = ExceptionFiles[pos];
        foreach (string line in lines)
        {
            if (line.StartsWith(word))
            {
                return line.Substring(word.Length).Trim(); // Return the base form
            }
        }
        return null;
    }

    private static bool IsDefined(string word, int pos)
    {
        // Placeholder for checking if the word is defined in the given POS
        return !string.IsNullOrEmpty(word); // Simplified check
    }

    // public IndexPtr GetValidIndexPointer(string word, int pos)
    // {
    //     IndexPtr idx = GetIndex(word, pos); // Attempt to find the word in the index

    //     // If not found, try morphological variations
    //     if (idx == null)
    //     {
    //         string morphWord = MorphStr(word, pos);
    //         while (morphWord != null)
    //         {
    //             idx = GetIndex(morphWord, pos);
    //             if (idx != null) break;
    //             morphWord = MorphStr(null, pos);
    //         }
    //     }
    //     return idx; // Return the found index or null
    // }
}

public class Preposition
{
    public string Str { get; }
    public int Length { get; }

    public Preposition(string str, int length)
    {
        Str = str;
        Length = length;
    }
}