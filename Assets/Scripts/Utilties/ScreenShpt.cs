using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ScreenShpt : MonoBehaviour
{
    void Start()
    {
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
           ScreenCapture.CaptureScreenshot("screenshot.png" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");
        }
    }
}