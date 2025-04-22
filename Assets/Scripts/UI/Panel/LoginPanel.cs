using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private Button loginByEmailButton;
    [SerializeField] private Button loginByGoogleButton;
    [SerializeField] private Button loginByAppleButton;

    void Start()
    {
        loginByEmailButton.onClick.AddListener(OnLoginByEmailButtonClick);
        loginByGoogleButton.onClick.AddListener(OnLoginByGoogleButtonClick);
        loginByAppleButton.onClick.AddListener(OnLoginByAppleButtonClick);
    }

    private void OnLoginByEmailButtonClick()
    {
        PanelManager.CloseTopPanel();
        PanelManager.CreatePanel(ContainerPanel.instance.transform, "LoginByEmailAndPasswordPanel", new Vector2(0, 0), true);
    }

    private void OnLoginByGoogleButtonClick()
    {
        PanelManager.CloseTopPanel();
        LoginManager.instance.SignInWithGoogle();
    }

    private void OnLoginByAppleButtonClick()
    {
        PanelManager.CloseTopPanel();
        LoginManager.instance.PerformLoginWithAppleIdAndFirebase(LoginManager.instance.HandleAppleSignInResult);

    }   
}
