using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using Firebase.Auth;
using Firebase;
using Firebase.Extensions;
using System.Threading.Tasks;
using System;
using i5.Toolkit.Core.DeepLinkAPI;
// using Google;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.OpenIDConnectClient;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using AppleAuth.Interfaces;

using System.Threading;
using AppleAuth.Enums;
using AppleAuth.Native;
using AppleAuth;


public class LoginManager : MonoBehaviour
{
    public static LoginManager instance;
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    private IAppleAuthManager appleAuthManager;
    public event Action<bool, string> OnLoginResult;
    public event Action<bool, string, bool> OnRegisterResult;
    public event Action<bool, string> OnDeleteAccountResult;
    //public event Action<bool, string> OnAppleSignInResult;
    public bool isLoadedProfile = false;
    void Awake()
    {
        instance = this;
        auth = FirebaseAuth.DefaultInstance;
        currentUser = auth.CurrentUser;
    }

    void Start()
    {
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            this.appleAuthManager = new AppleAuthManager(deserializer);
        }
        //OnAppleSignInResult += HandleAppleSignInResult;
    }
    void Update()
    {
        if (this.appleAuthManager != null)
        {
            this.appleAuthManager.Update();
        }
    }
    #region Email and password
    public async void TryRegisterUser(string email, string password, string confirmPassword)
    {
        if (email == "" || password == "")
        {
            OnRegisterResult?.Invoke(false, "REGISTER_FAILED_MESSAGE_1", true);
            return;
        }

        if (!IsValidEmail(email))
        {
            OnRegisterResult?.Invoke(false, "REGISTER_FAILED_MESSAGE_3", true);
            return;
        }

        if (!IsValidPassword(password))
        {
            OnRegisterResult?.Invoke(false, "REGISTER_FAILED_MESSAGE_4", true);
            return;
        }

        if (password != confirmPassword)
        {
            //Debug.LogError("Passwords do not match");
            OnRegisterResult?.Invoke(false, "REGISTER_FAILED_MESSAGE_2", true);
            return;
        }

        bool IsValidEmail(string email)
        {
            return email.Contains("@") && email.Contains(".") && !ContainsSpecialCharacters(email);
        }

        bool IsValidPassword(string password)
        {
            return password.Length >= 6 && !ContainsSpecialCharacters(password);
        }

        bool ContainsSpecialCharacters(string input)
        {
            string specialCharacters = @"!#$%^&*()_+{}[]|\\:;'<>,?/~`";
            return input.IndexOfAny(specialCharacters.ToCharArray()) != -1;
        }

        var result = await RegisterUser(email, password);
        if (result.success)
        {
            currentUser = auth.CurrentUser;
            OnRegisterResult?.Invoke(true, "REGISTER_SUCCESS_MESSAGE", true);
            SendEmailForVerification();
        }
        else
        {
            OnRegisterResult?.Invoke(false, result.message, false);
        }
    }

    private async void SendEmailForVerification()
    {
        var user = auth.CurrentUser;
        if (user != null)
        {
            try
            {
                await user.SendEmailVerificationAsync();
                Debug.Log("Email sent successfully");
            }
            catch (FirebaseException e)
            {
                AuthError errorCode = (AuthError)e.ErrorCode;
                switch (errorCode)
                {
                    case AuthError.InvalidEmail:
                        Debug.LogError("The email address is badly formatted.");
                        break;
                    default:
                        Debug.LogError("Error sending email: " + e.Message);
                        break;
                }
            }
        }
    }

    public async void TryLoginUser(string email, string password)
    {
        var result = await LoginUser(email, password);
        if (result.success)
        {
            currentUser = auth.CurrentUser;
            if (currentUser.IsEmailVerified)
            {
                ServerManager.instance.TryToUploadClientInfo();
                OnLoginResult?.Invoke(true, result.message);
            }
            else
            {
                SendEmailForVerification();
                OnLoginResult?.Invoke(false, "Email not verified, check your email for a verification link.");
            }


        }
        else
        {
            OnLoginResult?.Invoke(false, result.message);
        }
    }

    public void SignOutFirebaseAuth()
    {
        if (auth.CurrentUser != null)
        {
            auth.SignOut();
            currentUser = auth.CurrentUser;
            OnLoginResult?.Invoke(false, "Firebase User logged out");
        }
    }

    public async Task<(bool success, string message)> RegisterUser(string email, string password)
    {
        try
        {
            var result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            return (true, $"User registered successfully: {result.User.Email}");
        }
        catch (FirebaseException e)
        {
            AuthError errorCode = (AuthError)e.ErrorCode;
            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    return (false, "The email address is already in use by another account.");
                case AuthError.InvalidEmail:
                    return (false, "The email address is badly formatted.");
                case AuthError.WeakPassword:
                    return (false, "The password is too weak. Please use a stronger password.");
                case AuthError.OperationNotAllowed:
                    return (false, "User registration is not enabled. Please contact the administrator.");
                default:
                    return (false, $"Registration failed: {e.Message}");
            }
        }
    }

    public async Task<(bool success, string message)> LoginUser(string email, string password)
    {
        try
        {
            var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            return (true, $"User logged in successfully: {result.User.Email}");
        }
        catch (FirebaseException e)
        {
            AuthError errorCode = (AuthError)e.ErrorCode;
            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                    return (false, "The email address is badly formatted.");
                case AuthError.WrongPassword:
                    return (false, "The password is incorrect. Please try again.");
                case AuthError.UserNotFound:
                    return (false, "There is no user record corresponding to this email. The user may have been deleted.");
                case AuthError.UserDisabled:
                    return (false, "The user account has been disabled by an administrator.");
                case AuthError.TooManyRequests:
                    return (false, "Too many unsuccessful login attempts. Please try again later.");
                default:
                    return (false, $"Login failed: {e.Message}");
            }
        }
    }
    #endregion

    #region Google Sign-In

    [DeepLink("oauth2redirect")]
    public void HandleDeepLink(DeepLinkArgs args)
    {
        //Debug.Log($"Deep link activated: {args.Uri}");
        var oidcService = ServiceManager.GetService<OpenIDConnectService>();
        oidcService.HandleActivation(args);
    }
    // public void OnDisable()
    // {
    //     var deepLinkingService = ServiceManager.GetService<DeepLinkingService>();
    //     deepLinkingService.RemoveDeepLinkListener(this);
    // }

    public async Task SignInWithGoogle()
    {
        var oidcService = ServiceManager.GetService<OpenIDConnectService>();
        oidcService.LoginCompleted += OnLoginComplete;

        //var deepLinkingService = ServiceManager.GetService<DeepLinkingService>();
        //deepLinkingService.AddDeepLinkListener(this);

        await oidcService.OpenLoginPageAsync();
    }
    private void OnLoginComplete(object sender, EventArgs e)
    {
        Debug.Log("Login complete");
        string accessToken = ServiceManager.GetService<OpenIDConnectService>().AccessToken;

        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(null, accessToken);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith
        (
            task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                    return;
                }

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
                currentUser = auth.CurrentUser;
                ServerManager.instance.TryToUploadClientInfo();
                OnLoginResult?.Invoke(true, "Google User logged in successfully");

            }
        );
    }

    public void LogOutGoogleAccount()
    {
        // Unsubscribe from the login event
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted -= OnLoginComplete;

        // Logout from OpenID Connect service
        var oidcService = ServiceManager.GetService<OpenIDConnectService>();
        oidcService.LogoutCompleted += OnLogoutComplete;

        // Clear the Google session by redirecting to Google's logout endpoint
        // string googleLogoutUrl = "https://accounts.google.com/logout";
        // Application.OpenURL(googleLogoutUrl);

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
        OnLoginResult?.Invoke(false, "Google User logged out");
    }

    public string GetUserProfileUrl()
    {
        if (currentUser != null)
        {
            string photoUrl = currentUser.PhotoUrl?.ToString() ?? "";
            return photoUrl;
        }
        return "";
    }
    #endregion

    #region Apple Sign-In

    // Your Firebase authentication client
    private void PerformFirebaseAuthentication(IAppleIDCredential appleIdCredential, string rawNonce, Action<FirebaseUser> firebaseAuthCallback)
    {
        try
        {
            Debug.Log("Starting Firebase Authentication with Apple");

            // Debug Apple credential information
            Debug.Log($"Apple Credential - Email: {appleIdCredential.Email}");
            if (appleIdCredential.FullName != null)
            {
                Debug.Log($"Full Name Available - Given: {appleIdCredential.FullName.GivenName}, Family: {appleIdCredential.FullName.FamilyName}");
            }
            else
            {
                Debug.Log("Full Name is null in Apple Credential");
            }

            var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
            var firebaseCredential = OAuthProvider.GetCredential(
            "apple.com",
            identityToken,
                rawNonce,
                authorizationCode);

            this.auth.SignInWithCredentialAsync(firebaseCredential)
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        Debug.Log($"Firebase Sign-in successful. User ID: {task.Result.UserId}");

                        if (appleIdCredential.FullName != null)
                        {
                            string displayName = $"{appleIdCredential.FullName.GivenName} {appleIdCredential.FullName.FamilyName}".Trim();
                            Debug.Log($"Attempting to update display name to: {displayName}");

                            if (!string.IsNullOrEmpty(displayName))
                            {
                                var profileUpdate = new UserProfile { DisplayName = displayName };
                                task.Result.UpdateUserProfileAsync(profileUpdate).ContinueWith(updateTask =>
                                {
                                    if (updateTask.IsCompleted && !updateTask.IsFaulted)
                                    {
                                        Debug.Log($"Successfully updated user display name to: {displayName}");
                                    }
                                    else if (updateTask.IsFaulted)
                                    {
                                        Debug.LogError($"Failed to update display name: {updateTask.Exception}");
                                    }
                                });
                            }
                        }
                        else
                        {
                            Debug.Log("No name information available from Apple to update profile");
                        }
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError($"Firebase Sign-in failed: {task.Exception}");
                    }

                    HandleSignInWithUser(task, firebaseAuthCallback);
                });
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in PerformFirebaseAuthentication: {ex.Message}\nStackTrace: {ex.StackTrace}");
            firebaseAuthCallback(null);
        }
    }
    public void PerformLoginWithAppleIdAndFirebase(Action<FirebaseUser> firebaseAuthCallback)
    {
        Debug.Log("Starting Apple Sign In Process");

        var rawNonce = GeneratingNonces.GenerateRandomString(32);
        var nonce = GeneratingNonces.GenerateSHA256NonceFromRawNonce(rawNonce);

        // Explicitly request both email and full name
        var loginArgs = new AppleAuthLoginArgs(
            LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
            nonce);

        Debug.Log("Requesting Apple login with full name and email");

        this.appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                Debug.Log("Received credential from Apple");
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    Debug.Log("Successfully got Apple ID credential");
                    if (appleIdCredential.FullName != null)
                    {
                        Debug.Log($"Full Name in credential - Given: {appleIdCredential.FullName.GivenName}, Family: {appleIdCredential.FullName.FamilyName}");
                    }
                    else
                    {
                        Debug.Log("Full Name is null in credential despite requesting it");
                    }
                    this.PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
                }
                else
                {
                    Debug.LogError("Failed to get Apple ID credential");
                }
            },
            error =>
            {
                Debug.LogError($"Error during Apple Sign In: {error}");
                firebaseAuthCallback(null);
            });
    }

    public void PerformQuickLoginWithFirebase(Action<FirebaseUser> firebaseAuthCallback)
    {
        var rawNonce = GeneratingNonces.GenerateRandomString(32);
        var nonce = GeneratingNonces.GenerateSHA256NonceFromRawNonce(rawNonce);

        var quickLoginArgs = new AppleAuthQuickLoginArgs(nonce);

        this.appleAuthManager.QuickLogin(
            quickLoginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    this.PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
                }
            },
            error =>
            {
                // Something went wrong
            });
    }

    public void HandleAppleSignInResult(FirebaseUser user)
    {
        if (user != null)
        {
            currentUser = auth.CurrentUser;
            ServerManager.instance.TryToUploadClientInfo();
            OnLoginResult?.Invoke(true, "Apple User logged in successfully");
        }
        else
        {
            OnLoginResult?.Invoke(false, "Apple User login failed");
        }
    }

    private void HandleSignInWithUser(Task<FirebaseUser> task, Action<FirebaseUser> firebaseUserCallback)
    {
        if (task.IsCanceled)
        {
            OnLoginResult?.Invoke(false, "Firebase auth was canceled");
            firebaseUserCallback(null);
        }
        else if (task.IsFaulted)
        {
            OnLoginResult?.Invoke(false, "Firebase auth failed");
            firebaseUserCallback(null);
        }
        else
        {
            var firebaseUser = task.Result;
            OnLoginResult?.Invoke(true, "Firebase auth completed | User ID:" + firebaseUser.UserId);
            firebaseUserCallback(firebaseUser);
        }
    }
    #endregion

    #region Common Functions
    public string GetUserDisplayName()
    {
        return auth.CurrentUser?.DisplayName;
    }

    public string GetUserEmail()
    {
        return auth.CurrentUser?.Email;
    }

    public string GetUserName()
    {
        string displayName = GetUserDisplayName();
        if(displayName == null || displayName == "")
        {
            return GetUserEmail();
        }
        return displayName;
    }

    public string GetUserId()
    {
        return auth.CurrentUser?.UserId ?? "";
    }

    public bool IsUserLoggedIn()
    {
        // if (auth.CurrentUser != null)
        // {
        //     Debug.Log("From LoginManager, User is logged in");
        // }
        // else
        // {
        //     Debug.Log("From LoginManager, User is not logged in");
        // }
        // return auth.CurrentUser != null;
        return false;
    }

    public bool IsUserEmailVerified()
    {
        return auth.CurrentUser?.IsEmailVerified ?? false;
    }

    public FirebaseUser GetCurrentUser()
    {
        return auth.CurrentUser;
    }
    public void showProviderInfo()
    {
        var providerData = auth.CurrentUser.ProviderData;
        foreach (var item in providerData)
            Debug.Log(item.ProviderId);
    }
    public string getProviderID()
    {
        if (auth == null || auth.CurrentUser == null)
        {
            Debug.Log("User is not logged in");
            return "";
        }
        string id = auth.CurrentUser.ProviderId;
        Debug.Log(id);
        return id;
    }

    public void LogoutAllProviders()
    {
        var providerData = auth.CurrentUser.ProviderData;
        foreach (var item in providerData)
        {
            Debug.Log(item.ProviderId);
            switch (item.ProviderId)
            {
                case "google.com":
                    LogOutGoogleAccount();
                    break;
                case "apple.com":
                    break;
                case "password":
                default:
                    break;
            }
        }
        SignOutFirebaseAuth();
        isLoadedProfile = false;
    }

    #endregion

    #region Account Deletion
    public async void TryDeleteAccount()
    {
        try 
        {
            Firebase.Auth.FirebaseUser user = auth.CurrentUser;

            await user.DeleteAsync();
            ServerManager.instance.TryToDeleteAccountFromServer();
            currentUser = null;
            isLoadedProfile = false;
            
            OnDeleteAccountResult?.Invoke(true, "Account deleted successfully");
            
            await Task.Delay(100); 
        
        }
        catch (Exception e)
        {
            Debug.LogError($"Error deleting account: {e.Message}");
            OnDeleteAccountResult?.Invoke(false, $"Failed to delete account: {e.Message}");
        }
    }

    // Helper method for re-authentication if needed
    // public async Task<(bool success, string message)> ReauthenticateUser()
    // {
    //     try 
    //     {
    //         if (auth.CurrentUser == null)
    //         {
    //             return (false, "No user is currently logged in");
    //         }

    //         // Get the provider ID to determine authentication method
    //         var providerData = auth.CurrentUser.ProviderData;
    //         if (providerData.Count == 0)
    //         {
    //             return (false, "No authentication provider found");
    //         }

    //         string providerId = providerData[0].ProviderId;
            
    //         switch (providerId)
    //         {
    //             case "password": // Email authentication
    //                 // You'll need to get these values from a UI prompt
    //                 string email = ""; // Get from UI input
    //                 string password = ""; // Get from UI input
    //                 var emailCredential = EmailAuthProvider.GetCredential(email, password);
    //                 await auth.CurrentUser.ReauthenticateAsync(emailCredential);
    //                 break;

    //             case "google.com":
    //                 try
    //                 {
    //                     var oidcService = ServiceManager.GetService<OpenIDConnectService>();
    //                     await oidcService.OpenLoginPageAsync();
    //                     string accessToken = ServiceManager.GetService<OpenIDConnectService>().AccessToken;
    //                     var googleCredential = GoogleAuthProvider.GetCredential(null, accessToken);
    //                     await auth.CurrentUser.ReauthenticateAsync(googleCredential);
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Debug.LogError($"Google reauthentication failed: {ex.Message}");
    //                     return (false, "Failed to reauthenticate with Google");
    //                 }
    //                 break;

    //             case "apple.com":
    //                 try
    //                 {
    //                     var tcs = new TaskCompletionSource<(bool success, string message)>();
                        
    //                     var rawNonce = GeneratingNonces.GenerateRandomString(32);
    //                     var nonce = GeneratingNonces.GenerateSHA256NonceFromRawNonce(rawNonce);

    //                     var loginArgs = new AppleAuthLoginArgs(
    //                         LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
    //                         nonce);

    //                     this.appleAuthManager.LoginWithAppleId(
    //                         loginArgs,
    //                         credential =>
    //                         {
    //                             var appleIdCredential = credential as IAppleIDCredential;
    //                             if (appleIdCredential != null)
    //                             {
    //                                 var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
    //                                 var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
    //                                 var firebaseCredential = OAuthProvider.GetCredential(
    //                                     "apple.com",
    //                                     identityToken,
    //                                     rawNonce,
    //                                     authorizationCode);

    //                                 auth.CurrentUser.ReauthenticateAsync(firebaseCredential)
    //                                     .ContinueWith(task =>
    //                                     {
    //                                         if (task.IsFaulted)
    //                                         {
    //                                             tcs.SetResult((false, "Apple reauthentication failed"));
    //                                         }
    //                                         else
    //                                         {
    //                                             tcs.SetResult((true, "Reauthentication successful"));
    //                                         }
    //                                     });
    //                             }
    //                             else
    //                             {
    //                                 tcs.SetResult((false, "Failed to get Apple credential"));
    //                             }
    //                         },
    //                         error =>
    //                         {
    //                             tcs.SetResult((false, $"Apple Sign In failed: {error}"));
    //                         });

    //                     return await tcs.Task;
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Debug.LogError($"Apple reauthentication failed: {ex.Message}");
    //                     return (false, "Failed to reauthenticate with Apple");
    //                 }
    //                 break;

    //             default:
    //                 return (false, $"Unsupported authentication provider: {providerId}");
    //         }

    //         return (true, "Reauthentication successful");
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"Reauthentication failed: {e.Message}");
    //         return (false, $"Reauthentication failed: {e.Message}");
    //     }
    // }
    #endregion
}
