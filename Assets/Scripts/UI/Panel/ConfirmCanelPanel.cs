using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;  
using UnityEngine.UI;

public class ConfirmCanelPanel : ParentPanel
{
    [SerializeField]
    private Button confirmButton;
    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Text messageText;

    void Start()
    {
        base.Start();
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        cancelButton.onClick.AddListener(ClosePanel);
        closeButton.onClick.AddListener(ClosePanel);
        LoginManager.instance.OnDeleteAccountResult += OnDeleteAccountResult;
    }
    
    private void ClosePanel()
    {
        PanelManager.ClosePanelVertically(gameObject, null, -2000f);
    }

    public void OnConfirmButtonClick()
    {
        LoginManager.instance.TryDeleteAccount();
    }

    private void OnDeleteAccountResult(bool success, string message)
    {
        if (success)
        {
            Functions.setTextStr(messageText, "CONFIRM_DELETE_ACCOUNT");
        }
        else
        {
            messageText.text = message;
        }
    }

    private void OnDestroy()
    {
        LoginManager.instance.OnDeleteAccountResult -= OnDeleteAccountResult;
    }
}
 