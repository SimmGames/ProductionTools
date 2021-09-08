using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueSystem;
using System;

namespace DialogueSystem
{
    [Serializable]
    public class ChatNodeData : NodeData
    {
        public string DialogueText;
        public string CharacterName;
        public string Audio;
    }
}