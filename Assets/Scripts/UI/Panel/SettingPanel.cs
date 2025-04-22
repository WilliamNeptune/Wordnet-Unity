using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.Serialization;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.iOS;
using UnityEngine.Networking;
using System.IO;
using System.Threading.Tasks;
using System;

public class SettingPanel : ParentPanel
{
    public static SettingPanel instance;
    [SerializeField] Text userEmail;
    [SerializeField] Button[] btnSettings;
    [SerializeField] Button btnSign;
    [SerializeField] Image icon;
    [SerializeField] Text currentVersion;
    private bool needsUIUpdate = false;
    private Coroutine updateUICoroutine;
    private Coroutine updateProfileImageCoroutine;
    void Start()
    {
        base.Start();
        instance = this;
        for (int i = 0; i < btnSettings.Length; i++)
        {
            int index = i;
            btnSettings[index].onClick.AddListener(() =>
            {
                OnClickSettingButton(index + 1);
            });
        } 

        //UpdateUI();
        btnSign.onClick.AddListener(() =>
        {
            if (LoginManager.instance.IsUserLoggedIn())
            {
                OnClickSettingButton((int)Define.SettingPage.AccountSettings);
            }
            else
            {
                Debug.Log("Now is not logged in, OnClickSettingButton");
                OnClickSettingButton((int)Define.SettingPage.Login);
            }
        });
        LoginManager.instance.OnLoginResult += UpdateLoginResult;
        //LoginManager.instance.OnDeleteAccountResult += UpdateLoginResult;

        currentVersion.text =  "SynonymNet Version: " + SynonymData.GetCurrentVersion();
        UpdateUI();
    }

    void OnEnable()
    {
        UpdateUI();
    }
    void UpdateLoginResult(bool success, string message)
    {
        Debug.Log("From SettingPanel, Update SettingPanel LoginResult:  " + message);
        needsUIUpdate = true;
    }

    void OnClickSettingButton(int index)
    {
        if (index == (int)Define.SettingPage.Rating)
        {
            RequestReview();
        }

        string[] panelName = {
            "LoginPanel",
            "SubscribePanel",
            "LangSettingPanel",
            "ColorModeSettingPanel",
            "RatingPanel",
            "FeedbackPanel",
            "AboutPanel",
            "AccountSettingsPanel"};

        var gameObject = PanelManager.CreatePanel(ContainerPanel.instance.transform, panelName[index], new Vector2(1171, 0), true);
        PanelManager.openPanelHorizontally(gameObject, null, 0);
    
        string[] titles = {
            "LOGIN",
            "SUBSCRIBE",
            "LANG",
            "COLOR_MODE",
            "",
            "FEEDBACK",
            "ABOUT",
            "ACCOUNT_SETTINGS"};
        ContainerPanel.instance.SetTitle(titles[index], true);
    }

    public void UpdateUI()
    {
        updateUICoroutine = StartCoroutine(UpdateUIWhenReady());
    }

    private IEnumerator UpdateUIWhenReady()
    {
        // Wait for next frame to ensure UI components are ready
        yield return null;

        bool isLoggedIn = LoginManager.instance.IsUserLoggedIn();
        if (isLoggedIn)
        {
            string displayName = LoginManager.instance.GetUserName();
            if (userEmail != null) userEmail.text = displayName;
            if (btnSign != null) Functions.setTextStr(btnSign.GetComponentInChildren<Text>(), "ACCOUNT_SETTINGS");
            Debug.Log("From SettingPanel, User has been logged in: " + displayName);
            if (icon != null && !LoginManager.instance.isLoadedProfile)
            {
                icon.sprite = Resources.Load<Sprite>("Prefabs/UI/Images/icon_line_app_40");
                icon.color = Functions.getProspectColor();
                icon.SetNativeSize();
            }
            if (!LoginManager.instance.isLoadedProfile)
            {
                TryToGetUserProfileImage();
                LoginManager.instance.isLoadedProfile = true;
            }
        }
        else
        {
            if (userEmail != null) Functions.setTextStr(userEmail, "SIGN_UP");
            Debug.Log("From SettingPanel, User has been logged out: ");
            if (btnSign != null) Functions.setTextStr(btnSign.GetComponentInChildren<Text>(), "SIGN_IN");
            if (icon != null)
            {
                icon.sprite = Resources.Load<Sprite>("Prefabs/UI/Images/icon_line_app_26");
                icon.color = Functions.getProspectColor();
                icon.SetNativeSize();
            }
        }
    }
    private void TryToGetUserProfileImage()
    {
        string photoUrl = LoginManager.instance.GetUserProfileUrl();
        if (string.IsNullOrEmpty(photoUrl)) return;
        updateProfileImageCoroutine = StartCoroutine(LoadProfileImage(photoUrl));
    }
    private IEnumerator LoadProfileImage(string photoUrl)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(photoUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                if (icon != null)
                {
                    icon.sprite = sprite;
                    icon.SetNativeSize();
                    icon.color = Color.white;
                }
            }
            else
            {
                Debug.LogError($"Failed to load profile image: {request.error}");
            }
        }
    }
    private void Update()
    {
        if (needsUIUpdate)
        {
            needsUIUpdate = false;
            UpdateUI();
        }
    }

    void OnDisable()
    {
        if (updateUICoroutine != null)
        {
            StopCoroutine(updateUICoroutine);
            updateUICoroutine = null;
        }
        if (updateProfileImageCoroutine != null)
        {
            StopCoroutine(updateProfileImageCoroutine);
            updateProfileImageCoroutine = null;
        }
    }
    void RequestReview()
    {
        if (!SystemManager.Instance.getReviewRequested())
        {
            bool popupShown = Device.RequestStoreReview();
            if (popupShown)
            {
                // The review popup was presented to the user, set "reviewRequested" to "true" to reflect that
                // Note: there's no way to check if the user actually gave a review for the app or cancelled the popup.
                SystemManager.Instance.saveReviewRequested(true);
            }
            else
            {
                // The review popup wasn't presented. Log a message and reset "reviewRequested" so you can revisit this in the future.
                Debug.Log("iOS version is too low or StoreKit framework was not linked.");
            }
        }
    }
}
