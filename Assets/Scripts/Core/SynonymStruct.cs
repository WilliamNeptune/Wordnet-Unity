using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.InteropServices;

[Serializable]
public class SynonymStruct
{
    [Serializable]
    public class SynsetGroup
    {
        public int synsetId;
        public string definition;
        public string pos;
        //public string[] lemmas;
        public List<string> lemmas;  
        public List<string> customLemmas;
    }

    [Serializable]
    public class SynsetResponse
    {
        public string lemma;
        public List<SynsetGroup> synsetGroups;
    }

    public class RequestData
    {
        public string lemma1;
        public string lemma2;
    }

    public struct Feedback
    {
        public string userId;
        public string feedback;
    }

    public struct TrendingWord
    {
        public string word;
        public float percentageChange;
    }
    public struct SynsetRequest
    {
        public string lemma;
        public string userId;
    }   

    public struct BookmarkPages
    {
        public int bookid;
        public string bookmarkName;
    }   
}