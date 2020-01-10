using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEditor;

namespace Tests
{
    public class DialogueManagerTest
    {
        // Dialogue Canvas Elements
        public GameObject dialogueCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public Text nameText;
        public Text dialogueText;

        // Dialogue VR Canvas Elements
        public GameObject dialogueVRCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public Text nameVRText;
        public Text dialogueVRText;

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
            GameObject audioSourceObject = new GameObject();    // Might need to make global.

            audioSource = audioSourceObject.AddComponent<AudioSource>();
            dialogueCanvas = new GameObject();
            dialogueVRCanvas = new GameObject();
            dialogueTreeTest = AssetDatabase.LoadAssetAtPath<DialogueTree>("Assets/DialogueToolPackage/DialogueTreeAssets/Introduction.asset");
            sentences = new Queue<string>();
            sentenceAudioClips = new Queue<AudioClip>();

            Assert.IsNotNull(audioSource);
            Assert.IsNotNull(dialogueCanvas);
            Assert.IsNotNull(dialogueVRCanvas);
            Assert.IsNotNull(dialogueTreeTest);
            Assert.IsNotNull(sentences);
            Assert.IsNotNull(sentenceAudioClips);

            // The dialogueTreeTest object can also be set with these methods
            //dialogueTreeTest = Resources.Load<DialogueTree>("DialogueTreeAssets/Introduction");
            //dialogueTreeTest = (DialogueTree)ScriptableObject.CreateInstance("DialogueTree");
            //dialogueTreeTest.dialogueTreeElements.Add()
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

            nameText = nameTextObject.AddComponent<Text>();
            Assert.IsNotNull(nameText);
            Assert.IsEmpty(nameText.text);
            Assert.IsNotEmpty(dialogueTreeTest.characterName);

            nameText.text = dialogueTreeTest.characterName;
            Assert.AreEqual(nameText.text, dialogueTreeTest.characterName);

            // 5
            GameObject nameVRTextObject = new GameObject();
            nameVRText = nameVRTextObject.AddComponent<Text>();
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
            Assert.AreEqual(sentenceAudioClips.Count, dialogueTreeTest.dialogueTreeAudioClips.Count);

            // 9
            DisplayNextSentenceTest();
        }

        public void DisplayNextSentenceTest()
        {
            // 1
            if (sentences.Count == 0)
            {
                Assert.AreEqual(sentences.Count, 0);
                EndDialogue();
                return;
            }

            // 2
            string sentence = sentences.Peek();
            Assert.AreEqual(sentence, sentences.Peek());

            int previousSentenceCount = sentences.Count;
            sentences.Dequeue();
            Assert.AreEqual(sentences.Count, previousSentenceCount - 1);
            CollectionAssert.DoesNotContain(sentences, sentence);

            AudioClip clip = sentenceAudioClips.Peek();
            Assert.AreEqual(clip, sentenceAudioClips.Peek());

            int previousSentenceAudioClipsCount = sentenceAudioClips.Count;
            sentenceAudioClips.Dequeue();
            Assert.AreEqual(sentenceAudioClips.Count, previousSentenceAudioClipsCount - 1);
            CollectionAssert.DoesNotContain(sentenceAudioClips, clip);

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

        private IEnumerator TypeSentence(string sentence, AudioClip clip)
        {
            audioSource.Stop();

            if (playWithAudio)
            {
                if (clip)
                    audioSource.PlayOneShot(clip, volume);
                else
                    Debug.LogError("No audioclip for string displayed! Please place audioclip in AudioClip List for respective string element.");
            }

            if (instantPrint)
            {
                int punctutationCount = 0;

                foreach (char letter in sentence.ToCharArray())
                {
                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        punctutationCount++;
                    }
                }

                dialogueText.text = sentence;         // Display full sentence instantly
                dialogueVRText.text = sentence;         // Display full sentence instantly

                float fullSentenceDelay = (printLetterDelay * sentence.Length) + (punctutationCount * sentenceDelay) + sentenceDelay; // (CharacterCount from current dialogueTreeElement  * print delay time) + (number of punctuation characters * sentence delay time) + end of dialogueTreeElement delay time.

                if (debugComponent)
                    Debug.Log("fullSentenceDelay: " + fullSentenceDelay);

                if (!requireContinueButton)
                {
                    yield return new WaitForSeconds(fullSentenceDelay);
                    DisplayNextSentenceTest();
                }
            }
            else
            {
                dialogueText.text = "";
                dialogueVRText.text = "";

                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText.text += letter;
                    dialogueVRText.text += letter;

                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        yield return new WaitForSeconds(sentenceDelay);
                        //yield return null; // Wait a single frame/tick
                    }
                    else
                        yield return new WaitForSeconds(printLetterDelay);
                }

                // If moving on with the next dialogue to type requires input, then
                if (!requireContinueButton)
                {
                    // If last character is not any form of punctutation, then delay next sentence
                    if (!(sentence.EndsWith(",") || sentence.EndsWith(";") || sentence.EndsWith(".") || sentence.EndsWith("?") || sentence.EndsWith("!")))
                    {
                        yield return new WaitForSeconds(sentenceDelay);
                    }

                    DisplayNextSentenceTest();
                }
            }
        }

        private void EndDialogue()
        {
            audioSource.Stop();

            if (debugComponent)
                Debug.Log("End of conversation.");

            if (useOpenCloseAnimation)
            {
                dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", false);

                dialogueVRCanvas.GetComponent<Animator>().SetBool("isOpen", false);
            }
            else
            {
                //dialogueCanvas.SetActive(false);
                dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);

                dialogueVRCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
            }
        }

        [TearDown]
        public void Teardown()
        {
            audioSource = null;
            dialogueCanvas = null;
            dialogueVRCanvas = null;
            dialogueTreeTest = null;
            sentences = null;
            sentenceAudioClips = null;

            Assert.IsNull(audioSource);
            Assert.IsNull(dialogueCanvas);
            Assert.IsNull(dialogueVRCanvas);
            Assert.IsNull(dialogueTreeTest);
            Assert.IsNull(sentences);
            Assert.IsNull(sentenceAudioClips);
        }
    }
}
