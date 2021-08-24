﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using DialogueSystem;

namespace DialogueSystem
{
    [Serializable]
    public class DialogueContainer : ScriptableObject
    {
        public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();
        public List<DialogueNodeData> DialogueNodeData = new List<DialogueNodeData>();
        public List<ConditionNodeData> ConditionNodeData = new List<ConditionNodeData>();
        public List<EventNodeData> EventNodeData = new List<EventNodeData>();
        public List<VariableNodeData> VariableNodeData = new List<VariableNodeData>();
    }
}