using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginByEmailPanel : MonoBehaviour
{
    public InputField emailInput;
    public InputField passwordInput;
    public InputField confirmPasswordInput;
    public Button loginButton;
    public Button registerButton;
    public Text errorText;
    void Start()
    {
        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
    }

    public void OnLoginButtonClick()
    {
        //LoginManager.instance.TryLoginUser(emailInput.text, passwordInput.text);
    }

    public void OnRegisterButtonClick()
    {
        //LoginManager.instance.TryRegisterUser(emailInput.text, passwordInput.text, confirmPasswordInput.text);
    }
}
