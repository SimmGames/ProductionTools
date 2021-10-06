using System;
using UnityEngine;
using DialogueSystem;
using System.Collections.Generic;

namespace DialogueSystem
{
    [Serializable]
    public class NodeData
    {
        public string Guid;
        public Vector2 Position;
        public NodeType Type;
        public Dictionary<string, string> TextFields;
    }
}