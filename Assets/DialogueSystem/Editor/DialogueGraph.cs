using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{

    private DialogueGraphView _graphView;
    private Toolbar _toolbar;

    [MenuItem("Window/Dialogue System/Dialogue Graph", false, 3010)]
    public static void OpenDialogueGraphWindow() 
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraph();
        GenerateToolbar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);
        rootVisualElement.Remove(_toolbar);
    }

    private void ConstructGraph() 
    {
        _graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);
    }
    private void GenerateToolbar() 
    {
        _toolbar = new Toolbar();
        var nodeCreateButton = new Button(() => {
            _graphView.CreateNode("Dialogue Node");
        });
        nodeCreateButton.text = "Create Node";
        _toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(_toolbar);
    }
}
