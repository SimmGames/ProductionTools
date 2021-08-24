using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

public class DialogueNode : BasicNode
{
    public string DialogueText;
    public List<OutputPort> outputPorts;

    public DialogueNode() 
    {
        outputPorts = new List<OutputPort>();
    }
}
