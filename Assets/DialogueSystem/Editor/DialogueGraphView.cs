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

    public DialogueGraphView() 
    {
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

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

    public void CreateNode(string nodeName) 
    {
        AddElement(CreateDialogueNode(nodeName));
    }

    public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "") 
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Multi);
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;

        var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        generatedPort.contentContainer.Remove(oldLabel);

        var choicePortName = string.IsNullOrEmpty(overriddenPortName) ? $"Choice {outputPortCount}" : overriddenPortName;
        var otherPort = ports.ToList().Where(x => x.portName == choicePortName && x.node == dialogueNode && x != generatedPort);
        if (otherPort.Any())
        {
            choicePortName = $"{choicePortName}[{otherPort.Count()}]";
        }

        generatedPort.portName = choicePortName;

        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => {
            var ogPort = ports.ToList().Where(x =>
                x.portName == evt.newValue && x.node == dialogueNode && x != generatedPort);
            if (ogPort.Any())
            {
                generatedPort.portName = $"{evt.newValue}[{ogPort.Count()}]";
                textField.SetValueWithoutNotify(generatedPort.portName);
            }
            else
            {
                generatedPort.portName = evt.newValue;
            }
        });

        generatedPort.contentContainer.Add(new Label("  "));
        generatedPort.contentContainer.Add(textField);
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);

        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
    }

    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
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

    public DialogueNode CreateDialogueNode(string nodeName) 
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            DialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);

        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        var button = new Button(()=> 
        {
            AddChoicePort(dialogueNode);
        });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);

        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt=> 
        {
            dialogueNode.DialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        textField.SetValueWithoutNotify(dialogueNode.title);

        dialogueNode.mainContainer.Add(textField);

        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, DefaltNodeSize));

        return dialogueNode;
    }


}
