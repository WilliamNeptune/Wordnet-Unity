using UnityEngine;
using AppleAuth.Editor;
using UnityEditor.Build;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEditor.Build.Reporting;

public static class SignInWithApplePostprocessor 
{
    [PostProcessBuild(1)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        var projectPath = PBXProject.GetPBXProjectPath(path);
        var project = new PBXProject();
        project.ReadFromString(System.IO.File.ReadAllText(projectPath));
        var manager = new ProjectCapabilityManager(projectPath, "Entitlements.entitlements", null, project.GetUnityMainTargetGuid());
        manager.AddSignInWithAppleWithCompatibility();
        manager.WriteToFile();

        string targetGuid = project.GetUnityMainTargetGuid();
        project.AddFrameworkToProject(targetGuid, "AuthenticationServices.framework", false);
        project.AddFrameworkToProject(targetGuid, "AdSupport.framework", false);
        project.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);
        project.WriteToFile(projectPath);
    }
    [PostProcessBuild(2)]
    public static void OnPostProcessBuildMac(BuildTarget target, string path)
    {
        if (target != BuildTarget.StandaloneOSX)
            return;

        AppleAuthMacosPostprocessorHelper.FixManagerBundleIdentifier(target, path);
    }
}
