using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnitySceneSaveExtender;
using System.IO;

[InitializeOnLoad]
public static class SaveSceneExtender
{
    static Texture svicon;

    static readonly string SAVE_FILE_PATH = "SceneSaveFile";

    static string currentSceneName = "";
    static string selectSaveFileName = "";

    static string currentScenePath = "";
    static string saveFilePath = "";
    static DirectoryInfo directoryInfo;

    static string name = "";

    static SaveSceneExtender()
    {
        OnUpdate();
        svicon = Resources.Load("Tool/Icon/saveicon") as Texture;
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    static void OnUpdate()
    {
        if (currentSceneName == EditorSceneManager.GetActiveScene().name) return;

        //Set Path
        currentSceneName = EditorSceneManager.GetActiveScene().name;
        currentScenePath = $@"{Application.dataPath}/Scenes/{currentSceneName}";
        saveFilePath = $@"{Application.dataPath}/{SAVE_FILE_PATH}/{currentSceneName}";

        //Set SaveFileDirectoryInfo
        directoryInfo = new DirectoryInfo(saveFilePath);

        SaveSceneTool.OnGUIActions.Clear();
        SaveSceneTool.OnGUIActions.Add(() =>
        {
            GUILayout.Label(currentSceneName, GUILayout.MaxWidth(100));
            GUILayout.FlexibleSpace();

            if (GUILayout.Button(svicon, GUILayout.MaxWidth(20), GUILayout.MaxHeight(18)))
            {
                //Set Select SceneFile
                selectSaveFileName = name;
   
                name = "";
                SaveSceneFile();
            }

            name = GUILayout.TextField(name, GUILayout.MinWidth(120));
        });

        SaveSceneTool.OnGUIButtonActions.Clear();
        SaveSceneTool.OnGUIButtonActions.Add(() =>
        {
            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Name.Contains("meta") == false)
                {
                    GUILayout.BeginHorizontal("toolbar");
                    GUILayout.Label(file.Name);
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Load"))
                    {
                        File.Copy($"{saveFilePath}/{file.Name}", $"{currentScenePath}.unity", overwrite: true);
                        EditorSceneManager.OpenScene($"{currentScenePath}.unity");
                    }
                    if (GUILayout.Button("Delete"))
                    {
                        File.Delete($"{saveFilePath}/{file.Name}");
                        GUILayout.EndHorizontal();
                        break;

                    }
                    GUILayout.EndHorizontal();
                }
            }
        });
    }

    static void SaveSceneFile()
    {
        if (Directory.Exists(saveFilePath) == false)
        {
            Directory.CreateDirectory(saveFilePath);
        }

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        File.Copy($"{currentScenePath}.unity", $"{saveFilePath}/{selectSaveFileName}.unity", overwrite: true);
    }
}
