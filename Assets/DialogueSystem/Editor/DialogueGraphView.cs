using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System.Linq;
using DialogueSystem;

namespace DialogueSystem
{
    public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaltNodeSize = new Vector2(150, 200);
        public Vector2 localMousePosition;

        private List<BasicNode> Nodes => nodes.ToList().Cast<BasicNode>().ToList();

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

        private Port GeneratePort(BasicNode node, Direction portDirection, Port.Capacity capacity)
        {
            return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();

            ports.ForEach((port) =>
            {
                if (startPort != port && startPort.node != port.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private BasicNode GenerateEntryPointNode()
        {
            var node = new BasicNode
            {
                title = "START",
                GUID = ensureGuid(),
                EntryPoint = true,
                Type = nodeType.Entry
            };

            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            var generatedPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            // GUID Label
            node.extensionContainer.Add(new Label($"{node.GUID}") { name = "guid" });

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
                case nodeType.Branch:
                    AddElement(CreateConditionNode(nodeName, location));
                    break;
                case nodeType.Event:
                    AddElement(CreateEventNode(nodeName, location));
                    break;
                case nodeType.Variable:
                    AddElement(CreateVariableNode(nodeName, location));
                    break;
                case nodeType.Chat:
                    AddElement(CreateChatNode(nodeName, location));
                    break;
            }
        }

        public string ensureGuid()
        {
            string tempGuid = Guid.NewGuid().ToString();
            while (Nodes.Where(x => x.GUID == tempGuid).Count() > 0)
            {
                tempGuid = Guid.NewGuid().ToString();
            }
            return tempGuid;
        }

        public ConditionNode CreateConditionNode(string condition, Vector2 location, string overrideGUID = "")
        {
            var conditionNode = new ConditionNode
            {
                title = "Condition",
                Condition = condition,
                GUID = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = nodeType.Branch
            };



            conditionNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Condition Info
            var conditionContainer = new VisualElement
            {
                name = "bottom"
            };

            var conditionLabel = new Label("Condition: ");
            conditionContainer.Add(conditionLabel);

            var conditionTextField = new TextField(string.Empty) { name = "script" };
            conditionTextField.multiline = true;
            conditionTextField.RegisterValueChangedCallback(evt =>
            {
                conditionNode.Condition = evt.newValue;
            });
            conditionTextField.SetValueWithoutNotify(conditionNode.Condition);
            conditionContainer.Add(conditionTextField);

            conditionNode.mainContainer.Add(conditionContainer);

            // Input
            var inputPort = GeneratePort(conditionNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            conditionNode.inputContainer.Add(inputPort);

            // Output

            var passPort = GeneratePort(conditionNode, Direction.Output, Port.Capacity.Multi);
            passPort.portName = "Pass";
            conditionNode.outputContainer.Add(passPort);

            var failPort = GeneratePort(conditionNode, Direction.Output, Port.Capacity.Multi);
            failPort.portName = "Fail";
            conditionNode.outputContainer.Add(failPort);

            // GUID Label
            conditionNode.extensionContainer.Add(new Label($"{conditionNode.GUID}") { name = "guid" });

            // Update Graphics and Position

            conditionNode.RefreshExpandedState();
            conditionNode.RefreshPorts();
            conditionNode.SetPosition(new Rect(location, DefaltNodeSize));

            return conditionNode;
        }

        public EventNode CreateEventNode(string code, Vector2 location, string overrideGUID = "")
        {
            var eventNode = new EventNode
            {
                title = "Event",
                Code = code,
                GUID = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = nodeType.Event
            };
            eventNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Event Info
            var eventContainer = new VisualElement
            {
                name = "bottom"
            };

            var eventLabel = new Label("Code: ");
            eventContainer.Add(eventLabel);

            var eventTextField = new TextField(string.Empty) { name = "script" };
            eventTextField.multiline = true;
            eventTextField.RegisterValueChangedCallback(evt =>
            {
                eventNode.Code = evt.newValue;
            });
            eventTextField.SetValueWithoutNotify(eventNode.Code);
            eventContainer.Add(eventTextField);

            eventNode.mainContainer.Add(eventContainer);

            // Input
            var inputPort = GeneratePort(eventNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            eventNode.inputContainer.Add(inputPort);

            // GUID Label
            eventNode.extensionContainer.Add(new Label($"{eventNode.GUID}") { name = "guid" });

            // Update Graphics and Position

            eventNode.RefreshExpandedState();
            eventNode.RefreshPorts();
            eventNode.SetPosition(new Rect(location, DefaltNodeSize));

            return eventNode;
        }

        public VariableNode CreateVariableNode(string code, Vector2 location, string overrideGUID = "")
        {
            var varNode = new VariableNode
            {
                title = "Variables",
                Code = code,
                GUID = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = nodeType.Variable
            };
            varNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Event Info
            var varContainer = new VisualElement
            {
                name = "bottom"
            };

            var varTextField = new TextField(string.Empty) { name = "script" };
            varTextField.multiline = true;
            varTextField.RegisterValueChangedCallback(evt =>
            {
                varNode.Code = evt.newValue;
            });
            varTextField.SetValueWithoutNotify(varNode.Code);
            varContainer.Add(varTextField);

            varNode.mainContainer.Add(varContainer);

            // GUID Label
            varNode.extensionContainer.Add(new Label($"{varNode.GUID}") { name = "guid" });

            // Update Graphics and Position

            varNode.RefreshExpandedState();
            varNode.RefreshPorts();
            varNode.SetPosition(new Rect(location, DefaltNodeSize));

            return varNode;
        }

        private string limit(string str, int length)
        {
            return (str.Length <= length ? str : $"{str.Substring(0, length - 3)}...");
        }

        public DialogueNode CreateDialogueNode(string nodeName, Vector2 position, string charcaterName = "", string overrideGUID = "")
        {
            var dialogueNode = new DialogueNode
            {
                title = $"Dialogue: {limit(nodeName, 20)}",
                DialogueText = nodeName,
                CharacterName = charcaterName,
                GUID = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = nodeType.Dialogue
            };
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Dialogue Text Info

            var dialogueContainer = new VisualElement
            {
                name = "bottom"
            };

            dialogueContainer.Add(new Label("Character Name:"));

            var nameTextField = new TextField(string.Empty);
            nameTextField.multiline = true;
            nameTextField.RegisterValueChangedCallback(evt =>
            {
                dialogueNode.CharacterName = evt.newValue;
            });
            nameTextField.SetValueWithoutNotify(dialogueNode.CharacterName);
            dialogueContainer.Add(nameTextField);


            var dialogueLabel = new Label("Dialogue Text:");
            dialogueContainer.Add(dialogueLabel);

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

            // GUID Label
            dialogueNode.extensionContainer.Add(new Label($"{dialogueNode.GUID}") { name = "guid" });

            // Update Graphics and Position

            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(position, DefaltNodeSize));

            return dialogueNode;
        }

        public ChatNode CreateChatNode(string nodeName, Vector2 position, string charcaterName = "", string overrideGUID = "")
        {
            var dialogueNode = new DialogueNode
            {
                title = $"Chat: {limit(nodeName, 20)}",
                DialogueText = nodeName,
                CharacterName = charcaterName,
                GUID = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = nodeType.Chat
            };
            dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            // Dialogue Text Info

            var dialogueContainer = new VisualElement
            {
                name = "bottom"
            };

            dialogueContainer.Add(new Label("Character Name:"));

            var nameTextField = new TextField(string.Empty);
            nameTextField.multiline = true;
            nameTextField.RegisterValueChangedCallback(evt =>
            {
                dialogueNode.CharacterName = evt.newValue;
            });
            nameTextField.SetValueWithoutNotify(dialogueNode.CharacterName);
            dialogueContainer.Add(nameTextField);


            var dialogueLabel = new Label("Dialogue Text:");
            dialogueContainer.Add(dialogueLabel);

            var dialogueTextField = new TextField(string.Empty);
            dialogueTextField.multiline = true;
            dialogueTextField.RegisterValueChangedCallback(evt =>
            {
                dialogueNode.DialogueText = evt.newValue;
                dialogueNode.title = $"Chat: {limit(nodeName, 20)}";
            });
            dialogueTextField.SetValueWithoutNotify(dialogueNode.DialogueText);
            dialogueContainer.Add(dialogueTextField);

            dialogueNode.mainContainer.Add(dialogueContainer);

            // Input Ports

            var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            dialogueNode.inputContainer.Add(inputPort);

            // Output Ports

            var outputPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Multi);
            outputPort.portName = "Next";
            dialogueNode.outputContainer.Add(outputPort);

            // GUID Label
            dialogueNode.extensionContainer.Add(new Label($"{dialogueNode.GUID}") { name = "guid" });

            // Update Graphics and Position

            dialogueNode.RefreshExpandedState();
            dialogueNode.RefreshPorts();
            dialogueNode.SetPosition(new Rect(position, DefaltNodeSize));

            return dialogueNode;
        }

        public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "", string conditions = "", string overriddenGUID = "")
        {
            var generatedPort = GeneratePort(dialogueNode, Direction.Output, Port.Capacity.Multi);
            var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
            var outputPort = new OutputPort
            {
                NodeGUID = dialogueNode.GUID,
                GUID = (string.IsNullOrEmpty(overriddenGUID) ? Guid.NewGuid().ToString() : overriddenGUID),
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
            textField.RegisterValueChangedCallback(evt =>
            {
                outputPort.Value = evt.newValue;
            });

            var conditionField = new TextField
            {
                name = "script",
                value = conditions,
                multiline = true
            };
            conditionField.RegisterValueChangedCallback(evt =>
            {
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
        Variable,
        Chat,
        Entry,
        Exit
    }
}