using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using DialogueSystem;

namespace DialogueSystem
{
    public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;

        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<BasicNode> Nodes => _targetGraphView.nodes.ToList().Cast<BasicNode>().ToList();
        private List<Port> Ports => _targetGraphView.ports.ToList();

        public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = targetGraphView
            };
        }

        public void SaveGraph(string fileName)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            dialogueContainer.DialogueName = fileName;

            if (Edges.Any())
            {
                var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
                for (var i = 0; i < connectedPorts.Length; i++)
                {
                    BasicNode outputNode = connectedPorts[i].output.node as BasicNode;
                    BasicNode inputNode = connectedPorts[i].input.node as BasicNode;
                    var lookingForGUID = connectedPorts[i].output.portName;


                    if (outputNode.Type == nodeType.Dialogue && !outputNode.EntryPoint)
                    {
                        var outputPort = ((DialogueNode)outputNode).outputPorts.Find(x => x.GUID == lookingForGUID);
                        dialogueContainer.NodeLinks.Add(new NodeLinkData
                        {
                            BaseNodeGuid = outputNode.GUID,
                            NodeGUID = connectedPorts[i].output.portName,
                            PortName = outputPort.Value,
                            Condition = outputPort.Condition,
                            TargetNodeGuid = inputNode.GUID
                        });
                    }
                    else
                    {
                        dialogueContainer.NodeLinks.Add(new NodeLinkData
                        {
                            BaseNodeGuid = outputNode.GUID,
                            PortName = connectedPorts[i].output.portName,
                            Condition = "",
                            TargetNodeGuid = inputNode.GUID,
                            NodeGUID = ""
                        });
                    }
                }
            }

            var unconnectedPorts = Ports.Where(x => x.connected == false && x.direction == Direction.Output).ToArray();
            for (var i = 0; i < unconnectedPorts.Length; i++)
            {
                BasicNode outputNode = unconnectedPorts[i].node as BasicNode;

                if (outputNode.Type == nodeType.Dialogue)
                {
                    var outputPort = ((DialogueNode)outputNode).outputPorts.Find(x => x.GUID == unconnectedPorts[i].portName);
                    dialogueContainer.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGuid = outputNode.GUID,
                        PortName = outputPort.Value,
                        Condition = outputPort.Condition,
                        TargetNodeGuid = "",
                        NodeGUID = unconnectedPorts[i].portName
                    });
                }
            }

            foreach (var node in Nodes.Where(node => !node.EntryPoint))
            {
                if (node.Type == nodeType.Dialogue)
                {
                    dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                    {
                        Guid = node.GUID,
                        DialogueText = ((DialogueNode)node).DialogueText,
                        Position = node.GetPosition().position
                    });
                }
                else if (node.Type == nodeType.Branch)
                {
                    dialogueContainer.ConditionNodeData.Add(new ConditionNodeData
                    {
                        Guid = node.GUID,
                        Condition = ((ConditionNode)node).Condition,
                        Position = node.GetPosition().position
                    });
                }
                else if (node.Type == nodeType.Event)
                {
                    dialogueContainer.EventNodeData.Add(new EventNodeData
                    {
                        Guid = node.GUID,
                        code = ((EventNode)node).Code,
                        Position = node.GetPosition().position
                    });
                }
                else if (node.Type == nodeType.Variable)
                {
                    dialogueContainer.VariableNodeData.Add(new VariableNodeData
                    {
                        Guid = node.GUID,
                        Code = ((VariableNode)node).Code,
                        Position = node.GetPosition().position
                    });
                }
            }

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!AssetDatabase.IsValidFolder("Assets/Resources/DialogueTrees"))
                AssetDatabase.CreateFolder("Resources", "DialogueTrees");

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/DialogueTrees/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        public void LoadGraph(string fileName)
        {
            _containerCache = Resources.Load<DialogueContainer>("DialogueTrees/" + fileName);

            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target dialogue graph file does not exist!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
        }

        private void ConnectNodes()
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].GUID && !string.IsNullOrEmpty(x.TargetNodeGuid)).ToList();
                for (var j = 0; j < connections.Count; j++)
                {
                    List<Port> outputPorts;
                    if (string.IsNullOrEmpty(connections[j].NodeGUID))
                        outputPorts = Ports.Where(x => x.portName == connections[j].PortName).ToList();
                    else
                        outputPorts = Ports.Where(x => x.portName == connections[j].NodeGUID).ToList();

                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);

                    Port basePort = outputPorts.Where(x => ((BasicNode)x.node).GUID == connections[j].BaseNodeGuid).First();
                    Port targetPort = (Port)targetNode.inputContainer[0];
                    LinkNodes(basePort, targetPort);

                }
            }
        }

        private void LinkNodes(Port output, Port input)
        {
            var tempEdge = new Edge
            {
                output = output,
                input = input
            };

            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);

            _targetGraphView.Add(tempEdge);
        }

        private void CreateNodes()
        {
            foreach (var nodeData in _containerCache.ConditionNodeData)
            {
                var tempNode = _targetGraphView.CreateConditionNode(nodeData.Condition, nodeData.Position, nodeData.Guid);

                _targetGraphView.AddElement(tempNode);
            }

            foreach (var nodeData in _containerCache.EventNodeData)
            {
                var tempNode = _targetGraphView.CreateEventNode(nodeData.code, nodeData.Position, nodeData.Guid);

                _targetGraphView.AddElement(tempNode);
            }

            foreach (var nodeData in _containerCache.VariableNodeData)
            {
                var tempNode = _targetGraphView.CreateVariableNode(nodeData.Code, nodeData.Position, nodeData.Guid);

                _targetGraphView.AddElement(tempNode);
            }

            foreach (var nodeData in _containerCache.DialogueNodeData)
            {
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, _containerCache.DialogueNodeData.First(x => x.Guid == nodeData.Guid).Position, nodeData.Guid);

                _targetGraphView.AddElement(tempNode);

                var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
                nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName, x.Condition, x.NodeGUID));
            }
        }

        private void ClearGraph()
        {
            var entryPoint = Nodes.Find(x => x.EntryPoint);
            entryPoint.GUID = _containerCache.NodeLinks[0].BaseNodeGuid;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint) continue;
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

                _targetGraphView.RemoveElement(node);
            }
        }
    }
}