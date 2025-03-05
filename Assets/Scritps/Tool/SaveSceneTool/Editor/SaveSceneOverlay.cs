using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Collections.Generic;
using System.Linq;

[Overlay(typeof(SceneView), "SceneSaveTool", true)]
public class SaveSceneOverlay : Overlay
{
    VisualElement root;
    Label currentSceneLabel;
    TextField inputSaveFileName;
    string sceneName = "";

    // 경로 설정
    private string currentScenePath = "";
    private string saveFilePath = "";
    private DirectoryInfo directoryInfo;
    private Texture2D saveIcon;

    public override VisualElement CreatePanelContent()
    {
        //Init Line
        root = new VisualElement() { name = "My Toolbar Root" };
        sceneName = EditorSceneManager.GetActiveScene().name;
        currentSceneLabel = new Label() { name = "SceneName", text = sceneName };
        inputSaveFileName = new TextField() { maxLength = 20 };
        inputSaveFileName.style.width = 80;
        saveIcon = Resources.Load<Texture2D>("Tool/Icon/saveicon");

        // 경로 설정
        UpdatePaths();

        // 입력 요소 생성
        var inputElement = new VisualElement() { name = "input" };
        inputElement.style.flexDirection = FlexDirection.Row;
        inputElement.style.justifyContent = Justify.SpaceBetween;
        inputElement.style.marginBottom = 5;
        inputElement.style.paddingBottom = 5;
        inputElement.style.borderBottomWidth = 1;
        inputElement.style.borderBottomColor = Color.gray;

        inputElement.Add(currentSceneLabel);

        // 저장 버튼 추가
        var saveButton = new Button(() => { SaveSceneFile(); }) { text = "Save" };
        saveButton.style.width = 40;
        inputElement.Add(saveButton);
        inputElement.Add(inputSaveFileName);

        root.Add(inputElement);

        // 저장된 파일 목록 생성
        RefreshSavedFileList();

        root.style.width = 250;
        return root;
    }

    // 저장된 파일 목록 새로 고침
    private void RefreshSavedFileList()
    {
        // 기존 파일 목록 제거 (Save 영역 제외)
        var children = root.Children().ToList();
        for (int i = 1; i < children.Count; i++) // 첫 번째 요소(inputElement)는 건너뜀
        {
            root.Remove(children[i]);
        }

        // 디렉토리가 존재하는지 확인
        if (!Directory.Exists(saveFilePath))
        {
            try
            {
                Directory.CreateDirectory(saveFilePath);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"디렉토리 생성 실패: {e.Message}");
                return;
            }
        }

        directoryInfo = new DirectoryInfo(saveFilePath);

        if (directoryInfo.Exists)
        {
            bool hasFiles = false;

            foreach (var file in directoryInfo.GetFiles())
            {
                if (file.Name.EndsWith(".meta"))
                    continue;

                hasFiles = true;
                root.Add(CreateFileListItem(file));
            }

            if (!hasFiles)
            {
                var noFilesLabel = new Label("저장된 씬 파일이 없습니다.");
                noFilesLabel.style.paddingTop = 10;
                noFilesLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
                root.Add(noFilesLabel);
            }
        }
    }

    // 파일 항목 UI 생성
    private VisualElement CreateFileListItem(FileInfo file)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.justifyContent = Justify.SpaceBetween;
        container.style.alignItems = Align.Center;
        container.style.paddingTop = 3;
        container.style.paddingBottom = 3;

        var fileName = Path.GetFileNameWithoutExtension(file.Name);
        var fileLabel = new Label(fileName);
        fileLabel.style.flexGrow = 1;
        container.Add(fileLabel);

        var buttonsContainer = new VisualElement();
        buttonsContainer.style.flexDirection = FlexDirection.Row;

        var loadButton = new Button(() => { LoadSceneFile(file); }) { text = "Load" };
        loadButton.style.width = 40;
        loadButton.style.marginRight = 5;

        var deleteButton = new Button(() => { DeleteSceneFile(file); }) { text = "Delete" };
        deleteButton.style.width = 40;

        buttonsContainer.Add(loadButton);
        buttonsContainer.Add(deleteButton);
        container.Add(buttonsContainer);

        return container;
    }

    // 경로 업데이트
    private void UpdatePaths()
    {
        sceneName = EditorSceneManager.GetActiveScene().name;
        currentScenePath = $"{Application.dataPath}/Scenes/{sceneName}";
        saveFilePath = $"{Application.dataPath}/SceneSave/SceneSaveFile/{sceneName}";

    }

    // 씬 파일 저장
    private void SaveSceneFile()
    {
        string fileName = inputSaveFileName.value;

        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("경고", "저장 파일 이름을 입력해주세요.", "확인");
            return;
        }

        try
        {
            if (!Directory.Exists(saveFilePath))
            {
                Directory.CreateDirectory(saveFilePath);
            }

            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            File.Copy($"{currentScenePath}.unity", $"{saveFilePath}/{fileName}.unity", overwrite: true);
            inputSaveFileName.value = "";

            AssetDatabase.Refresh();
            EditorApplication.delayCall += () => RefreshSavedFileList();

            Debug.Log($"씬이 성공적으로 저장되었습니다: {fileName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"씬 저장 실패: {e.Message}");
            EditorUtility.DisplayDialog("오류", $"씬 저장 중 오류가 발생했습니다: {e.Message}", "확인");
        }
    }

    // 씬 파일 로드
    private void LoadSceneFile(FileInfo file)
    {
        try
        {
            // 대상 디렉토리 경로 추출
            string targetDirectory = Path.GetDirectoryName($"{currentScenePath}.unity");

            // 대상 디렉토리가 존재하지 않으면 생성
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            File.Copy($"{saveFilePath}/{file.Name}", $"{currentScenePath}.unity", overwrite: true);
            EditorSceneManager.OpenScene($"{currentScenePath}.unity");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 로드 실패: {e.Message}");
            EditorUtility.DisplayDialog("오류", $"파일 로드 중 오류가 발생했습니다: {e.Message}", "확인");
        }
    }

    // 씬 파일 삭제
    private void DeleteSceneFile(FileInfo file)
    {
        try
        {
            string filePath = $"{saveFilePath}/{file.Name}";
            File.Delete(filePath);
            AssetDatabase.Refresh();
            EditorApplication.delayCall += () => RefreshSavedFileList();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"파일 삭제 실패: {e.Message}");
            EditorUtility.DisplayDialog("오류", $"파일 삭제 중 오류가 발생했습니다: {e.Message}", "확인");
        }
    }

    public SaveSceneOverlay()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    void OnUpdate()
    {
        if (root == null || sceneName == EditorSceneManager.GetActiveScene().name) return;

        // 씬 변경 시 경로 업데이트 및 UI 새로고침
        UpdatePaths();
        sceneName = EditorSceneManager.GetActiveScene().name;
        currentSceneLabel.text = sceneName;
        RefreshSavedFileList();
    }
}
