using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
public class FirebaseAuthManager : MonoBehaviour
{
    private Firebase.Auth.FirebaseAuth auth;
    // Start is called before the first frame update
    void Start()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SignIn()
    {
        SignInWithGoogle();
    }

    public void SignOut()
    {
        Logout();
    }

    private async Task SignInWithGoogle()
    {
           ServiceManager.GetService<OpenIDConnectService>().LoginCompleted+= OnLoginComplete;
           await ServiceManager.GetService<OpenIDConnectService>().OpenLoginPageAsync();
    }

    private void OnLoginComplete(object sender, EventArgs e)
    {
        Debug.Log("Login complete");
        string accessToken = ServiceManager.GetService<OpenIDConnectService>().AccessToken;

        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(null, accessToken);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task => {
                if (task.IsCanceled) {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            });
    }

    public void Logout()
    {
        // Unsubscribe from the login event
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted -= OnLoginComplete;
        
        // Sign out from Firebase
        if (auth != null) {
            auth.SignOut();
        }
        
        // Logout from OpenID Connect service
        var oidcService = ServiceManager.GetService<OpenIDConnectService>();
        oidcService.LogoutCompleted += OnLogoutComplete;
        
        // Clear the Google session by redirecting to Google's logout endpoint
        string googleLogoutUrl = "https://accounts.google.com/logout";
        Application.OpenURL(googleLogoutUrl);
        
        // Then perform the OIDC logout
        oidcService.Logout();
        
        // Clear any stored tokens
        PlayerPrefs.DeleteKey("AccessToken");  // If you're storing the token
        PlayerPrefs.Save();
    }

    private void OnLogoutComplete(object sender, EventArgs e)
    {
        Debug.Log("Logout completed");
        // Unsubscribe from the logout event to prevent memory leaks
        ServiceManager.GetService<OpenIDConnectService>().LogoutCompleted -= OnLogoutComplete;
        
        // Force clear any cookies if you're using WebView
        #if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        using (var webView = new AndroidJavaObject("android.webkit.WebView", activity))
                        {
                            webView.Call("clearCache", true);
                            webView.Call("clearHistory");
                            using (var cookieManager = new AndroidJavaClass("android.webkit.CookieManager").CallStatic<AndroidJavaObject>("getInstance"))
                            {
                                cookieManager.Call("removeAllCookies", null);
                            }
                        }
                    }
                }
            }
        #endif
    }
}
