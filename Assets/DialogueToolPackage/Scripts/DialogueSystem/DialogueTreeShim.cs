using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DialogueSystem
{
    /// <summary>
    /// A static class that serves as a shim to pass DialogueTree data from NonMonobehavior objects to the DialogueManager.
    /// </summary>
    public static class DialogueTreeShim
    {
        public static DialogueTree tempDialogueTree = (DialogueTree)ScriptableObject.CreateInstance(typeof(DialogueTree));       // Set this to the DialogueTree scriptable object that will only have a single reusable updatable node.
        private static string characterName = "";

        /// <summary>
        /// A method call to set and display the dialogue node content into the tempDialogueTree.
        /// </summary>
        /// <param name="dialogueTree">The dialogue tree that you are passing in from a NonMonobehavior object.</param>
        /// <param name="changeCharacterName">Used to overwrite the existing characterName string on a DialogueTree scriptable object.</param>
        public static void SetAndDisplayDialogueNodeContent(DialogueTree dialogueTree, bool changeCharacterName = false)
        {
            Debug.LogWarning("dialogueTree: " + dialogueTree);
            Debug.LogWarning("tempDialogueTree: " + tempDialogueTree);

            tempDialogueTree = (DialogueTree)ScriptableObject.CreateInstance(typeof(DialogueTree));       // Set this to the DialogueTree scriptable object that will only have a single reusable updatable node.

            // Add new dialogue node if dialogueNodeElements list count is 0
            if (tempDialogueTree.dialogueNodeElements.Count == 0)
                tempDialogueTree.dialogueNodeElements.Add(new DialogueTree.DialogueNode());

            // Create new dialogue node for tempDialogueTree to contain.
            DialogueTree.DialogueNode dialogueNode = new DialogueTree.DialogueNode();

            // Get reference to the characterName from the DialogueTree scriptable object that will be modified.
            if (characterName == "")
                characterName = tempDialogueTree.dialogueNodeElements[0].nodeCharacterName;

            // First clear existing data that was previous used in the tempDialogueTree if any
            tempDialogueTree.dialogueNodeElements.Clear();

            for (int i = 0; i < dialogueTree.dialogueNodeElements.Count; i++)
            {
                // Set the new dialogueNode's dialogue string and audio clip to dialogueTree contents that is being passed in.
                dialogueNode.nodeDialogueString = dialogueTree.dialogueNodeElements[i].nodeDialogueString;
                dialogueNode.nodeDialogueAudioClip = dialogueTree.dialogueNodeElements[i].nodeDialogueAudioClip;

                // Check to see if the characterName needs to be changed. (By default, it should be false.)
                if (!changeCharacterName)
                    dialogueNode.nodeCharacterName = characterName;
                else
                    dialogueNode.nodeCharacterName = dialogueTree.dialogueNodeElements[i].nodeCharacterName;

                // Then add the dialogueNode so that there is always one node in the dialogueNodeElements list.
                tempDialogueTree.dialogueNodeElements.Add(dialogueNode);
            }

            Debug.LogWarning("tempDialogueTree: " + tempDialogueTree);

            // Call the DialogueManager to start the tempDialogueTree
            DialogueManager.instance.StartDialogue(tempDialogueTree);
        }
    }
}