using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
//using UnityEngine.Assertions;

namespace Tests
{
    public class DialogueManagerTest
    {
        // Dialogue Canvas Elements
        public GameObject dialogueCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas

        // Dialogue VR Canvas Elements
        public GameObject dialogueVRCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas

        // Dialogue Print Settings
        public float printLetterDelay = 0.1f;
        public bool instantPrint = false;
        public bool printDialogue = true;

        // Dialogue Input Settings
        public bool requireContinueButton = false;

        // Dialogue Delay Settings
        public float sentenceDelay = 1.0f;

        // Dialogue Animation Settings
        public bool useOpenCloseAnimation = false;

        // Dialogue Audio Settings
        public float volume = 1.0f;
        public bool playWithAudio = true;
        private AudioSource audioSource;

        // Dialogue Test Settings
        public bool playAtStart = false;
        public DialogueTree dialogueTreeTest;

        // Debug Settings
        public bool debugComponent = false;

        // Dialogue Queues
        private Queue<string> sentences;
        private Queue<AudioClip> sentenceAudioClips;

        [SetUp]
        public void Setup()
        {
            dialogueCanvas = new GameObject();
            dialogueVRCanvas = new GameObject();
            dialogueTreeTest = (DialogueTree)ScriptableObject.CreateInstance("DialogueTree");

            sentences = new Queue<string>();
            sentenceAudioClips = new Queue<AudioClip>();
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
                    Assert.AreEqual(rt.localScale, new Vector3(1, 1, 1));

                    animatorVR.enabled = false;
                    rtVR.localScale = new Vector3(1, 1, 1);

                    Assert.IsFalse(animatorVR.enabled);
                    Assert.AreEqual(rtVR.localScale, new Vector3(1, 1, 1));
                }
            }

            // 3
            if (debugComponent)
            {
                Assert.IsTrue(debugComponent);
                Debug.Log("Start conversation with " + dialogueTreeTest.characterName);
                LogAssert.Expect(LogType.Error, "Start conversation with " + dialogueTreeTest.characterName);
            }

            // 4
            GameObject nameTextObject = new GameObject();
            Assert.IsNotNull(nameTextObject);

            Text nameText = nameTextObject.AddComponent<Text>();
            Assert.IsNotNull(nameText);
            Assert.IsEmpty(nameText.text);
            Assert.IsEmpty(dialogueTreeTest.characterName);

            nameText.text = dialogueTreeTest.characterName;
            Assert.AreEqual(nameText.text, dialogueTreeTest.characterName);

            // 5
            GameObject nameVRTextObject = new GameObject();
            Text nameVRText = nameVRTextObject.AddComponent<Text>();
            Assert.IsNotNull(nameVRTextObject);
            Assert.IsNotNull(nameVRText);

            nameVRText.text = dialogueTreeTest.characterName;
            Assert.AreEqual(nameVRText.text, dialogueTreeTest.characterName);

            // 6
            sentences.Clear();
            Assert.AreEqual(sentences.Count, 0);

            // 7
            foreach (string sentence in dialogueTreeTest.dialogueTreeElements)
            {
                sentences.Enqueue(sentence);
            }
            Assert.AreEqual(sentences.Count, dialogueTreeTest.dialogueTreeElements.Count);

            // 8
            foreach (AudioClip clip in dialogueTreeTest.dialogueTreeAudioClips)
            {
                sentenceAudioClips.Enqueue(clip);
            }
            Assert.AreEqual(sentences.Count, dialogueTreeTest.dialogueTreeAudioClips.Count);

            // 9
            //DisplayNextSentence();
        }

        [Test]
        public void DisplayNextSentenceTest()
        {
            // 1
            if (sentences.Count == 0)
            {
                Assert.AreEqual(sentences.Count, 0);
                //EndDialogue();
                return;
            }

            // 2
            string sentence = sentences.Peek();
            Assert.AreEqual(sentence, sentences.Peek());

            int previousSentenceCount = sentences.Count;
            sentences.Dequeue();
            Assert.AreEqual(sentences.Count, previousSentenceCount - 1);

            AudioClip clip = sentenceAudioClips.Peek();
            Assert.AreEqual(clip, sentenceAudioClips.Peek());

            int previousSentenceAudioClipsCount = sentenceAudioClips.Count;
            sentenceAudioClips.Dequeue();
            Assert.AreEqual(sentenceAudioClips.Count, previousSentenceCount - 1);

            // 3
            if (debugComponent)
            {
                Assert.IsTrue(debugComponent);
                Debug.Log(sentence);
                LogAssert.Expect(LogType.Error, sentence);
            }

            // 4
            //StopAllCoroutines();                    // Stop coroutine before starting new one.

            // 5
            //StartCoroutine(TypeSentence(sentence, clip)); // Display or type one character at a time.
        }

        [TearDown]
        public void Teardown()
        {
            Object.DestroyImmediate(dialogueCanvas);
            Object.DestroyImmediate(dialogueVRCanvas);
            Object.DestroyImmediate(dialogueTreeTest);

            sentences.Clear();
            sentenceAudioClips.Clear();
        }
    }
}
