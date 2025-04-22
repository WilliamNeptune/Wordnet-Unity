using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ParentPanel : MonoBehaviour
{
    [SerializeField]protected GameObject[] whiteElements;
    [SerializeField]protected GameObject[] blackElements;
    protected  Action OnClosePanel;

    protected virtual void Start()
    {
        SetColorMode();
        Functions.OnColorModeChanged += SetColorMode;
    }

    protected void OnDestroy()
    {
        Functions.OnColorModeChanged -= SetColorMode;
    }

    protected void Update()
    {
        
    }

    protected void OnEnable()
    {
        
    }

    protected void OnDisable()
    {
        OnClosePanel?.Invoke();
    }

    protected void closePanel()
    {
        PanelManager.CloseTopPanel();
    }

    protected virtual void SetColorMode()
    {
        Color color1 = Functions.getBackgroundColor();
        Color color2 = Functions.getProspectColor();

        foreach (GameObject element in whiteElements)
        {
            Image image = element.GetComponent<Image>();
            if (image != null)
            {
                image.color = color1;
            }
            Text text = element.GetComponent<Text>();
            if (text != null)
            {
                text.color = color1;
            }                
        }
        foreach (GameObject element in blackElements)
        {
            Image image = element.GetComponent<Image>();
            if (image != null)
            {
                image.color = color2;
            }
            Text text = element.GetComponent<Text>();
            if (text != null)
            {
                text.color = color2;
            }
        }
    }
}
