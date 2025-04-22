using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : ParentPanel
{
    public Text messageText;
    public Image txtBackground;
    public void updateMessage(string message, bool isLocalize = false)
    {
         if (isLocalize)
        {
            Functions.setTextStr(messageText, message);
        }
        else
        {
            messageText.text = message;
        }
        setBackground();
    }

    public void setBackground()
    {
        // Get the preferred width of the text
        float textWidth = messageText.preferredWidth;
        
        // Add some padding if needed
        float padding = 20f; // Adjust this value as needed
        Debug.Log("textWidth == " + textWidth);
        // Set the width of the background
        RectTransform bgRect = txtBackground.rectTransform;
        bgRect.sizeDelta = new Vector2(textWidth + padding, bgRect.sizeDelta.y);
        
        // Force layout update
        LayoutRebuilder.ForceRebuildLayoutImmediate(txtBackground.rectTransform);
    }
}
