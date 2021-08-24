using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueGraphView _targetGraphView;
    private DialogueContainer _containerCache;

    private List<Edge> Edges => _targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();
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
        if (!Edges.Any()) return;

        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();

        var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
        for (var i = 0; i < connectedPorts.Length; i++) 
        {
            DialogueNode outputNode = connectedPorts[i].output.node as DialogueNode;
            DialogueNode inputNode = connectedPorts[i].input.node as DialogueNode;
            var lookingForGUID = connectedPorts[i].output.portName;
            var outputPort = outputNode.outputPorts.Find(x => x.GUID == lookingForGUID);

            if (lookingForGUID == "Next") 
            {
                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = connectedPorts[i].output.portName,
                    Condition = "",
                    TargetNodeGuid = inputNode.GUID
                });
            }
            else
            {
                dialogueContainer.NodeLinks.Add(new NodeLinkData
                {
                    BaseNodeGuid = outputNode.GUID,
                    PortName = outputPort.Value,
                    Condition = outputPort.Condition,
                    TargetNodeGuid = inputNode.GUID
                });
            }
        }

        var unconnectedPorts = Ports.Where(x => x.connected == false && x.direction == Direction.Output).ToArray();
        for (var i = 0; i < unconnectedPorts.Length; i++)
        {
            var outputNode = unconnectedPorts[i].node as DialogueNode;
            var outputPort = outputNode.outputPorts.Find(x => x.GUID == unconnectedPorts[i].portName);

            dialogueContainer.NodeLinks.Add(new NodeLinkData
            {
                BaseNodeGuid = outputNode.GUID,
                PortName = outputPort.Value,
                Condition = outputPort.Condition,
                TargetNodeGuid = ""
            });
        }

        foreach (var dialogueNode in Nodes.Where(node=>!node.EntryPoint)) 
        {
            dialogueContainer.DialogueNodeData.Add(new DialogueNodeData 
            {
                Guid = dialogueNode.GUID,
                DialogueText = dialogueNode.DialogueText,
                Position = dialogueNode.GetPosition().position
            });
        }

        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");

        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        _containerCache = Resources.Load<DialogueContainer>(fileName);

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
                var targetNodeGuid = connections[j].TargetNodeGuid;
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                LinkNodes(Nodes[i].outputContainer[j].Q<Port>(), (Port)targetNode.inputContainer[0]);

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
        foreach (var nodeData in _containerCache.DialogueNodeData) 
        {
            var tempNode = _targetGraphView.CreateDialogueNode(nodeData.DialogueText, _containerCache.DialogueNodeData.First(x => x.Guid == nodeData.Guid).Position);
            tempNode.GUID = nodeData.Guid;

            _targetGraphView.AddElement(tempNode);

            var nodePorts = _containerCache.NodeLinks.Where(x => x.BaseNodeGuid == nodeData.Guid).ToList();
            nodePorts.ForEach(x => _targetGraphView.AddChoicePort(tempNode, x.PortName, x.Condition));
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
