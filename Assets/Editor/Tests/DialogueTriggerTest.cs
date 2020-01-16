using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests
{
    public class DialogueTriggerTest
    {
        public DialogueTree dialogueTree;

        [Test]
        public void TriggerDialogueTest()
        {
            if (DialogueManager.instance)
            {
                Assert.IsNotNull(DialogueManager.instance);
                DialogueManager.instance.StartDialogue(dialogueTree);
            }
            else
            {
                Assert.IsNull(DialogueManager.instance);
                Debug.LogError("DialogueManager instance is not set! Please place DialogueManager in the scene.");
                LogAssert.Expect(LogType.Error, "DialogueManager instance is not set! Please place DialogueManager in the scene.");
            }
        }
    }
}
