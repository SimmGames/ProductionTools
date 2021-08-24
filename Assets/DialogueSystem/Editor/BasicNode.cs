using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using DialogueSystem;

namespace DialogueSystem
{
    public class BasicNode : Node
    {
        public string GUID;
        public bool EntryPoint = false;
        public List<OutputPort> outputPorts;
        public nodeType Type;

        public BasicNode()
        {
            outputPorts = new List<OutputPort>();
        }
    }
}
