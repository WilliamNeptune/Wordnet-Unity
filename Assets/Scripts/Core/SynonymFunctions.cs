using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data = SynonymData;
using Structs = SynonymStruct;
public static class SynonymFunctions
{
    public static List<string> GetAllPosTypes(Structs.SynsetResponse response)
    {
        return response.synsetGroups
            .Select(group => group.pos)
            .Distinct()
            .ToList();
    }

    public static int CountSynsetsByPos(Structs.SynsetResponse response, int posIndex)
    {
        var allPosTypes = GetAllPosTypes(response);
        
        if (posIndex < 0 || posIndex >= allPosTypes.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(posIndex), "POS index is out of range.");
        }

        string targetPos = allPosTypes[posIndex];

        return response.synsetGroups
            .Where(group => group.pos == targetPos)
            .Select(group => group.synsetId)
            .Distinct()
            .Count();
    }

    public static int getGlobalSynsetIndex(Structs.SynsetResponse response, int posIndex, int currentSynsetIndex)
    {
        var allPosTypes = GetAllPosTypes(response);
        
        if (posIndex < 0 || posIndex >= allPosTypes.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(posIndex), "POS index is out of range.");
        }

        int globalIndex = 0;

        // Sum up synsets for all previous POS types
        for (int i = 0; i < posIndex; i++)
        {
            globalIndex += CountSynsetsByPos(response, i);
        }

        // Add the current synset index
        globalIndex += currentSynsetIndex;

        return globalIndex;
    }

    // ... existing code ...

    // public static int CountLemmasInCurrentSynset(Structs.SynsetResponse response, int posIndex, int currentSynsetIndex)
    // {
    //     var allPosTypes = GetAllPosTypes(response);
        
    //     if (posIndex < 0 || posIndex >= allPosTypes.Count)
    //     {
    //         throw new ArgumentOutOfRangeException(nameof(posIndex), "POS index is out of range.");
    //     }

    //     string targetPos = allPosTypes[posIndex];
        
    //     var synsetsByPos = response.synsetGroups
    //         .Where(group => group.pos == targetPos)
    //         .ToList();

    //     if (currentSynsetIndex < 0 || currentSynsetIndex >= synsetsByPos.Count)
    //     {
    //         throw new ArgumentOutOfRangeException(nameof(currentSynsetIndex), "Synset index is out of range.");
    //     }

    //     return synsetsByPos[currentSynsetIndex].lemmas.Count + synsetsByPos[currentSynsetIndex].customLemmas.Count;
    // }
}
