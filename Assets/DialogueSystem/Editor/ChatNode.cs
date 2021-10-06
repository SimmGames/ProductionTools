using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;

namespace DialogueSystem
{
    public class ChatNode : BasicNode, IGraphNode
    {
        private ChatNodeData NodeData 
        {   
            get { return (ChatNodeData)_nodeData; } 
            set { _nodeData = (NodeData)value; } 
        }

        public string DialogueText { get; set; }
        public string CharacterName { get; set; }
        public string Audio { get; set; }

        public override BasicNode CreateNode(NodeData data, string guid)
        {
            
        }

        public override NodeData SaveNodeData()
        {
            NodeData
        }
    }
}