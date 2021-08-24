using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{

    private DialogueGraphView _graphView;
    private Toolbar _toolbar;
    private MiniMap _miniMap;
    private string _fileName = "New Dialogue";

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
        GenerateMiniMap();
    }

    private void GenerateMiniMap()
    {
        _miniMap = new MiniMap { anchored = true };
        _miniMap.SetPosition(new Rect(10, 30, 200, 140));

        _graphView.Add(_miniMap);
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

        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(_fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => {
            _fileName = evt.newValue;
        });

        _toolbar.Add(fileNameTextField);

        _toolbar.Add(new Button(() => RequestDataOperation(true)) { text = "Save Data" });
        _toolbar.Add(new Button(() => RequestDataOperation(false)) { text = "Load Data" });

        var nodeCreateButton = new Button(() => {
            _graphView.CreateNode("Dialogue Node");
        });
        nodeCreateButton.text = "Create Node";
        _toolbar.Add(nodeCreateButton);

        rootVisualElement.Add(_toolbar);
    }

    private void RequestDataOperation(bool save) 
    {
        if (string.IsNullOrEmpty(_fileName)) 
        {
            EditorUtility.DisplayDialog("Invalid File Name!", "File name can not be blank!", "OK");
            return;
        }

        var saveUtility = GraphSaveUtility.GetInstance(_graphView);
        if (save)
            saveUtility.SaveGraph(_fileName);
        else
            saveUtility.LoadGraph(_fileName);
;    }
}
