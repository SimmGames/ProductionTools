using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using DialogueSystem;

namespace DialogueSystem
{
    public static class DialogueCoder
    {
        // Code to make code
        public static List<DialogueContainer> GrabDialogueContainers() 
        {
            DialogueContainer[] containers = Resources.LoadAll<DialogueContainer>("DialogueTrees");
            List<DialogueContainer> listOfContainers = new List<DialogueContainer>();
            foreach (DialogueContainer c in containers)
                listOfContainers.Add(c);
            return listOfContainers;
        }

        public static void GenerateCode(List<DialogueContainer> containers) 
        {
            string setUp = "";
            string variables = "";
            string eventFunctions = "";
            string conditionChecks = "";
            string dialogueChecks = "";
            foreach (DialogueContainer container in containers) 
            {
                variables += $"\n\n// Container: {container.DialogueName} //\n\n";
                foreach (VariableNodeData variableNodes in container.VariableNodeData) 
                {
                    variables += $"\n// Node: {variableNodes.Guid} //\n{variableNodes.Code}\n";
                }

                eventFunctions = $"\n\n// Container: {container.DialogueName} //\n\n";
                foreach (EventNodeData eventNodes in container.EventNodeData) 
                {
                    var functionName = $"{container.DialogueName.Replace(" ", "_").Trim()}_{eventNodes.Guid.Replace("-", "")}";

                    eventFunctions += $"\n// Node: {eventNodes.Guid} //\n";
                    eventFunctions += $"public void {functionName}() {{\n";
                    eventFunctions += eventNodes.code;
                    eventFunctions += $"\n}}\n\n";

                    setUp += $"eventFunctions.Add(\"{functionName}\",{functionName});\n";
                }

                conditionChecks = $"\n\n// Container: {container.DialogueName} //\n\n";
                foreach (ConditionNodeData conditions in container.ConditionNodeData) 
                {
                    var functionName = $"{container.DialogueName.Replace(" ", "_").Trim()}_{conditions.Guid.Replace("-", "")}";

                    conditionChecks += $"\n// Node: {conditions.Guid} //\n";
                    conditionChecks += $"public bool {functionName}() {{\nreturn (";
                    conditionChecks += conditions.Condition;
                    conditionChecks += $");\n}}\n\n";

                    setUp += $"conditionChecks.Add(\"{functionName}\",{functionName});\n";
                }

                dialogueChecks = $"\n\n// Container: {container.DialogueName} //\n\n";
                foreach (NodeLinkData dialogueChoiceCondition in container.NodeLinks)
                {
                    if (container.DialogueNodeData.Find(x => x.Guid == dialogueChoiceCondition.BaseNodeGuid) != null) 
                    {
                        var functionName = $"{container.DialogueName.Replace(" ", "_").Trim()}_{dialogueChoiceCondition.BaseNodeGuid.Replace("-", "")}_{dialogueChoiceCondition.PortGUID.Replace("-", "")}";

                        dialogueChecks += $"\n// Node: {dialogueChoiceCondition.BaseNodeGuid} //\n// Choice: {dialogueChoiceCondition.PortGUID} //\n";
                        dialogueChecks += $"public bool {functionName}() {{\nreturn (";
                        dialogueChecks += (string.IsNullOrEmpty(dialogueChoiceCondition.Condition.Trim()) ? "true" : dialogueChoiceCondition.Condition);
                        dialogueChecks += $");\n}}\n\n";

                        setUp += $"dialogueChecks.Add(\"{functionName}\",{functionName});\n";
                    }
                }
            }

            CodeBuilder(setUp, variables, dialogueChecks, conditionChecks, eventFunctions);
        }

        private static void CodeBuilder(string setUp, string variables, string dialogueChecks, string conditionNodesChecks, string eventNodeFunctions) 
        {
            string usingStuff = $"using System.Collections;\nusing System.Collections.Generic;\nusing UnityEngine;\nusing System;\nusing UnityEditor;\nusing DialogueSystem;\n\n";
            string interfaceStuff = $"public delegate void eventDelegate();\n" +
                $"public delegate bool conditionDelegate();\n" +
                $"private Dictionary<string, eventDelegate> eventFunctions = new Dictionary<string, eventDelegate>();\n" +
                $"private Dictionary<string, conditionDelegate> conditionChecks = new Dictionary<string, conditionDelegate>();\n" +
                $"private Dictionary<string, conditionDelegate> dialogueChecks = new Dictionary<string, conditionDelegate>();\n" +
                $"public Dictionary<string, eventDelegate> GetEventFunctions() {{ return eventFunctions; }}\n" +
                $"public Dictionary<string, conditionDelegate> GetConditionChecks() {{ return conditionChecks; }}\n" +
                $"public Dictionary<string, conditionDelegate> GetDialogueChecks() {{ return dialogueChecks; }}\n" +
                $"public string GetVariable(string variableName) {{ return this.GetType().GetField(variableName).GetValue(this).ToString(); }}\n";

            WriteString($"{usingStuff}namespace DialogueSystem {{\npublic class GeneratedDialogueCode{{\n" +
                $"{interfaceStuff}\n\n{variables}\n\npublic void Start() {{\n{setUp}\n}}\n\n{dialogueChecks}\n\n{conditionNodesChecks}\n\n{eventNodeFunctions}\n\n" +
                $"\n}}\n}}");
        }

        
        private static void WriteString(string code)
        {
            string path = "Assets/DialogueSystem/Runtime/GeneratedCode/GeneratedDialogueCode.cs";

            if (!AssetDatabase.IsValidFolder("Runtime/GeneratedCode"))
                AssetDatabase.CreateFolder("Runtime", "GeneratedCode");

            // Write the code to a CS file
            StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine(code);
            writer.Close();

            // Tell unity that there's a new file present
            AssetDatabase.Refresh();
        }
    }

}