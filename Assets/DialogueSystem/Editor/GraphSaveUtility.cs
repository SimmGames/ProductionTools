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
        
        /// <summary>
        /// Save the instance of Dialogue Graph View to a file with the name of <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveGraph(string fileName)
        {
            var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

            dialogueContainer.DialogueName = fileName;

            // Port Saving

            if (Edges.Any())
            {
                var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
                for (var i = 0; i < connectedPorts.Length; i++)
                {
                    BasicNode outputNode = connectedPorts[i].output.node as BasicNode;
                    BasicNode inputNode = connectedPorts[i].input.node as BasicNode;
                    var lookingForGUID = connectedPorts[i].output.portName;


                    if (outputNode.Type == NodeType.Dialogue && !outputNode.EntryPoint)
                    {
                        var outputPort = ((DialogueNode)outputNode).outputPorts.Find(x => x.GUID == lookingForGUID);
                        dialogueContainer.NodeLinks.Add(new NodeLinkData
                        {
                            BaseNodeGuid = outputNode.Guid,
                            PortGUID = connectedPorts[i].output.portName,
                            PortName = outputPort.Value,
                            Condition = outputPort.Condition,
                            TargetNodeGuid = inputNode.Guid
                        });
                    }
                    else
                    {
                        dialogueContainer.NodeLinks.Add(new NodeLinkData
                        {
                            BaseNodeGuid = outputNode.Guid,
                            PortName = connectedPorts[i].output.portName,
                            Condition = "",
                            TargetNodeGuid = inputNode.Guid,
                            PortGUID = ""
                        });
                    }
                }
            }

            var unconnectedPorts = Ports.Where(x => x.connected == false && x.direction == Direction.Output).ToArray();
            for (var i = 0; i < unconnectedPorts.Length; i++)
            {
                BasicNode outputNode = unconnectedPorts[i].node as BasicNode;

                if (outputNode.Type == NodeType.Dialogue)
                {
                    var outputPort = ((DialogueNode)outputNode).outputPorts.Find(x => x.GUID == unconnectedPorts[i].portName);
                    dialogueContainer.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGuid = outputNode.Guid,
                        PortName = outputPort.Value,
                        Condition = outputPort.Condition,
                        TargetNodeGuid = "",
                        PortGUID = unconnectedPorts[i].portName
                    });
                }
            }

            // Node Saving. All of this will be defined by the node itself

            dialogueContainer.EntryPointGUID = Nodes.Find(x => x.EntryPoint).Guid;

            foreach (var node in Nodes.Where(node => !node.EntryPoint))
            {
                if (node.Type == NodeType.Dialogue) // Dialogue Node
                {
                    dialogueContainer.DialogueNodeData.Add(new DialogueNodeData
                    {
                        Guid = node.Guid,
                        DialogueText = ((DialogueNode)node).DialogueText,
                        CharacterName = ((DialogueNode)node).CharacterName,
                        Position = node.GetPosition().position,
                        Type = NodeType.Dialogue,
                        Audio = ((DialogueNode)node).Audio
                    });
                }
                else if (node.Type == NodeType.Branch) // Branch Node
                {
                    dialogueContainer.ConditionNodeData.Add(new ConditionNodeData
                    {
                        Guid = node.Guid,
                        Condition = ((ConditionNode)node).Condition,
                        Position = node.GetPosition().position,
                        Type = NodeType.Branch
                    });
                }
                else if (node.Type == NodeType.Event) // Event Node
                {
                    dialogueContainer.EventNodeData.Add(new EventNodeData
                    {
                        Guid = node.Guid,
                        code = ((EventNode)node).Code,
                        Position = node.GetPosition().position,
                        Type = NodeType.Event
                    });
                }
                else if (node.Type == NodeType.Variable) // Variable Node
                {
                    dialogueContainer.VariableNodeData.Add(new VariableNodeData
                    {
                        Guid = node.Guid,
                        Code = ((VariableNode)node).Code,
                        Position = node.GetPosition().position,
                        Type = NodeType.Variable
                    });
                }
                else if (node.Type == NodeType.Chat) // Chat Node
                {
                    dialogueContainer.ChatNodeData.Add(new ChatNodeData 
                    {
                        Guid = node.Guid,
                        DialogueText = ((ChatNode)node).DialogueText,
                        CharacterName = ((ChatNode)node).CharacterName,
                        Position = node.GetPosition().position,
                        Type = NodeType.Chat,
                        Audio = ((ChatNode)node).Audio
                    });
                }
            }

            // Creating Asset (And asset folder)

            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");

            if (!AssetDatabase.IsValidFolder("Assets/Resources/DialogueTrees"))
                AssetDatabase.CreateFolder("Resources", "DialogueTrees");

            AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/DialogueTrees/{fileName}.asset");
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Load an instance of Dialogue Graph View from a file with the name of <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName"></param>
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
                var connections = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == Nodes[i].Guid && !string.IsNullOrEmpty(x.TargetNodeGuid)).ToList();
                for (var j = 0; j < connections.Count; j++)
                {
                    List<Port> outputPorts;
                    if (string.IsNullOrEmpty(connections[j].PortGUID))
                        outputPorts = Ports.Where(x => x.portName == connections[j].PortName).ToList();
                    else
                        outputPorts = Ports.Where(x => x.portName == connections[j].PortGUID).ToList();

                    var targetNodeGuid = connections[j].TargetNodeGuid;
                    var targetNode = Nodes.First(x => x.Guid == targetNodeGuid);

                    Port basePort = outputPorts.Where(x => ((BasicNode)x.node).Guid == connections[j].BaseNodeGuid).First();
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
            // Replace this with a "Load Node"
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
                var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, _containerCache.DialogueNodeData.First(x => x.Guid == nodeData.Guid).Position, nodeData.CharacterName, nodeData.Audio, nodeData.Guid);

                _targetGraphView.AddElement(tempNode);

                var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
                nodePorts.ForEach((x) => 
                {
                    if(tempNode.outputPorts.Find(y => y.GUID == x.PortGUID) == null)
                        _targetGraphView.AddChoicePort(tempNode, x.PortName, x.Condition, x.PortGUID);
                });
            }

            foreach (var nodeData in _containerCache.ChatNodeData)
            {
                _targetGraphView.CreateNode(nodeData);
            }
        }

        private void ClearGraph()
        {
            var entryPoint = Nodes.Find(x => x.EntryPoint);
            entryPoint.Guid = _containerCache.EntryPointGUID;

            foreach (var node in Nodes)
            {
                if (node.EntryPoint) continue;
                Edges.Where(x => x.input.node == node).ToList().ForEach(edge => _targetGraphView.RemoveElement(edge));

                _targetGraphView.RemoveElement(node);
            }
        }
    }
}