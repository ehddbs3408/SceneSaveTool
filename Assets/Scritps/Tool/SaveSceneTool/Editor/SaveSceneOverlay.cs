using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;
[Overlay(typeof(SceneView), "SceneSaveTool", true)]
public class SaveSceneOverlay : Overlay
{
    VisualElement root;
    Label currenSceneLabel;
    TextField inputSaveFileName;
    string sceneName = "";
    
    public override VisualElement CreatePanelContent()
    {
        //Init Line
        root = new VisualElement() { name = "My Toolbar Root" };
        sceneName = EditorSceneManager.GetActiveScene().name;
        currenSceneLabel = new Label() { name = "SceneName", text = sceneName };
        inputSaveFileName = new TextField() { maxLength = 10 };
        inputSaveFileName.style.width = 80;
        //
        var inputElement = new VisualElement() {name = "input"};
        inputElement.style.flexDirection = FlexDirection.Row;
        inputElement.style.justifyContent = Justify.SpaceBetween;
        inputElement.Add(currenSceneLabel);
        inputElement.Add(new ToolbarButtom() {});
        inputElement.Add(inputSaveFileName);

        root.Add(inputElement);
        root.Add(SceneSaveBoxVisualElement("A"));
        root.Add(SceneSaveBoxVisualElement("B"));
        root.style.width = 150;
        return root;

    }

    public VisualElement SceneSaveBoxVisualElement(string fileName)
    {
        VisualElement vi = new VisualElement();
        vi.Add(new Label() {name = fileName,text = fileName });

        VisualElement buttomElement = new VisualElement();
        buttomElement.Add(new ToolbarButtom("Load","") { });
        buttomElement.Add(new ToolbarButtom("Delete","") { });
        buttomElement.style.flexDirection = FlexDirection.Row;
        vi.Add(buttomElement);

        vi.style.flexDirection = FlexDirection.Row;
        vi.style.alignItems = Align.Center;
        vi.style.justifyContent = Justify.SpaceBetween;
        

        return vi;
    }

    SaveSceneOverlay()
    {
        EditorApplication.update -= OnUpdate;
        EditorApplication.update += OnUpdate;
    }

    void OnUpdate()
    {
        Debug.Log("OnUpdate");
        if(sceneName == EditorSceneManager.GetActiveScene().name) return;

        sceneName = EditorSceneManager.GetActiveScene().name;
        root.Q<Label>("SceneName").text = sceneName;
    }


    [EditorToolbarElement(id, typeof(SceneView))]
    class ToolbarButtom : EditorToolbarButton
    {
        // This ID is used to populate toolbar elements.
        public const string id = "ExampleToolbar/Button";


        public ToolbarButtom()
        {
            text = "";
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCubeIcon.png");
            tooltip = "ex";
            clicked += OnClick;

            style.paddingLeft = 3;
            style.paddingRight = 3;
        }
        public ToolbarButtom(string text,string tooltip)
        {
            this.text = text;
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/CreateCubeIcon.png");
            this.tooltip = tooltip;
            clicked += OnClick;
        }

        void OnClick()
        {

        }
    }

}
