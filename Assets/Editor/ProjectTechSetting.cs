using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using TCUtil;

public class ProjectTechSettings : EditorWindow
{
    #region Property
    public bool UseDebugLog
    {
        get { return EditorPrefsEx.GetBool("UseDebugLog"); }
        set { EditorPrefsEx.SetBool("UseDebugLog", value); }
    }

    public bool ShowQuadTree
    {
        get { return EditorPrefsEx.GetBool("ShowQuadTree"); }
        set { EditorPrefsEx.SetBool("ShowQuadTree", value); }   
    }

    private GUIStyle _clientVersionStyle;
    #endregion

    [MenuItem("ProjectTech/Settings")]
    public static void ShowWindow()
    {
        ProjectTechSettings window = (ProjectTechSettings)EditorWindow.GetWindow(typeof(ProjectTechSettings));
        window.Init();
    }

    public void Init()
    {

    }

    private void OnGUI()
    {
        UseDebugLog = EditorGUILayout.Toggle("UseDebugEx", UseDebugLog);
        ShowQuadTree = EditorGUILayout.Toggle("ShowQuadTree ", ShowQuadTree);

        UpdateDefineOption();
    }

    private void UpdateDefineOption()
    {
        List<string> options = GetCurrentDefineOption();

        if (UseDebugLog)
        {
            options.Add("ENABLE_LOG");
        }
        else
        {
            if (options.Contains("ENABLE_LOG"))
                options.Remove("ENABLE_LOG");
        }
        if (ShowQuadTree)
        {
            options.Add("SHOW_BOUNDRY");
        }
        else
        {
            if (options.Contains("SHOW_BOUNDRY"))
                options.Remove("SHOW_BOUNDRY");
        }

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
