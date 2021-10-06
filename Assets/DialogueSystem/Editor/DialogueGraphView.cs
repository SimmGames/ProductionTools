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
        public delegate void RemovePortDelegate(DialogueNode dialogueNode, Port generatedPort);

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
                Guid = ensureGuid(),
                EntryPoint = true,
                Type = NodeType.Entry
            };

            node.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

            var generatedPort = GeneratePort(node, Direction.Output, Port.Capacity.Multi);
            generatedPort.portName = "Next";
            node.outputContainer.Add(generatedPort);

            node.capabilities &= ~Capabilities.Movable;
            node.capabilities &= ~Capabilities.Deletable;

            // GUID Label
            node.extensionContainer.Add(new Label($"{node.Guid}") { name = "guid" });

            node.RefreshExpandedState();
            node.RefreshPorts();

            node.SetPosition(new Rect(Vector2.zero, DefaltNodeSize));
            return node;
        }

        public BasicNode CreateNode(NodeData data) 
        {
            BasicNode temp = null;
            switch (data.Type)
            {
                case NodeType.Dialogue:
                    RemovePortDelegate Remove = RemovePort;
                    temp = DialogueNode.CreateNode(data, ensureGuid(data.Guid), Remove);
                    break;
                case NodeType.Branch:
                    temp = ConditionNode.CreateNode(data, ensureGuid(data.Guid));
                    break;
                case NodeType.Event:
                    temp = EventNode.CreateNode(data, ensureGuid(data.Guid));
                    break;
                case NodeType.Variable:
                    temp = VariableNode.CreateNode(data, ensureGuid(data.Guid));
                    break;
                case NodeType.Chat:
                    temp = ChatNode.CreateNode(data, ensureGuid(data.Guid));
                    break;
            }
            if(temp != null)
                AddElement(temp);
            return temp;
    }

        public void CreateNode(string nodeName, NodeType type, Vector2 location)
        {
            switch (type)
            {
                case NodeType.Dialogue:
                    RemovePortDelegate Remove = RemovePort;
                    AddElement(DialogueNode.CreateNode(location, nodeName, ensureGuid(), Remove));
                    break;
                case NodeType.Branch:
                    AddElement(ConditionNode.CreateNode(location, nodeName, ensureGuid()));
                    break;
                case NodeType.Event:
                    AddElement(EventNode.CreateNode(location, nodeName, ensureGuid()));
                    break;
                case NodeType.Variable:
                    AddElement(VariableNode.CreateNode(location, nodeName, ensureGuid()));
                    break;
                case NodeType.Chat:
                    AddElement(ChatNode.CreateNode(location, nodeName, ensureGuid()));
                    break;
            }
        }

        private string ensureGuid(string overrideGUID)
        {
            return (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID);
        }
        private string ensureGuid()
        {
            string tempGuid = Guid.NewGuid().ToString();
            while (Nodes.Where(x => x.Guid == tempGuid).Count() > 0)
            {
                tempGuid = Guid.NewGuid().ToString();
            }
            return tempGuid;
        }


        public EventNode CreateEventNode(string code, Vector2 location, string overrideGUID = "")
        {
            var eventNode = new EventNode
            {
                title = "Event",
                Code = code,
                Guid = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = NodeType.Event
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
            eventNode.extensionContainer.Add(new Label($"{eventNode.Guid}") { name = "guid" });

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
                Guid = (string.IsNullOrEmpty(overrideGUID) ? ensureGuid() : overrideGUID),
                Type = NodeType.Variable
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
            varNode.extensionContainer.Add(new Label($"{varNode.Guid}") { name = "guid" });

            // Update Graphics and Position

            varNode.RefreshExpandedState();
            varNode.RefreshPorts();
            varNode.SetPosition(new Rect(location, DefaltNodeSize));

            return varNode;
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
}