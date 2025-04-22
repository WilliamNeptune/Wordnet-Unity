using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LemmaRisingPanel : MonoBehaviour
{
    [SerializeField] private Button btnClose;
    [SerializeField] private Button btnAddToBookmarks;
    [SerializeField] private Button btnShowdown;
    
    void Start()
    {
        btnClose.onClick.AddListener(ClosePanel);
        btnAddToBookmarks.onClick.AddListener(AddToBookmarks);
        btnShowdown.onClick.AddListener(ClosePanel);
    }

    private void ClosePanel()
    {
        PanelManager.ClosePanelVertically(this.gameObject, null, 0f);
    }

    private void AddToBookmarks()
    {
        GameObject panel = PanelManager.CreatePanel(ContainerPanel.instance.transform, "BookmarkChoosingPanel", new Vector2(0, 0), true);
        PanelManager.OpenPanelVertically(panel, null, 900f);
    }
}

