using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// A class to handle triggering dialogue in a cutscene or during an interaction with an NPC.
    /// </summary>
    public class DialogueTrigger : MonoBehaviour
    {
        public DialogueTree dialogueTree;

        /// <summary>
        /// A method to call the DialogueManager to start the dialogueTree.
        /// </summary>
        public void TriggerDialogue()
        {
            if (DialogueManager.instance)
                DialogueManager.instance.StartDialogue(dialogueTree);
            else
                Debug.LogError("DialogueManager instance is not set! Please place DialogueManager in the scene.");
        }

        /// <summary>
        /// A method to overwrite the current dialogueTree with new dialogueTreeElements and dialogueTreeAudioClip lists.
        /// </summary>
        /// <param name="dialogueTreeElements">The list of strings for the new dialogueTree.</param>
        /// <param name="dialogueTreeAudioClips">The list of Audioclips for the new dialogueTree.</param>
        public void SetDialogueTreeContent(List<string> dialogueTreeElements, List<AudioClip> dialogueTreeAudioClips)
        {
            if (dialogueTreeElements == null)
                Debug.LogError("Argument dialogueTreeElements is null. If you wish to not have strings, then pass in a new List<string>() instead.");

            if (dialogueTreeAudioClips == null)
                Debug.LogError("Argument dialogueTreeAudioClips is null. If you wish to not have audioclips, then pass in a new List<AudioClip>() instead.");

            if (dialogueTreeElements == null || dialogueTreeAudioClips == null)
                return;

            dialogueTree.dialogueTreeElements = dialogueTreeElements;
            dialogueTree.dialogueTreeAudioClips = dialogueTreeAudioClips;
        }
    }
}