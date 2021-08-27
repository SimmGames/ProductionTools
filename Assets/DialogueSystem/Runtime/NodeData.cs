using System;
using UnityEngine;
using DialogueSystem;

namespace DialogueSystem
{
    [Serializable]
    public class NodeData
    {
        public string Guid;
        public Vector2 Position;
        public NodeType Type;
    }
}