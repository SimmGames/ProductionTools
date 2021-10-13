using DialogueSystem;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace DialogueSystem
{
    [Serializable]
    public class NodeData
    {
        private List<KeyValuePair<string, string>> _textFields = new List<KeyValuePair<string, string>>();
        public string Guid;
        public Vector2 Position;
        public NodeType Type;
        
        [XmlIgnore]
        public Dictionary<string, string> TextFields = new Dictionary<string, string>();
    }
}