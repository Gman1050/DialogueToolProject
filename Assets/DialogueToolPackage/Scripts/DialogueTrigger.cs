using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueTree dialogueTree;
    
    public void TriggerDialogue()
    {
        if (DialogueManager.instance)
            DialogueManager.instance.StartDialogue(dialogueTree);
        else
            Debug.LogError("DialogueManager instance is not set! Please place DialogueManager in the scene.");
    }
}
