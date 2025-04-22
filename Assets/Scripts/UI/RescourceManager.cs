using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class RescourceManager : MonoBehaviour
{

    public static RescourceManager instance;
    void Awake()
    {
        instance = this;
    }
    public Font timeNewRoman;
    public Font cour;
    public Font Felixti;

    public Font ahronbd;
}
