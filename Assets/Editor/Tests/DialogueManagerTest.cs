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
        public float textDisplayWidth = 800.0f;
        public float printLetterDelay = 0.1f;
        public bool instantPrint = true;
        public bool printDialogue = true;

        // Dialogue Input Settings
        public bool requireContinueButton = false;

        // Dialogue Delay Settings
        public float sentenceDelay = 1.0f;

        // Dialogue Animation Settings
        public bool useOpenCloseAnimation = true;

        // Dialogue Audio Settings
        public float volume = 1.0f;
        public bool playWithAudio = false;
        private AudioSource audioSource;

        // Dialogue Test Settings
        public bool playAtStart = false;
        public DialogueTree dialogueTreeTest;

        // Debug Settings
        public bool debugComponent = true;

        // Dialogue Queues
        private Queue<string> sentences;
        private Queue<AudioClip> sentenceAudioClips;

        private Animator animator, animatorVR;
        private RectTransform rt, rtVR;

        private string sentenceStringToPlay = "";
        private AudioClip sentenceAudioClipToPlay;

        [SetUp]
        public void Setup()
        {
            GameObject audioSourceObject = new GameObject();    // Might need to make global.

            audioSource = audioSourceObject.AddComponent<AudioSource>();
            dialogueCanvas = new GameObject();
            //dialogueCanvas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DialogueToolPackage/Prefabs/DialogueBoxCanvas.prefab").transform.GetChild(0).gameObject;
            dialogueVRCanvas = new GameObject();
            //dialogueVRCanvas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/DialogueToolPackage/Prefabs/DialogueBoxVRCanvas.prefab").transform.GetChild(0).gameObject;
            dialogueTreeTest = AssetDatabase.LoadAssetAtPath<DialogueTree>("Assets/DialogueToolPackage/DialogueTreeAssets/Introduction.asset");
            sentences = new Queue<string>();
            sentenceAudioClips = new Queue<AudioClip>();

            animator = dialogueCanvas.AddComponent<Animator>();
            //animator = dialogueCanvas.GetComponent<Animator>();
            rt = dialogueCanvas.AddComponent<RectTransform>();
            //rt = dialogueCanvas.GetComponent<RectTransform>();

            animatorVR = dialogueVRCanvas.AddComponent<Animator>();
            //animatorVR = dialogueVRCanvas.GetComponent<Animator>();
            rtVR = dialogueVRCanvas.AddComponent<RectTransform>();
            //rtVR = dialogueVRCanvas.GetComponent<RectTransform>();

            dialogueText = new GameObject().AddComponent<Text>();
            //dialogueText = dialogueCanvas.transform.GetChild(1).GetComponent<Text>();
            dialogueVRText = new GameObject().AddComponent<Text>();
            //dialogueVRText = dialogueVRCanvas.transform.GetChild(1).GetComponent<Text>();

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

            animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/DialogueBoxCanvas.controller");
            animatorVR.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/DialogueBoxCanvas.controller");

            Assert.IsNotNull(animator.runtimeAnimatorController);
            Assert.IsNotNull(animatorVR.runtimeAnimatorController);

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
                LogAssert.Expect(LogType.Log, "Start conversation with " + dialogueTreeTest.characterName);
            }

            // 4
            GameObject nameTextObject = new GameObject();
            Assert.IsNotNull(nameTextObject);

            nameText = nameTextObject.AddComponent<Text>();
            //nameText = dialogueCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
            Assert.IsNotNull(nameText);
            Assert.IsEmpty(nameText.text);
            Assert.IsNotEmpty(dialogueTreeTest.characterName);

            nameText.text = dialogueTreeTest.characterName;
            Assert.AreEqual(dialogueTreeTest.characterName, nameText.text);

            // 5
            GameObject nameVRTextObject = new GameObject();
            nameVRText = nameVRTextObject.AddComponent<Text>();
            //nameVRText = dialogueVRCanvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();
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
            DisplayNextSentenceTest();
        }

        [Test, Order(2)]
        public void DisplayNextSentenceTest()
        {
            // 1
            if (sentences.Count == 0)
            {
                Assert.AreEqual(0, sentences.Count);
                EndDialogue();
                return;
            }
            Assert.AreNotEqual(0, sentences.Count);

            // 2
            dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y);
            dialogueVRText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y);
            Assert.AreEqual(new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y), dialogueText.GetComponent<RectTransform>().sizeDelta);
            Assert.AreEqual(new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y), dialogueVRText.GetComponent<RectTransform>().sizeDelta);

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
                LogAssert.Expect(LogType.Log, sentence);
            }

            // 4
            //StopAllCoroutines();                    // Stop coroutine before starting new one.

            // 5
            //StartCoroutine(TypeSentence(sentence, clip)); // Display or type one character at a time.
            sentenceStringToPlay = sentence;
            sentenceAudioClipToPlay = clip;

            Assert.AreEqual(sentence, sentenceStringToPlay);
            Assert.AreEqual(clip, sentenceAudioClipToPlay);
        }

        [UnityTest, Order(3)]
        public IEnumerator TypeSentence()
        {
            string sentence = sentenceStringToPlay;     // FIXME: Value not reinitialized in DisplayNextSentenceTest.
            AudioClip clip = sentenceAudioClipToPlay;   // FIXME: Value not reinitialized in DisplayNextSentenceTest.

            Assert.AreEqual(sentenceStringToPlay, sentence);
            Assert.AreEqual(sentenceAudioClipToPlay, clip);

            audioSource.Stop();
            Assert.IsFalse(audioSource.isPlaying);

            if (playWithAudio)
            {
                Assert.IsTrue(playWithAudio);
                if (clip)
                {
                    audioSource.PlayOneShot(clip, volume);
                    Assert.IsTrue(audioSource.isPlaying);
                }
                else
                {
                    Debug.LogError("No audioclip for string displayed! Please place audioclip in AudioClip List for respective string element.");
                    LogAssert.Expect(LogType.Error, "No audioclip for string displayed! Please place audioclip in AudioClip List for respective string element.");
                }
            }
            else
                Assert.IsFalse(playWithAudio);

            if (instantPrint)
            {
                Assert.IsTrue(instantPrint);

                int punctutationCount = 0;
                Assert.AreEqual(0, punctutationCount);

                foreach (char letter in sentence.ToCharArray())
                {
                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        Assert.IsTrue(letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!');
                        punctutationCount++;
                    }
                }

                dialogueText.text = sentence;         // Display full sentence instantly
                dialogueVRText.text = sentence;         // Display full sentence instantly
                Assert.AreEqual(sentence, dialogueText.text);
                Assert.AreEqual(sentence, dialogueVRText.text);

                float fullSentenceDelay = (printLetterDelay * sentence.Length) + (punctutationCount * sentenceDelay) + sentenceDelay; // (CharacterCount from current dialogueTreeElement  * print delay time) + (number of punctuation characters * sentence delay time) + end of dialogueTreeElement delay time.
                Assert.AreEqual((printLetterDelay * sentence.Length) + (punctutationCount * sentenceDelay) + sentenceDelay, fullSentenceDelay);

                if (debugComponent)
                {
                    Assert.IsTrue(debugComponent);
                    Debug.Log("fullSentenceDelay: " + fullSentenceDelay);
                    LogAssert.Expect(LogType.Log, "fullSentenceDelay: " + fullSentenceDelay);
                }

                if (!requireContinueButton)
                {
                    Assert.IsFalse(requireContinueButton);
                    //yield return new WaitForSeconds(fullSentenceDelay);
                    yield return null;
                    //DisplayNextSentenceTest();
                }
            }
            else
            {
                dialogueText.text = "";
                dialogueVRText.text = "";
                Assert.AreEqual(sentence, dialogueText.text);
                Assert.AreEqual(sentence, dialogueVRText.text);

                foreach (char letter in sentence.ToCharArray())
                {
                    string previousDialogueTextValue = dialogueText.text;
                    string previousDialogueVRTextValue = dialogueVRText.text;

                    dialogueText.text += letter;
                    dialogueVRText.text += letter;
                    Assert.AreEqual(dialogueText.text, previousDialogueTextValue + letter);
                    Assert.AreEqual(dialogueVRText.text, previousDialogueVRTextValue + letter);

                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        Assert.IsTrue(letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!');
                        //yield return new WaitForSeconds(sentenceDelay);
                        yield return null; // Wait a single frame/tick
                    }
                    else
                    {
                        Assert.IsFalse(letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!');
                        yield return null; // Wait a single frame/tick
                        //yield return new WaitForSeconds(printLetterDelay);
                    }
                }

                // If moving on with the next dialogue to type requires input, then
                if (!requireContinueButton)
                {
                    Assert.IsFalse(requireContinueButton);
                    // If last character is not any form of punctutation, then delay next sentence
                    if (!(sentence.EndsWith(",") || sentence.EndsWith(";") || sentence.EndsWith(".") || sentence.EndsWith("?") || sentence.EndsWith("!")))
                    {
                        Assert.IsTrue(!(sentence.EndsWith(",") || sentence.EndsWith(";") || sentence.EndsWith(".") || sentence.EndsWith("?") || sentence.EndsWith("!")));
                        //yield return new WaitForSeconds(sentenceDelay);
                        yield return null; // Wait a single frame/tick
                    }

                    DisplayNextSentenceTest();
                }
            }
        }

        [Test, Order(4)]
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

                animator.SetBool("isOpen", false);
                animatorVR.SetBool("isOpen", false);

                Assert.IsFalse(animator.GetBool("isOpen"));
                Assert.IsFalse(animatorVR.GetBool("isOpen"));
            }
            else
            {
                Assert.IsFalse(useOpenCloseAnimation);

                rt.localScale = new Vector3(1, 0, 1);
                rtVR.localScale = new Vector3(1, 0, 1);

                Assert.AreEqual(new Vector3(1, 0, 1), rt.localScale);
                Assert.AreEqual(new Vector3(1, 0, 1), rtVR.localScale);
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
