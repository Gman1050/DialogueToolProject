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

        private Animator animator, animatorVR;
        private RectTransform rt, rtVR;

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

            animator = dialogueCanvas.AddComponent<Animator>();
            rt = dialogueCanvas.AddComponent<RectTransform>();

            animatorVR = dialogueVRCanvas.AddComponent<Animator>();
            rtVR = dialogueVRCanvas.AddComponent<RectTransform>();

            Assert.IsNotNull(audioSource);
            Assert.IsNotNull(dialogueCanvas);
            Assert.IsNotNull(dialogueVRCanvas);
            Assert.IsNotNull(dialogueTreeTest);
            Assert.IsNotNull(sentences);
            Assert.IsNotNull(sentenceAudioClips);
            Assert.IsNotNull(animator);
            Assert.IsNotNull(rt);
            Assert.IsNotNull(animatorVR);
            Assert.IsNotNull(rtVR);

            // The dialogueTreeTest object can also be set with these methods
            //dialogueTreeTest = Resources.Load<DialogueTree>("DialogueTreeAssets/Introduction");
            //dialogueTreeTest = (DialogueTree)ScriptableObject.CreateInstance("DialogueTree");
            //dialogueTreeTest.dialogueTreeElements.Add()
        }

        [Test, Order(1)]
        public void StartDialogueTest()
        {
            // 1
            if (!printDialogue && !playWithAudio)
            {
                Assert.IsTrue(!printDialogue && !playWithAudio);
                Debug.LogError("Cannot play dialogue! The printDialogue and playWithAudio booleans are false. Mark at least one of these as true in the inspector to start the dialogue.");
                LogAssert.Expect(LogType.Error, "Cannot play dialogue! The printDialogue and playWithAudio booleans are false. Mark at least one of these as true in the inspector to start the dialogue.");
                return;
            }


            //Animator animator = dialogueCanvas.AddComponent<Animator>();
            //RectTransform rt = dialogueCanvas.AddComponent<RectTransform>();

            //Animator animatorVR = dialogueVRCanvas.AddComponent<Animator>();
            //RectTransform rtVR = dialogueVRCanvas.AddComponent<RectTransform>();

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
                    Assert.AreEqual(new Vector3(1, 1, 1), rt.localScale);

                    animatorVR.enabled = false;
                    rtVR.localScale = new Vector3(1, 1, 1);

                    Assert.IsFalse(animatorVR.enabled);
                    Assert.AreEqual(new Vector3(1, 1, 1), rtVR.localScale);
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
            Assert.AreEqual(dialogueTreeTest.characterName, nameText.text);

            // 5
            GameObject nameVRTextObject = new GameObject();
            nameVRText = nameVRTextObject.AddComponent<Text>();
            Assert.IsNotNull(nameVRTextObject);
            Assert.IsNotNull(nameVRText);

            nameVRText.text = dialogueTreeTest.characterName;
            Assert.AreEqual(dialogueTreeTest.characterName, nameVRText.text);

            // 6
            sentences.Clear();
            Assert.AreEqual(0, sentences.Count);

            // 7
            foreach (string sentence in dialogueTreeTest.dialogueTreeElements)
            {
                sentences.Enqueue(sentence);
            }
            Assert.AreEqual(dialogueTreeTest.dialogueTreeElements.Count, sentences.Count);

            // 8
            foreach (AudioClip clip in dialogueTreeTest.dialogueTreeAudioClips)
            {
                sentenceAudioClips.Enqueue(clip);
            }
            Assert.AreEqual(dialogueTreeTest.dialogueTreeAudioClips.Count, sentenceAudioClips.Count);

            // 9
            //DisplayNextSentenceTest();
        }

        [Test, Order(2)]
        public void DisplayNextSentenceTest()
        {
            // 1
            if (sentences.Count == 0)
            {
                Assert.AreEqual(0, sentences.Count);
                //EndDialogue();
                return;
            }
            Assert.AreNotEqual(0, sentences.Count);

            // 2
            string sentence = sentences.Peek();
            Assert.AreEqual(sentences.Peek(), sentence);

            int previousSentenceCount = sentences.Count;
            sentences.Dequeue();
            Assert.AreEqual(previousSentenceCount - 1, sentences.Count);
            CollectionAssert.DoesNotContain(sentences, sentence);

            AudioClip clip = sentenceAudioClips.Peek();
            Assert.AreEqual(sentenceAudioClips.Peek(), clip);

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

        //[Test, Order(3)]
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

        [Test, Order(3)]
        public void EndDialogue()
        {
            Assert.IsNotNull(audioSource);

            audioSource.Stop();
            
            Assert.IsFalse(audioSource.isPlaying);

            if (debugComponent)
            {
                Assert.IsTrue(debugComponent);
                Debug.Log("End of conversation.");
                LogAssert.Expect(LogType.Log, "End of conversation.");
            }

            if (useOpenCloseAnimation)
            {
                Assert.IsTrue(useOpenCloseAnimation);

                dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", false);
                dialogueVRCanvas.GetComponent<Animator>().SetBool("isOpen", false);

                Assert.IsTrue(dialogueCanvas.GetComponent<Animator>().GetBool("isOpen"));
                Assert.IsTrue(dialogueVRCanvas.GetComponent<Animator>().GetBool("isOpen"));
            }
            else
            {
                Assert.IsFalse(useOpenCloseAnimation);

                dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
                dialogueVRCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);

                Assert.AreEqual(new Vector3(1, 0, 1), dialogueCanvas.GetComponent<RectTransform>().localScale);
                Assert.AreEqual(new Vector3(1, 0, 1), dialogueVRCanvas.GetComponent<RectTransform>().localScale);
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
            animator = null;
            rt = null;
            animatorVR = null;
            rtVR = null;

            Assert.IsNull(audioSource);
            Assert.IsNull(dialogueCanvas);
            Assert.IsNull(dialogueVRCanvas);
            Assert.IsNull(dialogueTreeTest);
            Assert.IsNull(sentences);
            Assert.IsNull(sentenceAudioClips);
            Assert.IsNull(animator);
            Assert.IsNull(rt);
            Assert.IsNull(animatorVR);
            Assert.IsNull(rtVR);
        }
    }
}
