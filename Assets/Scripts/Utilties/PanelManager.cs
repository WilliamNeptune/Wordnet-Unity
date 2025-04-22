using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public static class PanelManager
{
    private static GameObject topPanel;
    private static Stack<GameObject> panelStack = new Stack<GameObject>();
    public static GameObject CreatePanel(Transform parent, string panelName, Vector2 anchoredPosition, bool isSecondary = false)
    {
        GameObject panel = CreateGameObject.createUIObject(parent, anchoredPosition, panelName, isSecondary);
        Debug.Log("CreatePanel and push to stack: " + panel.name);
        panelStack.Push(panel);

        if(isSecondary)
        {
            ContainerPanel.instance.SetBtnBackVisible(true);
        }
        return panel;
    }

    public static void CreateMessagePanel(string message, bool isLocalize = false, float delay = 1.5f)
    {
        GameObject panel = CreateGameObject.createUIObject(ContainerPanel.instance.transform, Vector2.zero, "MessagePanel", true);
        if (panel.TryGetComponent(out MessagePanel messagePanel))
        {
            messagePanel.updateMessage(message, isLocalize);
        }
        panelStack.Push(panel);

        DOVirtual.DelayedCall(delay, () => {
            PopPanelFromStack(panel);
        });
    }
    //use this method if the panel is always the top panel
    public static void CloseTopPanel(Action onComplete = null)
    {
         if (panelStack.Count > 0)
        {
            GameObject topPanel = panelStack.Pop();
            Debug.Log("Destroy topPanel: " + topPanel.name);
            GameObject.Destroy(topPanel);
            onComplete?.Invoke();
        }
    }
    //recommend to use this method to close panel
    public static void PopPanelFromStack(GameObject panel)
    {
        if (panelStack.Contains(panel))
        {
            // Create a temporary stack to hold items
            Stack<GameObject> tempStack = new Stack<GameObject>();
            
            // Pop items until we find the panel to remove
            while (panelStack.Count > 0)
            {
                GameObject currentPanel = panelStack.Pop();
                if (currentPanel != panel)
                {
                    tempStack.Push(currentPanel);
                }
            }
            
            // Push back all other panels
            while (tempStack.Count > 0)
            {
                panelStack.Push(tempStack.Pop());
            }
            
            GameObject.Destroy(panel);

            Debug.Log("panelStack.Count: " + panelStack.Count);
            Debug.Log("ContainerPanel.instance.isWordnetMode: " + ContainerPanel.instance.isWordnetMode);
            if (panelStack.Count == 0 && !ContainerPanel.instance.isWordnetMode)
            {

                ContainerPanel.instance.SetBtnBackVisible(false);
            }
        }
        else
        {
            Debug.LogError("Panel not found in stack: " + panel.name);
        }
    }

    public static GameObject GetTopPanel()
    {
        return panelStack.Peek();
    }

    public static int GetGeneratedPanelCount()
    {
        return panelStack.Count;
    }

    public static void openPanelHorizontally(GameObject panel, Action onComplete, float xPosition)
    {
        panel.GetComponent<RectTransform>().DOAnchorPosX(xPosition, 0.1f).SetEase(Ease.OutQuad).OnComplete(() => {
            onComplete?.Invoke();
         });
    }

    public static void closePanelHorizontally(GameObject panel, Action onComplete, float xPosition)
    {
            panel.GetComponent<RectTransform>().DOAnchorPosX(xPosition, 0.1f).SetEase(Ease.InQuad).OnComplete(() => {
           if(panel)
           {
                PopPanelFromStack(panel);
           }
           onComplete?.Invoke();
        });
    }

    public static void ClosePanelVertically (GameObject panel, Action onComplete, float yPosition)
    {
        panel.GetComponent<RectTransform>().DOAnchorPosY(yPosition, 0.1f).SetEase(Ease.InQuad).OnComplete(() => {
           if(panel)
           {
                PopPanelFromStack(panel);
           }
           onComplete?.Invoke();
        });
    }

    public static void OpenPanelVertically (GameObject panel, Action onComplete, float yPosition, float duration = 0.1f)
    {;
        panel.GetComponent<RectTransform>().DOAnchorPosY(yPosition, duration).SetEase(Ease.OutQuad).OnComplete(() => {
           onComplete?.Invoke();
        });
    }
    public static bool isSynonymComparePanelOpen()
    {
        if (panelStack.Count > 0)
        {
            Debug.Log("panelStack.Count: " + panelStack.Count);
            if(SynonymComparePanel.instance != null)
            {
                return panelStack.Contains(SynonymComparePanel.instance.gameObject);
            }
        }
        return false;
    }
}
