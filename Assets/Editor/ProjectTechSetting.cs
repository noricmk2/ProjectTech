using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TCUtil;

public class ProjectUFSettings : EditorWindow
{
    #region Property
    public bool LoadFromResources
    {
        get { return EditorPrefsEx.GetBool("LoadFromResources"); }
        set { EditorPrefsEx.SetBool("LoadFromResources", value); }
    }
    public bool UseLocalSave
    {
        get { return EditorPrefsEx.GetBool("UseLocalSave"); }
        set { EditorPrefsEx.SetBool("UseLocalSave", value); }
    }
    public bool UseDebugLog
    {
        get { return EditorPrefsEx.GetBool("UseDebugLog"); }
        set { EditorPrefsEx.SetBool("UseDebugLog", value); }
    }

    private GUIStyle _clientVersionStyle;
    #endregion

    [MenuItem("ProjectTech/Settings")]
    public static void ShowWindow()
    {
        ProjectUFSettings window = (ProjectUFSettings)EditorWindow.GetWindow(typeof(ProjectUFSettings));
        window.Init();
    }

    public void Init()
    {

    }

    private void OnGUI()
    {
        LoadFromResources = EditorGUILayout.Toggle("Asset LoadFromResources: ", LoadFromResources);

        UpdateDefineOption();
    }

    private void UpdateDefineOption()
    {
        List<string> options = GetCurrentDefineOption();

        if (LoadFromResources)
        {
            options.Add("LOAD_FROM_RESOURCES");
        }
        if (UseLocalSave)
        {
            options.Add("LOCAL_SAVE");
        }
        if (UseDebugLog)
        {
            options.Add("ENABLE_LOG");
        }

        options.Add("MADORCA");

        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        options = options.Distinct().ToList();
        string allSymbols = string.Join(";", options.ToArray());
        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, allSymbols);
    }

    public static List<string> GetCurrentDefineOption()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        List<string> options = symbols.Split(DelimiterHelper.Semicolon).ToList();
        return options;
    }
}
