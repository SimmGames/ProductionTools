using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 DefaltNodeSize = new Vector2(150, 200);
    public Vector2 localMousePosition;

    public DialogueGraphView() 
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        RegisterCallback<MouseDownEvent>(evt => { localMousePosition = (evt.localMousePosition - new Vector2(viewTransform.position.x, viewTransform.position.y)) / scale; });

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();

        AddElement(GenerateEntryPointNode());
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity) 
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach((port)=> 
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    private DialogueNode GenerateEntryPointNode() 
    {
        var node = new DialogueNode
        {
            title = "START",
            GUID = Guid.NewGuid().ToString(),
            DialogueText = "ENTRYPOINT",
            EntryPoint = true
        };

        var generatedPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);

        node.capabilities &= ~Capabilities.Movable;
        node.capabilities &= ~Capabilities.Deletable;

        node.RefreshExpandedState();
        node.RefreshPorts();

        node.SetPosition(new Rect(Vector2.zero, DefaltNodeSize));
        return node;
    }

    public void CreateNode(string nodeName, nodeType type, Vector2 location) 
    {
        switch (type) 
        {
            case nodeType.Dialogue:
                AddElement(CreateDialogueNode(nodeName, location));
                break;
        }
    }

    private string limit(string str, int length) 
    {
        return (str.Length <= length ? str : $"{str.Substring(0,length-3)}...");
    }

    public DialogueNode CreateDialogueNode(string nodeName, Vector2 position)
    {
        var dialogueNode = new DialogueNode
        {
            title = $"Dialogue: {limit(nodeName, 20)}",
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };
        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        // Dialogue Text Info

        var dialogueContainer = new VisualElement
        {
            name = "dialogue"
        };

        var dialogueLable = new Label("Dialogue Text:");
        dialogueContainer.Add(dialogueLable);

        var dialogueTextField = new TextField(string.Empty);
        dialogueTextField.multiline = true;
        dialogueTextField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = $"Dialogue: {limit(nodeName, 20)}";
        });
        dialogueTextField.SetValueWithoutNotify(dialogueNode.DialogueText);
        dialogueContainer.Add(dialogueTextField);

        dialogueNode.mainContainer.Add(dialogueContainer);

        // Input Ports

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        // Output Ports

        var button = new Button(() =>
        {
            AddChoicePort(dialogueNode, "");
        });
        button.text = "+";
        dialogueNode.titleContainer.Add(button);

        

        // Script Container
        var scriptContainer = new VisualElement
        {
            name = "script"
        };

        dialogueNode.extensionContainer.Add(scriptContainer);

        // Update Graphics and Position

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position, DefaltNodeSize));
        
        return dialogueNode;
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "", string conditions = "") 
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Multi);
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        var outputPort = new OutputPort
        {
            NodeGUID = dialogueNode.GUID,
            GUID = GUID.Generate().ToString(),
            Condition = conditions,
            Value = ""
        };
        generatedPort.portName = outputPort.GUID;

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount}" : overriddenPortName;

        outputPort.Value = choicePortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName,
            isDelayed = true,
            multiline = true
        };
        textField.RegisterValueChangedCallback(evt => {
            outputPort.Value = evt.newValue;
        });

        var conditionField = new TextField
        {
            value = conditions,
            multiline = true
        };
        conditionField.name = "condition";
        conditionField.RegisterValueChangedCallback(evt => {
            outputPort.Condition = evt.newValue;
        });

        var mainContainer = new VisualElement
        {
            name = "mainContainer"
        };

        
        generatedPort.contentContainer.Add(new Label("  "));
        mainContainer.Add(textField);
        mainContainer.Add(new Label("Condition:"));
        mainContainer.Add(conditionField);
        generatedPort.contentContainer.Add(mainContainer);

        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "-"
        };
        generatedPort.contentContainer.Add(deleteButton);

        dialogueNode.outputPorts.Add(outputPort);

        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        dialogueNode.outputPorts.Remove(dialogueNode.outputPorts.Find(x => x.GUID == generatedPort.portName));

        var targetPort = ports.ToList().Where(x =>
            x.portName == generatedPort.portName && x.node == generatedPort.node);
        if (!targetPort.Any()) return;

        var targetEdge = edges.ToList().Where(x =>
            x.output.portName == generatedPort.portName && x.output.node == generatedPort.node);
        if (targetEdge.Any())
        {
            var edge = targetEdge.First();
            edge.input.Disconnect(edge);
            RemoveElement(targetEdge.First());
        }

        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

}

public enum nodeType 
{
    Dialogue,
    Branch,
    Event,
    Variable
}