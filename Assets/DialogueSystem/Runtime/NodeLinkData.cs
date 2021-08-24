using System;
using UnityEngine;
using DialogueSystem;

namespace DialogueSystem
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string NodeGUID;
        public string PortName;
        public string Condition;
        public string TargetNodeGuid;
    }
}