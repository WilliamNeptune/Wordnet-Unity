using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;  
using UnityEngine.UI;

public class AccountSettingPanel : ParentPanel
{
    [SerializeField]
    private Button deleteAccountButton;
    [SerializeField]
    private Button signOutButton;

    void Start()
    {
        base.Start();
        deleteAccountButton.onClick.AddListener(OnDeleteAccountButtonClick);
        signOutButton.onClick.AddListener(OnSignOutButtonClick);    
    }
    
    private void OnDeleteAccountButtonClick()
    {
        var gameObject = PanelManager.CreatePanel(ContainerPanel.instance.transform, "ConfirmCancelPanel", new Vector2(0, -2000f), true);
        PanelManager.OpenPanelVertically(gameObject, null, 0f);
    }

    public void OnSignOutButtonClick()
    {
        Debug.Log("Now is logged in, LogoutAllProviders");
        LoginManager.instance.LogoutAllProviders();
        PanelManager.closePanelHorizontally(gameObject, null, 0f);
    }


}
