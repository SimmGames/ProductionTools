using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using DialogueSystem;
using System;

namespace DialogueSystem
{
    public class BasicNode : Node, IGraphNode
    {
        protected NodeData _nodeData;

        public string Guid { get { return _nodeData.Guid; } set { _nodeData.Guid = value; } }
        public NodeType Type { get { return _nodeData.Type; } set { _nodeData.Type = value; } }

        public bool EntryPoint { get; set; }
        public List<OutputPort> outputPorts { get; set; }
        

        public BasicNode()
        {
            _nodeData = new NodeData();
            EntryPoint = false;
            outputPorts = new List<OutputPort>();
        }

        public virtual BasicNode CreateNode(NodeData data, string guid)
        {
            throw new System.Exception("Can Not Make a \"Basic Node\" Type of Node");
        }

        public virtual NodeData SaveNodeData()
        {
            _nodeData.Position = this.GetPosition().position;
            return _nodeData;
        }
    }
}
