﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using DialogueSystem;
using DialogueSystem.Code;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

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
            foreach (DialogueContainer container in containers)
            {
                string setUp = $"{Tab(3)}// Setup //\n";
                string variables = $"{Tab(2)}// Variables //\n";
                string eventFunctions = $"{Tab(2)}// Event Functions //\n";
                string conditionChecks = $"{Tab(2)}// Condition Checks //\n";
                string dialogueChecks = $"{Tab(2)}// Dialogue Checks //\n";
                string treeName = SanitizeName(container.DialogueName);

                foreach (NodeData node in container.Nodes)
                {
                    string functionName = string.Empty;
                    switch (node.Type)
                    {
                        case NodeType.Variable:
                            variables += $"{Tab(2)}// Variable(s) From Node: {node.Guid} //\n" +
                                $"{node.TextFields["Code"]}\n";
                            break;

                        case NodeType.Event:
                            functionName = DialogueCodeUtility.GenerateFunctionName(container.DialogueName, node.Guid);

                            eventFunctions += $"{Tab(2)}// Event From Node: {node.Guid} //\n" +
                                $"{Tab(2)}public void {functionName}() {{\n" +
                                $"{node.TextFields["Code"]}\n" +
                                $"{Tab(2)}}}\n";

                            setUp += $"{Tab(3)}eventFunctions.Add(\"{functionName}\",{functionName});\n";
                            break;

                        case NodeType.Branch:
                            functionName = DialogueCodeUtility.GenerateFunctionName(container.DialogueName, node.Guid);

                            conditionChecks += $"{Tab(2)}// Condition From Node: {node.Guid} //\n" +
                                $"{Tab(2)}public bool {functionName}() {{\n" +
                                $"{Tab(3)}return ({node.TextFields["Condition"]});\n" +
                                $"{Tab(2)}}}\n";

                            setUp += $"conditionChecks.Add(\"{functionName}\",{functionName});\n";
                            break;

                        case NodeType.Dialogue:
                            List<string> ListedPorts = new List<string>();
                            foreach (NodeLinkData dialogueChoiceCondition in container.NodeLinks)
                            {
                                if (container.Nodes.Find(x => x.Guid == dialogueChoiceCondition.BaseNodeGuid) != null && ListedPorts.Find(y => y.Equals(dialogueChoiceCondition.PortGUID)) == null)
                                {
                                    functionName = DialogueCodeUtility.GenerateFunctionName(container.DialogueName, dialogueChoiceCondition.BaseNodeGuid, dialogueChoiceCondition.PortGUID);

                                    dialogueChecks += $"{Tab(2)}// From Node: {dialogueChoiceCondition.BaseNodeGuid} //\n{Tab(2)}// Choice: {dialogueChoiceCondition.PortName} - {dialogueChoiceCondition.PortGUID} //\n";
                                    dialogueChecks += $"{Tab(2)}public bool {functionName}()\n{Tab(2)}{{\n{Tab(3)}return (";
                                    dialogueChecks += (string.IsNullOrEmpty(dialogueChoiceCondition.Condition.Trim()) ? "true" : dialogueChoiceCondition.Condition);
                                    dialogueChecks += $");\n{Tab(2)}}}\n";

                                    setUp += $"dialogueChecks.Add(\"{functionName}\",{functionName});\n";
                                    ListedPorts.Add(dialogueChoiceCondition.PortGUID);
                                }
                            }
                            break;
                    }
                }
                CodeBuilder(setUp, variables, dialogueChecks, conditionChecks, eventFunctions, treeName);
            }
        }

        private static void CodeBuilder(string setUp, string variables, string dialogueChecks, string conditionNodesChecks, string eventNodeFunctions, string treeName)
        {
            string precode1 = @"using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;
using DialogueSystem;

namespace DialogueSystem.Code
{
    public class ";

            string precode2 = @" : IDialogueCode
    {
        private Dictionary<string, IDialogueCode.EventDelegate> eventFunctions = new Dictionary<string, IDialogueCode.EventDelegate>();
        private Dictionary<string, IDialogueCode.ConditionDelegate> conditionChecks = new Dictionary<string, IDialogueCode.ConditionDelegate>();
        private Dictionary<string, IDialogueCode.ConditionDelegate> dialogueChecks = new Dictionary<string, IDialogueCode.ConditionDelegate>();
        public Dictionary<string, IDialogueCode.EventDelegate> EventFunctions => eventFunctions;
        public Dictionary<string, IDialogueCode.ConditionDelegate> ConditionChecks => conditionChecks;
        public Dictionary<string, IDialogueCode.ConditionDelegate> DialogueChecks => dialogueChecks;
        public string GetVariable(string variableName) {
            return this.GetType().GetField(variableName).GetValue(this).ToString(); 
        }
";
            string postcode = @"    }
}";
            string toWrite = $"{precode1}{treeName}_GenCode{precode2}\n\n{variables}\n\n{Tab(2)}public void Start()\n{Tab(2)}{{\n{setUp}\n{Tab(2)}}}" +
                $"\n\n{dialogueChecks}\n\n{conditionNodesChecks}\n\n{eventNodeFunctions}\n{postcode}";


            WriteString(toWrite, $"{treeName}_GenCode");
        }

        private static string Tab(int amount = 1)
        {
            string back = string.Empty;
            for (int i = 0; i < amount; i++)
                back += "    ";
            return back;
        }

        private static string SanitizeName(string name)
        {
            return DialogueCodeUtility.SanitizeName(name);
        }
        private static void WriteString(string code, string file)
        {
            string path = $"Assets/DialogueSystem/Runtime/GeneratedCode/{file}.cs";

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