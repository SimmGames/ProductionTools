using System.Collections;
using System.Collections.Generic;

namespace DialogueSystem 
{
    public interface IGraphNode
    {
        public NodeData SaveNodeData();
        public BasicNode CreateNode(NodeData data, string guid);
        
    }
}
