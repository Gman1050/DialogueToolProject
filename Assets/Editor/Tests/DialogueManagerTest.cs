using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
//using UnityEngine.Assertions;

namespace Tests
{
    public class DialogueManagerTest
    {
        public GameObject dialogueCanvas = new GameObject();   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public GameObject dialogueVRCanvas = new GameObject();   // Get the BackgroundPanel gameobject from DialogueBoxCanvas

        public float printLetterDelay = 0.1f;
        public bool instantPrint = false;
        public bool printDialogue = true;

        public bool requireContinueButton = false;

        public float sentenceDelay = 1.0f;

        public bool useOpenCloseAnimation = false;

        public float volume = 1.0f;
        public bool playWithAudio = true;
        private AudioSource audioSource;

       
        public bool playAtStart = false;
        public DialogueTree dialogueTreeTest;

        // A Test behaves as an ordinary method
        [Test]
        public void DialogueManagerTestSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator DialogueManagerTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }

        [Test]
        public void StartDialogueTest()
        {
            // 1
            if (!printDialogue && !playWithAudio)
            {
                Debug.LogError("Cannot play dialogue! The printDialogue and playWithAudio booleans are false. Mark at least one of these as true in the inspector to start the dialogue.");
                LogAssert.Expect(LogType.Error, "Cannot play dialogue! The printDialogue and playWithAudio booleans are false. Mark at least one of these as true in the inspector to start the dialogue.");
                return;
            }


            Animator animator = dialogueCanvas.AddComponent<Animator>();
            RectTransform rt = dialogueCanvas.AddComponent<RectTransform>();

            Animator animatorVR = dialogueVRCanvas.AddComponent<Animator>();
            RectTransform rtVR = dialogueVRCanvas.AddComponent<RectTransform>();

            // 2
            if (printDialogue)
            {
                Assert.IsTrue(printDialogue);
                
                // 2a
                if (useOpenCloseAnimation)
                {
                    Assert.IsTrue(useOpenCloseAnimation);

                    animator.enabled = true;
                    animator.SetBool("canTransition", true);
                    animator.SetBool("isOpen", true);

                    Assert.IsTrue(animator.enabled);
                    Assert.IsTrue(animator.GetBool("canTransition"));
                    Assert.IsTrue(animator.GetBool("isOpen"));

                    animatorVR.enabled = true;
                    animatorVR.SetBool("canTransition", true);
                    animatorVR.SetBool("isOpen", true);

                    Assert.IsTrue(animatorVR.enabled);
                    Assert.IsTrue(animatorVR.GetBool("canTransition"));
                    Assert.IsTrue(animatorVR.GetBool("isOpen"));
                }

                // 2b
                else
                {
                    Assert.IsFalse(useOpenCloseAnimation);

                    //dialogueCanvas.SetActive(true);
                    animator.enabled = false;
                    rt.localScale = new Vector3(1, 1, 1);

                    Assert.IsFalse(animator.enabled);
                    UnityEngine.Assertions.Assert.AreEqual(rt.localScale, new Vector3(1, 1, 1));

                    animatorVR.enabled = false;
                    rtVR.localScale = new Vector3(1, 1, 1);

                    Assert.IsTrue(animatorVR.enabled);
                    UnityEngine.Assertions.Assert.AreEqual(rtVR.localScale, new Vector3(1, 1, 1));
                }
            }
        }

        [SetUp]
        public void Setup()
        {
            //StartDialogueTest(dialogueTreeTest);
        }

        [TearDown]
        public void Teardown()
        {

        }
    }
}
