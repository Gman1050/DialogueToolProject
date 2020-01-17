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

        private List<string> dialogueTreeElementsTemp;
        private List<AudioClip> dialogueTreeAudioClipsTemp;

        DialogueManagerTest dialogueManagerTestInstance;

        [SetUp]
        public void Setup()
        {
            dialogueManagerTestInstance = new DialogueManagerTest();
            dialogueManagerTestInstance.Setup();
            dialogueTree = AssetDatabase.LoadAssetAtPath<DialogueTree>("Assets/DialogueToolPackage/DialogueTreeAssets/AI_Response.asset");
            Assert.IsNotNull(dialogueManagerTestInstance);
            Assert.IsNotNull(dialogueTree);

            dialogueTreeElementsTemp = dialogueTree.dialogueTreeElements;
            dialogueTreeAudioClipsTemp = dialogueTree.dialogueTreeAudioClips;
            Assert.AreEqual(dialogueTree.dialogueTreeElements, dialogueTreeElementsTemp);
            Assert.AreEqual(dialogueTree.dialogueTreeAudioClips, dialogueTreeAudioClipsTemp);
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
            List<string> dialogueTreeElements = new List<string>();
            List<AudioClip> dialogueTreeAudioClips = new List<AudioClip>();

            if (dialogueTreeElements == null)
            {
                Assert.IsNull(dialogueTreeElements);
                Debug.LogError("Argument dialogueTreeElements is null. If you wish to not have strings, then pass in a new List<string>() instead.");
                LogAssert.Expect(LogType.Error, "Argument dialogueTreeElements is null. If you wish to not have strings, then pass in a new List<string>() instead.");
            }

            if (dialogueTreeAudioClips == null)
            {
                Assert.IsNull(dialogueTreeAudioClips);
                Debug.LogError("Argument dialogueTreeAudioClips is null. If you wish to not have audioclips, then pass in a new List<AudioClip>() instead.");
                LogAssert.Expect(LogType.Error, "Argument dialogueTreeAudioClips is null. If you wish to not have audioclips, then pass in a new List<AudioClip>() instead.");
            }

            if (dialogueTreeElements == null || dialogueTreeAudioClips == null)
            {
                Assert.IsTrue(dialogueTreeElements == null || dialogueTreeAudioClips == null);
                return;
            }

            Assert.IsNotNull(dialogueTreeElements);
            Assert.IsNotNull(dialogueTreeAudioClips);

            dialogueTree.dialogueTreeElements = dialogueTreeElements;
            dialogueTree.dialogueTreeAudioClips = dialogueTreeAudioClips;

            Assert.AreEqual(dialogueTreeElements, dialogueTree.dialogueTreeElements);
            Assert.AreEqual(dialogueTreeAudioClips, dialogueTree.dialogueTreeAudioClips);
        }

        [TearDown]
        public void Teardown()
        {
            dialogueManagerTestInstance.Teardown();
            dialogueTree.dialogueTreeElements = dialogueTreeElementsTemp;
            dialogueTree.dialogueTreeAudioClips = dialogueTreeAudioClipsTemp;

            Assert.AreEqual(dialogueTreeElementsTemp, dialogueTree.dialogueTreeElements);
            Assert.AreEqual(dialogueTreeAudioClipsTemp, dialogueTree.dialogueTreeAudioClips);

            dialogueTree = null;
            dialogueTreeElementsTemp = null;
            dialogueTreeAudioClipsTemp = null;

            Assert.IsNull(dialogueTree);
            Assert.IsNull(dialogueTreeElementsTemp);
            Assert.IsNull(dialogueTreeAudioClipsTemp);
        }
    }
}
