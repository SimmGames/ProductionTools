using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using DialogueSystem.Code;
using System;

namespace DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField]
        private DialogueContainer ActiveDialogue = null; // Our dialogue tree to track
        private IDialogueCode dialogueCode = null; // The tree's dialogue code
        private NodeData currentNode;

        public string DialogueText => getDialogueText();
        public string Character => getCharacter();
        public string AudioFile => getAudioFile();
        public Dictionary<string, string> DialogueOptions => getDialogueOptions();


        // Start is called before the first frame update
        void Start()
        {
            currentNode = null;
            if (ActiveDialogue != null) 
            {
                Next(ActiveDialogue.EntryPointGUID);
                Type type = Type.GetType(DialogueCodeUtility.GenerateClassName(ActiveDialogue.name));
                if(type != null)
                    dialogueCode = (IDialogueCode)Activator.CreateInstance(type);
            }
        }

        public void Reset()
        {
            Start();
        }

        /// <summary>
        /// Generates the name of a function inside of the <see cref="GeneratedDialogueCode"/> class.
        /// <paramref name="portGuid"/> is optional but is needed for Conditions on <see cref="DialogueNodeData">Dialogue Nodes</see>.
        /// </summary>
        /// <param name="dialogueName"></param>
        /// <param name="nodeGuid"></param>
        /// <param name="portGuid"></param>
        /// <returns></returns>
        public static string GenerateFunctionName(string dialogueName, string nodeGuid, string portGuid = "") 
        {
            return DialogueCodeUtility.GenerateFunctionName(dialogueName, nodeGuid, portGuid);
        }

        /// <summary>
        /// Sets the working dialogue tree
        /// </summary>
        /// <param name="newDialogue"></param>
        public void SetDialogue(DialogueContainer newDialogue) 
        {
            ActiveDialogue = newDialogue;
            Start();
        }

        /// <summary>
        /// Gets ready for the next dialogue piece from the branch specified by the <paramref name="optionGUID"/>.
        /// </summary>
        /// <param name="optionGUID"></param>
        public void Next(string optionGUID) 
        {
            if (currentNode.Type == NodeType.Dialogue)
                currentNode = stepThroughNodes(optionGUID);
            else
                Debug.LogWarning("DialogueManager.Next(string guid) should only be used on a Dialogue Node!");
        }

        /// <summary>
        /// Gets ready for the next dialogue piece 
        /// </summary>
        public void Next() 
        {
            if (currentNode.Type == NodeType.Chat)
            {
                string portGuid = ActiveDialogue.NodeLinks.Find(x => x.BaseNodeGuid == currentNode.Guid).PortGUID;
                currentNode = stepThroughNodes(portGuid);
            }
            else
            {
                Debug.LogWarning("DialogueManager.Next should only be used on a Chat Node!");
            }
        }
        /// <summary>
        /// Use this method to grab any text field not predefined. Pass the text field's name in <paramref name="field"/>
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public string GetSomething(string field)
        {
            return GetTextField(currentNode, field);
        }

        /// <summary>
        /// Steps through the nodes and runs code. The currentNode is set after this.
        /// </summary>
        /// <param name="startOutputPortGUID"></param>
        private NodeData stepThroughNodes(string startOutputPortGUID) 
        {
            NodeData newNode = null;
            List<NodeData> nextNodes = getNextNodes(startOutputPortGUID);
            nextNodes.ForEach(x => 
            {
                switch (x.Type) 
                {
                    case NodeType.Branch:
                        newNode = runBranchCondition(x.Guid);
                        break;
                    case NodeType.Chat:
                        newNode = x;
                        break;
                    case NodeType.Dialogue:
                        newNode = x;
                        break;
                    case NodeType.Event:
                        runEventNode(x);
                        break;
                    case NodeType.Exit:
                        break;
                }
            });

            if (newNode == null)
            {
                // end condition
            }

            return newNode;
        }

        /// <summary>
        /// Check if a condition passed or not and runs the related nodes
        /// </summary>
        /// <param name="nodeGUID"></param>
        private NodeData runBranchCondition(string nodeGUID) 
        {
            Dictionary<string, IDialogueCode.ConditionDelegate> branchCondition = dialogueCode.ConditionChecks;
            IDialogueCode.ConditionDelegate conditionCheck;
            branchCondition.TryGetValue(GenerateFunctionName(ActiveDialogue.DialogueName, nodeGUID), out conditionCheck);
            if (conditionCheck())
            {
                // Pass
                return stepThroughNodes(ActiveDialogue.NodeLinks.Find(x => x.PortName == "Pass" && x.BaseNodeGuid == nodeGUID).PortGUID);
            }
            else 
            {
                // Fail
                return stepThroughNodes(ActiveDialogue.NodeLinks.Find(x => x.PortName == "Fail" && x.BaseNodeGuid == nodeGUID).PortGUID);
            }
        }

        /// <summary>
        /// Take a <see cref="NodeLinkData">Port's</see> GUID and returns a list of connected <see cref="NodeData">Nodes</see>.
        /// </summary>
        /// <param name="outputGuid"></param>
        /// <returns></returns>
        private List<NodeData> getNextNodes(string outputGuid) 
        {
            // Get the GUIDs of the targeted nodes
            List<string> guids = new List<string>(); 
            ActiveDialogue.NodeLinks.FindAll(x => x.PortGUID == outputGuid).ForEach(x => guids.Add(x.TargetNodeGuid));

            // Convert the GUIDs into NodeData objects
            List<NodeData> outputNodes = new List<NodeData>();
            guids.ForEach(guid => 
            {
                ActiveDialogue.Nodes.FindAll(x => x.Guid == guid).ForEach(x => outputNodes.Add(x));
            });
            return outputNodes;
        }

        /// <summary>
        /// Checks a <see cref="DialogueNodeData">Dialogue Node</see> and runs each condition for the choices.
        /// </summary>
        /// <param name="dialogueNode"></param>
        /// <returns>If the conditions pass, they are returned as Dictionary ( Option Text, Option Guid )</Option></returns>
        private Dictionary<string, string> formDialogueChoices(NodeData dialogueNode) 
        {
            // Find ports in node where the basenode guid's match. Make sure they're not in the list already
            Dictionary<string, string> dialogueOptions = new Dictionary<string, string>();
            ActiveDialogue.NodeLinks.FindAll(x => x.BaseNodeGuid == dialogueNode.Guid && !dialogueOptions.ContainsValue(x.PortGUID)).ForEach(x => 
            {
                dialogueOptions.Add(x.PortName, x.PortGUID);
            });

            // Check the conditions for the ports to run
            Dictionary<string, string> toReturnOptions = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> option in dialogueOptions)
            {
                if (checkCondition(dialogueNode.Guid, option.Value)) 
                {
                    toReturnOptions.Add(option.Key, option.Value);
                }
            }

            return toReturnOptions;
        }

        /// <summary>
        /// Runs the condition check in <see cref="GeneratedDialogueCode"/>.
        /// </summary>
        /// <param name="baseNodeGUID"></param>
        /// <param name="portGUID"></param>
        /// <returns></returns>
        private bool checkCondition(string baseNodeGUID, string portGUID) 
        {
            Dictionary<string, IDialogueCode.ConditionDelegate> portConditions = dialogueCode.DialogueChecks;
            IDialogueCode.ConditionDelegate conditionCheck;
            portConditions.TryGetValue(GenerateFunctionName(ActiveDialogue.DialogueName, baseNodeGUID, portGUID), out conditionCheck);
            return conditionCheck();
        }

        /// <summary>
        /// Takes an <see cref="EventNodeData"/> and runs the associated code in <see cref="GeneratedDialogueCode"/>
        /// </summary>
        /// <param name="eventNode"></param>
        private void runEventNode(NodeData eventNode) 
        {
            if (eventNode.Type != NodeType.Event) return;

            Dictionary<string, IDialogueCode.EventDelegate> events = dialogueCode.EventFunctions;
            IDialogueCode.EventDelegate eventFunction;
            events.TryGetValue(GenerateFunctionName(ActiveDialogue.DialogueName, eventNode.Guid), out eventFunction);
            eventFunction();
        }
        private Dictionary<string, string> getDialogueOptions()
        {
            if (currentNode.Type == NodeType.Dialogue)
            {
                return formDialogueChoices(currentNode);
            }
            else
            {
                return null;
            }
        }
        private string getCharacter()
        {
            if (currentNode.Type == NodeType.Chat || currentNode.Type == NodeType.Dialogue)
            {
                return GetTextField(currentNode, "CharacterName");
            }
            else
            {
                return null;
            }
        }
        private string getDialogueText()
        {
            if (currentNode.Type == NodeType.Chat || currentNode.Type == NodeType.Dialogue)
            {
                return GetTextField(currentNode, "DialogueText");
            }
            else 
            {
                return null;
            }
        }
        private string getAudioFile() 
        {
            if (currentNode.Type == NodeType.Chat || currentNode.Type == NodeType.Dialogue)
            {
                string file = GetTextField(currentNode, "Audio");
                return string.IsNullOrEmpty(file) ? null : file;
            }
            else
            {
                return null;
            }
        }

        private static string GetTextField(NodeData node, string field)
        {
            return DialogueCodeUtility.GetTextField(node, field);
        }
    }
}