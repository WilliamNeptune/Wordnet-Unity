using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class LoginByEmailAndPasswordPanel : ParentPanel
{
    [SerializeField] private InputField emailInput;
    [SerializeField] private InputField passwordInput;
    [SerializeField] private InputField confirmPasswordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button promptToRegisterButton;
    [SerializeField] private Text infoText;

    private bool isRegistering = false;
    void Start()
    {
        UpdateUI();
        loginButton.onClick.AddListener(OnLoginButtonClick);
        registerButton.onClick.AddListener(OnRegisterButtonClick);
        promptToRegisterButton.onClick.AddListener(OnPromptToRegisterButtonClick);

        LoginManager.instance.OnLoginResult += HandleLoginResult;
        LoginManager.instance.OnRegisterResult += HandleRegisterResult;

        passwordInput.contentType = InputField.ContentType.Password;
        confirmPasswordInput.contentType = InputField.ContentType.Password;
    }

    void OnDestroy()
    {
        LoginManager.instance.OnLoginResult -= HandleLoginResult;
        LoginManager.instance.OnRegisterResult -= HandleRegisterResult;
    }

    private void OnPromptToRegisterButtonClick()
    {
        isRegistering = !isRegistering;
        UpdateUI();
    }

    private void UpdateUI()
    {
        infoText.gameObject.SetActive(false);

        emailInput.gameObject.SetActive(true);
        passwordInput.gameObject.SetActive(true);
        confirmPasswordInput.gameObject.SetActive(isRegistering);

        loginButton.gameObject.SetActive(!isRegistering);
        registerButton.gameObject.SetActive(isRegistering);

        emailInput.text = "";
        passwordInput.text = "";
        confirmPasswordInput.text = "";

        if (isRegistering)
        {
            Functions.setTextStr(promptToRegisterButton.GetComponentInChildren<Text>(), "RETURN_TO_LOGIN");
        }
        else
        {
            Functions.setTextStr(promptToRegisterButton.GetComponentInChildren<Text>(), "REGISTER_AN_ACCOUNT");
        }
    }

    private void SetButtonsVisible(bool visible)
    {
        loginButton.gameObject.SetActive(visible);
        registerButton.gameObject.SetActive(visible);
        confirmPasswordInput.gameObject.SetActive(visible);
        emailInput.gameObject.SetActive(visible);
        passwordInput.gameObject.SetActive(visible);
    }

    private void SetInfoTextVisible(bool visible)
    {
        infoText.gameObject.SetActive(visible);
    }

    public void OnLoginButtonClick()
    {
        LoginManager.instance.TryLoginUser(emailInput.text, passwordInput.text);
    }

    public void OnRegisterButtonClick()
    {
        LoginManager.instance.TryRegisterUser(emailInput.text, passwordInput.text, confirmPasswordInput.text);
    }

    public void SetInfoText(string text, Color color)
    {
        Functions.setTextStr(infoText, text);
        infoText.color = color;
    }

    private void HandleRegisterResult(bool success, string message, bool UseText)
    {
        if (success)
        {
            SetInfoTextVisible(true);
            SetInfoText("VERIFY_EMAIL", new Color(0, 0, 0));
            SetButtonsVisible(false);
        }
        else
        {
            SetInfoTextVisible(true);
            if (UseText)
            {
                SetInfoText(message, new Color(1, 0, 0));
            }
            else
            {
                infoText.text = message;
            }
        }
    }

    private void HandleLoginResult(bool success, string message)
    {
        if (success)
        {
            PanelManager.CloseTopPanel();
        }
        else
        {
            SetInfoTextVisible(true);
            SetInfoText("LOGIN_FAILED", new Color(1, 0, 0));
        }
    }

}
