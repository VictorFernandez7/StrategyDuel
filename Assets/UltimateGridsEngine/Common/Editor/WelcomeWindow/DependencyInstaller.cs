using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;

public class DependencyInstaller : Editor
{
    public class AfterImport : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string importedAsset in importedAssets)
            {
                if (importedAsset.Contains("DependencyInstaller.cs"))
                {
                    InstallDependencies();
                    WelcomeWindow.ShowWindow();
                }
            }
        }
    }
    public static void InstallDependencies()
    {
        bool installHappened = false;
        if (!PackageInstallation.IsInstalled(WelcomeWindow.TilemapEditorPackageID))
        {
            InstallTilemapEditor();
            installHappened = true;
        }
        if (!PackageInstallation.IsInstalled(WelcomeWindow.PostProcessingPackageID))
        {
            InstallPostProcessing();
            installHappened = true;
        }
        if (!PackageInstallation.IsInstalled(WelcomeWindow.CinemachinePackageID))
        {
            InstallCinemachine();
            installHappened = true;
        }
        if (installHappened)
        {
            AssetDatabase.Refresh();
            ReloadCurrentScene();
        }
    }
    public static void ReloadCurrentScene()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        EditorSceneManager.OpenScene(currentScenePath);
    }

    public static void InstallTilemapEditor()
    {
        if (!PackageInstallation.IsInstalled(WelcomeWindow.TilemapEditorPackageID))
        {
            PackageInstallation.Install(WelcomeWindow.TilemapEditorPackageVersionID);
        }
    }

    public static void InstallPostProcessing()
    {
        if (!PackageInstallation.IsInstalled(WelcomeWindow.PostProcessingPackageID))
        {
            PackageInstallation.Install(WelcomeWindow.PostProcessingPackageVersionID);
        }
    }

    public static void InstallCinemachine()
    {
        if (!PackageInstallation.IsInstalled(WelcomeWindow.CinemachinePackageID))
        {
            PackageInstallation.Install(WelcomeWindow.CinemachinePackageVersionID);
        }
    }

    public static void InstallTextMeshPro()
    {
        if (!PackageInstallation.IsInstalled(WelcomeWindow.TextMeshProPackageID))
        {
            PackageInstallation.Install(WelcomeWindow.TextMeshProPackageVersionID);
        }
    }

    public class DefineSymbol : Editor
    {

        private static bool ObsoleteBuild(BuildTargetGroup group)
        {
            var attributes = typeof(BuildTargetGroup).GetField(group.ToString()).GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return ((attributes.Length > 0) && (attributes != null));
        }
    }
}
public class PackageInstallation
{
    public static bool IsInstalled(string packageID)
    {
        string packagesFolder = Application.dataPath + "/../Packages/";
        string manifestFile = packagesFolder + "manifest.json";
        string manifest = File.ReadAllText(manifestFile);
        return manifest.Contains(packageID);
    }
    public static void Install(string packageVersionID)
    {
        Client.Add(packageVersionID);
    }
}