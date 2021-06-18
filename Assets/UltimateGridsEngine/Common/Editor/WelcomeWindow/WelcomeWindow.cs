using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

public class WelcomeWindow : EditorWindow
{
    public const string TilemapEditorPackageID = "com.unity.2d.tilemap";
    public const string TilemapEditorPackageVersionID = "com.unity.2d.tilemap@1.0.0";
    public const string PostProcessingPackageID = "com.unity.postprocessing";
    public static string PostProcessingPackageVersionID = "com.unity.postprocessing@2.1.7";
    public const string CinemachinePackageID = "com.unity.cinemachine";
    public static string CinemachinePackageVersionID = "com.unity.cinemachine@2.5.0";
    public const string TextMeshProPackageID = "com.unity.textmeshpro";
    public static string TextMeshProPackageVersionID = "com.unity.textmeshpro@2.0.1";
    public const string DOCUMENTATION_URL = "https://gabodevelops.github.io/UGE-Documentation.html";

    private static readonly int WelcomeWindowWidth = 488;
    private static readonly int WelcomeWindowHeight = 700;

    private static GUIStyle _largeTextStyle;
    public static GUIStyle LargeTextStyle
    {
        get
        {
            if (_largeTextStyle == null)
            {
                _largeTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                {
                    richText = true,
                    wordWrap = true,
                    fontStyle = FontStyle.Bold,
                    fontSize = 14,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
                };
            }
            return _largeTextStyle;
        }
    }
    private static GUIStyle _regularTextStyle;
    public static GUIStyle RegularTextStyle
    {
        get
        {
            if (_regularTextStyle == null)
            {
                _regularTextStyle = new GUIStyle(UnityEngine.GUI.skin.label)
                {
                    richText = true,
                    wordWrap = true,
                    fontStyle = FontStyle.Normal,
                    fontSize = 12,
                    alignment = TextAnchor.MiddleLeft,
                    padding = new RectOffset() { left = 0, right = 0, top = 0, bottom = 0 }
                };
            }
            return _regularTextStyle;
        }
    }
    private static GUIStyle _footerTextStyle;
    public static GUIStyle FooterTextStyle
    {
        get
        {
            if (_footerTextStyle == null)
            {
                _footerTextStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
                {
                    alignment = TextAnchor.LowerCenter,
                    wordWrap = true,
                    fontSize = 12
                };
            }
            return _footerTextStyle;
        }
    }
    [MenuItem("Tools/GaboDevelops/Welcome to the Ultimate Grids Engine", false, 0)]
    public static void ShowWindow()
    {
        EditorWindow editorWindow = GetWindow(typeof(WelcomeWindow), false, " Welcome", true);
        editorWindow.autoRepaintOnSceneChange = true;
        editorWindow.titleContent.image = EditorGUIUtility.IconContent("_Help").image;
        editorWindow.maxSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
        editorWindow.minSize = new Vector2(WelcomeWindowWidth, WelcomeWindowHeight);
        editorWindow.position = new Rect(Screen.width / 2 + WelcomeWindowWidth / 2, Screen.height / 2, WelcomeWindowWidth, WelcomeWindowHeight);
        editorWindow.Show();
    }

    private void OnGUI()
    {
        if (EditorApplication.isCompiling)
        {
            this.ShowNotification(new GUIContent("Compiling Scripts", EditorGUIUtility.IconContent("BuildSettings.Editor").image));
        }
        else
        {
            this.RemoveNotification();
        }
        Texture2D welcomeImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/UltimateGridsEngine/Common/Editor/WelcomeWindow/welcome-banner-v2.png", typeof(Texture2D));
        Rect welcomeImageRect = new Rect(0, 0, 488, 325);
        UnityEngine.GUI.DrawTexture(welcomeImageRect, welcomeImage);
        GUILayout.Space(345);

        GUILayout.BeginArea(new Rect(EditorGUILayout.GetControlRect().x + 10, 345, WelcomeWindowWidth - 20, WelcomeWindowHeight));
        EditorGUILayout.LabelField("Welcome to the Ultimate Grids Engine - Version 2.1!\n"
            , LargeTextStyle);
        EditorGUILayout.Space();
        if (!PackageInstallation.IsInstalled(TilemapEditorPackageID)
            || !PackageInstallation.IsInstalled(PostProcessingPackageID))
        {
            EditorGUILayout.LabelField("IMPORTANT : DEPENDENCIES", LargeTextStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("The engine relies on a few <b>Unity packages</b> to run, and if you see this some of them are missing.\n"
                + "Click the <b>install buttons</b> below to complete installation.\n"
                + "You can learn more about this in the readme file.\n"
                + "Once all dependencies are installed, restart Unity and you're good to go!"
                    , RegularTextStyle);
            EditorGUILayout.Space();
            if (!PackageInstallation.IsInstalled(TilemapEditorPackageID))
            {
                EditorGUILayout.LabelField("Tilemap Editor is <b>not installed</b>", RegularTextStyle);
                if (GUILayout.Button(new GUIContent("  Install Tilemap Editor", EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image), GUILayout.MaxWidth(185f)))
                {
                    PackageInstallation.Install(TilemapEditorPackageVersionID);
                }
            }
            EditorGUILayout.Space();
            if (!PackageInstallation.IsInstalled(PostProcessingPackageID))
            {
                EditorGUILayout.LabelField("Post Processing is <b>not installed</b>", RegularTextStyle);
                if (GUILayout.Button(new GUIContent("  Install Post Processing", EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image), GUILayout.MaxWidth(185f)))
                {
                    PackageInstallation.Install(PostProcessingPackageVersionID);
                }
            }
            EditorGUILayout.Space();
            if (!PackageInstallation.IsInstalled(CinemachinePackageID))
            {
                EditorGUILayout.LabelField("Cinemachine is <b>not installed</b>", RegularTextStyle);
                if (GUILayout.Button(new GUIContent("  Install Cinemachine", EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image), GUILayout.MaxWidth(185f)))
                {
                    PackageInstallation.Install(CinemachinePackageVersionID);
                }
            }
            EditorGUILayout.Space();
            if (!PackageInstallation.IsInstalled(TextMeshProPackageID))
            {
                EditorGUILayout.LabelField("Textmesh Pro is <b>not installed</b>", RegularTextStyle);
                if (GUILayout.Button(new GUIContent("  Install Textmesh Pro", EditorGUIUtility.IconContent("BuildSettings.Standalone.Small").image), GUILayout.MaxWidth(185f)))
                {
                    PackageInstallation.Install(TextMeshProPackageVersionID);
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
        }
        EditorGUILayout.LabelField("GETTING STARTED", LargeTextStyle);
        EditorGUILayout.LabelField("You can start by having a look at the documentation and joining the discord server. \nHave fun with your project and rate the asset!", RegularTextStyle);
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(new GUIContent(" Open Documentation", EditorGUIUtility.IconContent("_Help").image), GUILayout.MaxWidth(185f), GUILayout.MaxHeight(20f)))
        {
            Application.OpenURL(DOCUMENTATION_URL);
        }
        if (GUILayout.Button(new GUIContent(" Join Discord       ", EditorGUIUtility.IconContent("d_console.infoicon.sml").image), GUILayout.MaxWidth(185f), GUILayout.MaxHeight(20f)))
        {
            Application.OpenURL(DOCUMENTATION_URL);
        }
        EditorGUILayout.EndHorizontal();
        GUILayout.EndArea();

        Rect areaRect = new Rect(0, WelcomeWindowHeight - 20, WelcomeWindowWidth, WelcomeWindowHeight - 20);
        GUILayout.BeginArea(areaRect);
        EditorGUILayout.LabelField("Copyright © 2020 GaboDevelops", FooterTextStyle);
        GUILayout.EndArea();
    }
    private void OnInspectorUpdate()
    {
        Repaint();
    }
}