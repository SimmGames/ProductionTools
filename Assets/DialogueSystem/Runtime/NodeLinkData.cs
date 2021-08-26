using System;
using UnityEngine;
using DialogueSystem;

namespace DialogueSystem
{
    [Serializable]
    public class NodeLinkData
    {
        public string BaseNodeGuid;
        public string PortName;
        public string PortGUID;
        public string Condition;
        public string TargetNodeGuid;
    }
}