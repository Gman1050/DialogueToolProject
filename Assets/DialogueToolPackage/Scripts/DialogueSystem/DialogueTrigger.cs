using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogueSystem
{
    /// <summary>
    /// A class to handle triggering dialogue in a cutscene or during an interaction with an NPC.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        [Header("The DialogueTree that will be played:")]
        public DialogueTree dialogueTree;

        [Header("Start and End Events of a DialogueTree:")]
        public UnityEvent onStartDialogue;
        public UnityEvent onEndDialogue;

        [Header("Dialogue Test Settings:")]
        public bool playAtStart = false;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            if (playAtStart)
                StartCoroutine(DelayTriggerDialogue(0.1f));
        }

        /// <summary>
        /// A method to call the DialogueManager to start the dialogueTree.
        /// </summary>
        public void TriggerDialogue()
        {
            if (DialogueManager.instance)
                DialogueManager.instance.StartDialogue(dialogueTree, transform);
            else
                Debug.LogError("DialogueManager instance is not set! Please place DialogueManager in the scene.");
        }

        /// <summary>
        /// A method to overwrite the current dialogueTree with new dialogueTreeElements and dialogueTreeAudioClip lists.
        /// </summary>
        /// <param name="dialogueTreeElements">The list of strings for the new dialogueTree.</param>
        /// <param name="dialogueTreeAudioClips">The list of Audioclips for the new dialogueTree.</param>
        public void SetDialogueTreeContent(List<DialogueTree.DialogueNode> dialogueNodeElements)
        {
            if (dialogueNodeElements == null)
            {
                Debug.LogError("Argument dialogueTreeElements is null. If you wish to not have strings, then pass in a new List<string>() instead.");
                return;
            }

            dialogueTree.dialogueNodeElements = dialogueNodeElements;
        }

        IEnumerator DelayTriggerDialogue(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            TriggerDialogue();
        }
    }
}