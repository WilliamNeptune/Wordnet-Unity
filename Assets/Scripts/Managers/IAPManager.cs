using UnityEngine;
using UnityEngine.Purchasing;
using System;
using Unity.Services.Core;


public class IAPManager : MonoBehaviour
{
    private static IAPManager instance;
    private bool isSubscribed = true;
    public static IAPManager Instance => instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

     public bool IsSubscribed()
    {
#if UNITY_EDITOR  // note : it is not enough to only set ture for Ai searching, you also need to login;
        return true;
#else
        return isSubscribed;
#endif
    }
}
