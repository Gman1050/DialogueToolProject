using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using DialogueSystem;

namespace Tests
{
    public class DialogueTriggerTest
    {
        public DialogueTree dialogueTree;

        private List<DialogueTree.DialogueNode> dialogueNodeElementsTemp;

        DialogueManagerTest dialogueManagerTestInstance;

        [SetUp]
        public void Setup()
        {
            dialogueManagerTestInstance = new DialogueManagerTest();
            dialogueManagerTestInstance.Setup();
            dialogueTree = AssetDatabase.LoadAssetAtPath<DialogueTree>("Assets/DialogueToolPackage/DialogueTreeAssets/AI_Response.asset");
            Assert.IsNotNull(dialogueManagerTestInstance);
            Assert.IsNotNull(dialogueTree);

            dialogueNodeElementsTemp = dialogueTree.dialogueNodeElements;
            Assert.AreEqual(dialogueTree.dialogueNodeElements, dialogueNodeElementsTemp);
        }

        [Test, Order(1)]
        public void TriggerDialogueTest()
        {
            if (dialogueManagerTestInstance != null)
            {
                Assert.IsTrue(dialogueManagerTestInstance != null);
                dialogueManagerTestInstance.StartDialogueTest();
            }
            else
            {
                Assert.IsFalse(dialogueManagerTestInstance != null);
                Debug.LogError("DialogueManager instance is not set! Please place DialogueManager in the scene.");
                LogAssert.Expect(LogType.Error, "DialogueManager instance is not set! Please place DialogueManager in the scene.");
            }
        }

        [Test, Order(2)]
        public void SetDialogueTreeContentTest()
        {
            List<DialogueTree.DialogueNode> dialogueNodeElements = new List<DialogueTree.DialogueNode>();

            if (dialogueNodeElements == null)
            {
                Assert.IsNull(dialogueNodeElements);
                Debug.LogError("Argument dialogueNodeElements is null. If you wish to not have strings, then pass in a new List<DialogueTree.DialogueNode>() instead.");
                LogAssert.Expect(LogType.Error, "Argument dialogueNodeElements is null. If you wish to not have strings, then pass in a new List<DialogueTree.DialogueNode>() instead.");
                return;
            }

            Assert.IsNotNull(dialogueNodeElements);

            dialogueTree.dialogueNodeElements = dialogueNodeElements;

            Assert.AreEqual(dialogueNodeElements, dialogueTree.dialogueNodeElements);
        }

        [TearDown]
        public void Teardown()
        {
            dialogueManagerTestInstance.Teardown();
            dialogueTree.dialogueNodeElements = dialogueNodeElementsTemp;

            Assert.AreEqual(dialogueNodeElementsTemp, dialogueTree.dialogueNodeElements);

            dialogueTree = null;
            dialogueNodeElementsTemp = null;

            Assert.IsNull(dialogueTree);
            Assert.IsNull(dialogueNodeElementsTemp);
        }
    }
}
