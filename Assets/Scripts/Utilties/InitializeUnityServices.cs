using System;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

public class InitializeUnityServices : MonoBehaviour
{
    public string environment = "production";

    async void Start()
    {
        try
        {
            var options = new InitializationOptions()
                .SetEnvironmentName(environment)
                .SetOption("auto-sign-in", "false")  // Prevent auto sign-in
                .SetOption("disableAutoSignIn", "true")
                .SetOption("disable_auto_sign_in", "true");

            await UnityServices.InitializeAsync(options);
        }
        catch (Exception exception)
        {
            // An error occurred during services initialization.
        }
    }
}