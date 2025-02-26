using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class SaveSceneTool
{
    public static List<Action> OnGUIActions { get; } = new();
    public static List<Action> OnGUIButtonActions { get; } = new();

    static SaveSceneTool()
    {
        SceneView.duringSceneGui += OnGUI;
    }

    private static void OnGUI(SceneView sceneView)
    {
        int width = 250;
        int height = 200;
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect((Screen.width / EditorGUIUtility.pixelsPerPoint) - width, (Screen.height / EditorGUIUtility.pixelsPerPoint) - height, width, height));
        if (OnGUIActions.Count != 0)
        {
            GUILayout.BeginHorizontal("toolbar");
            foreach (var action in OnGUIActions)
            {
                action();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            foreach (var action in OnGUIButtonActions)
            {
                action();
            }
        }
        if (OnGUIActions.Count == 0)
        {
            GUILayout.FlexibleSpace();
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }
}
