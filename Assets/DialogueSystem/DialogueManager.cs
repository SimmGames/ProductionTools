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

        // Start is called before the first frame update
        void Start()
        {

        }

        public void SetDialogue(DialogueContainer newDialogue) 
        {
            ActiveDialogue = newDialogue;
        }
    }
}