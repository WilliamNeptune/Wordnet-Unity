using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using Debug = UnityEngine.Debug;
public static class CreateGameObject
{
    public static string pathPrefabSynsetMarker = "Prefabs/UI/synsetCounter";
    public static string pathPrefabBookmarkEntity = "Prefabs/UI/BookmarksubWidget";
    public static string pathSecondaryPanelPrefix = "Prefabs/UI/Secondary/";
    public static string pathWordSuggestion = "Prefabs/UI/wordExample";
    public static string pathEntityPrefab = "Prefabs/2D/TestEntityPrefab";
    public static string pathEntityPrefabBranch1 = "Prefabs/2D/TestEntityPrefabBranch1";
    public static string pathEntityPrefabBranch2 = "Prefabs/2D/TestEntityPrefabBranch2";
    //public static string pathConnectionPrefab = "Prefabs/2D/ConnectionPrefab.prefab";
    //public static string pathLinePointPrefab = "Prefabs/2D/LinePointPrefab.prefab";
    public static string pathTrendingWordEntity = "Prefabs/UI/TrendingWordEntityWidget";
    public static string pathBookmarkShowEntity = "Prefabs/UI/BookmarkWidget";
    public static string pathBookmarkChoosingEntity = "Prefabs/UI/BookmarkWidgetForChoosing";

    public static GameObject createUIObject(Transform parent, Vector2 anchoredPosition, string path, bool isSecondary = false)
    {
        
        string pathPrefab = isSecondary ? (pathSecondaryPanelPrefix + path) : path;
        GameObject prefab = Resources.Load<GameObject>(pathPrefab);
        if (prefab == null)
        {
            Debug.LogError("Failed to load bookmark prefab.");
            return null;
        }

        GameObject newObject = GameObject.Instantiate(prefab, parent);
        
        if (newObject.TryGetComponent(out RectTransform rectTransform))
        {
            rectTransform.anchoredPosition = anchoredPosition;
        }
        else
        {
            Debug.LogWarning("Created object does not have a RectTransform component.");
        }

        Functions.ChangeAllFonts(newObject.transform);
        
        return newObject;
    }

    public static GameObject Create2DEntity(Transform parent, Vector3 position, string path)
    {
        GameObject prefab = Resources.Load<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("Failed to load bookmark prefab.");
            return null;
        }
        GameObject newObject = GameObject.Instantiate(prefab, parent);
        newObject.transform.position = position;
        return newObject;
    }
}

