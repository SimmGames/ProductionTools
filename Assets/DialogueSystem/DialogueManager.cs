using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private DialogueContainer ActiveDialogue;
        private GeneratedDialogueCode dialogueCode;

        private void OnEnable()
        {
            dialogueCode = new GeneratedDialogueCode();
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        public static string GenerateFunctionName(string dialogueName, string nodeGuid, string portGuid = "") 
        {
            return $"{dialogueName.Replace(" ", "_").Trim()}_{nodeGuid.Replace("-", "")}{(string.IsNullOrEmpty(portGuid) ? string.Empty : ('_' + portGuid))}";
        }

        public void SetDialogue(DialogueContainer newDialogue) 
        {
            ActiveDialogue = newDialogue;
        }

        private List<NodeData> getNextNodes(string outputGuid) 
        {
            List<string> guids = new List<string>(); 
            ActiveDialogue.NodeLinks.FindAll(x => x.BaseNodeGuid == outputGuid).ForEach(x => guids.Add(x.TargetNodeGuid));

            List<NodeData> outputNodes = new List<NodeData>();
            guids.ForEach(guid => 
            {
                outputNodes.Add(ActiveDialogue.DialogueNodeData.Find(x => x.Guid == guid));
                outputNodes.Add(ActiveDialogue.ChatNodeData.Find(x => x.Guid == guid));
                ActiveDialogue.ConditionNodeData.FindAll(x => x.Guid == guid).ForEach(x => outputNodes.Add(x));
                ActiveDialogue.EventNodeData.FindAll(x => x.Guid == guid).ForEach(x => outputNodes.Add(x));
            });
            return outputNodes;
        }

        private NodeData getNextNode(NodeData startNode) 
        {
            return getNextNode(startNode.Guid);
        }
        private NodeData getNextNode(string outputGuid) 
        {
            
        }
        private void formDialogueChoices(DialogueNodeData dialogueNode) 
        {
            
        }

        private void runEventNode(EventNodeData eventNode) 
        {
            Dictionary<string, GeneratedDialogueCode.eventDelegate> events = dialogueCode.GetEventFunctions();
            GeneratedDialogueCode.eventDelegate eventFunction;
            events.TryGetValue(GenerateFunctionName(ActiveDialogue.DialogueName, eventNode.Guid), out eventFunction);
            eventFunction();
        }
    }
}