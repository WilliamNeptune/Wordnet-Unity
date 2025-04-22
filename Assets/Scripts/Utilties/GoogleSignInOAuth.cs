using System.Collections;
using System.Collections.Generic;
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.DeepLinkAPI;
using UnityEngine;

public class GoogleSignInOAuth : BaseServiceBootstrapper
{
    [SerializeField] private ClientDataObject googleClientDataObject;
    [SerializeField] private ClientDataObject googleClientDataObjectEditor;

    protected override void RegisterServices()
    {
        OpenIDConnectService oidc = new OpenIDConnectService();
        oidc.OidcProvider = new GoogleOidcProvider();

    #if UNITY_EDITOR
        oidc.OidcProvider.ClientData = googleClientDataObjectEditor.clientData;
        oidc.RedirectURI = "https://www.polymuse.tech.com"; 
        oidc.ServerListener.ListeningUri = "http://127.0.0.1:52227/";
        
        // Improved server response handling
        oidc.ServerListener.RedirectReceived += (sender, args) =>
        {
            Debug.Log($"Received redirect with code: {args.RedirectUri}");
            if (args.RedirectUri.Contains("error="))
            {
                Debug.LogError($"OAuth error in redirect: {args.RedirectUri}");
            }
        };
    #else
        oidc.OidcProvider.ClientData = googleClientDataObject.clientData;
        oidc.RedirectURI = "com.linwood.synonyms:/"; 
    #endif
        ServiceManager.RegisterService(oidc);

        // Add this line to register the DeepLinkingService
        //ServiceManager.RegisterService(new DeepLinkingService());
    }

    protected override void UnRegisterServices()
    {

    }
}