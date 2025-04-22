using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelGenerator : MonoBehaviour
{
    [SerializeField] private string uiPrefabPath = "Prefabs/UI";    
    public GameObject InstantiatePanel(string prefabName)
    {
        GameObject prefab = Resources.Load(uiPrefabPath + "/" + prefabName) as GameObject;
        GameObject instance = Instantiate(prefab);
        return instance;
    }
}
