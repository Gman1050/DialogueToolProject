using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.TestTools;
using UnityEditor;
using DialogueSystem;

namespace Tests
{
    public class DialogueManagerTest
    {
        // Dialogue Canvas Elements
        public GameObject dialogueCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public Text nameText;
        public Text dialogueText;
        public RawImage autoContinueDialogueRawImage;
        public Image inputContinueDialogueImage;

        // Dialogue VR Canvas Elements
        public GameObject dialogueVRCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public Text nameVRText;
        public Text dialogueVRText;
        public RawImage autoContinueDialogueVRRawImage;
        public Image inputContinueDialogueVRImage;

        // Dialogue Print Settings
        public float textDisplayWidth = 800.0f;
        public float printLetterDelay = 0.1f;
        public bool instantPrintBegin = true;
        public bool printDialogue = true;
        private float currentPrintLetterDelay;

        // Dialogue Input Settings
        public bool requireContinueButton = false;

        // Requires requireContinueButton to be true
        public bool instantPrintFinish = true;  // Won't apply if instantPrintBegin is true
        public bool speedPrintFinish = false;   // Won't apply if instantPrintFinish is true

        // Dialogue Delay Settings
        public float sentenceDelay = 1.0f;
        private float currentSentenceDelay;

        // Dialogue Animation Settings
        public bool useOpenCloseAnimation = true;
        public float inputContinueDialogueImageAnimationSpeed = 0.15f;
        public float autoContinueDialogueImageAnimationSpeed = 2.0f;
        public AudioClip openWithAnimation;
        public AudioClip closeWithAnimation;
        public AudioClip openWithoutAnimation;
        public AudioClip closeWithoutAnimation;

        // Dialogue Audio Settings
        public float volume = 1.0f;
        public bool playWithAudio = false;
        private AudioSource audioSource;

        // Dialogue Test Settings
        public bool playAtStart = false;
        public DialogueTree dialogueTreeTest;
        public bool useTestButtons = false;
        public GameObject testButtons;
        public GameObject testVRButtons;

        // Debug Settings
        public bool debugComponent = false;

        // Dialogue Queues
        private Queue<DialogueTree.DialogueNode> dialogueNodes;

        private bool isTypeSentenceCoroutineRunning = false;
        private string currentSentence;

        private Animator dialogueCanvasAnimator, dialogueCanvasAnimatorVR;
        private Animator autoContinueDialogueImageAnimator, autoContinueDialogueImageAnimatorVR;
        private Animator inputContinueDialogueImageAnimator, inputContinueDialogueImageAnimatorVR;
        private RectTransform rt, rtVR;

        private DialogueTree.DialogueNode dialogueNodeToPlay;

        [SetUp]
        public void Setup()
        {
            audioSource = new GameObject().AddComponent<AudioSource>();
            dialogueCanvas = new GameObject();
            dialogueVRCanvas = new GameObject();
            dialogueTreeTest = AssetDatabase.LoadAssetAtPath<DialogueTree>("Assets/DialogueToolPackage/DialogueTreeAssets/Introduction.asset");
            dialogueNodes = new Queue<DialogueTree.DialogueNode>();

            dialogueCanvasAnimator = dialogueCanvas.AddComponent<Animator>();
            rt = dialogueCanvas.AddComponent<RectTransform>();

            dialogueCanvasAnimatorVR = dialogueVRCanvas.AddComponent<Animator>();
            rtVR = dialogueVRCanvas.AddComponent<RectTransform>();

            dialogueText = new GameObject().AddComponent<Text>();
            dialogueVRText = new GameObject().AddComponent<Text>();

            autoContinueDialogueRawImage = new GameObject().AddComponent<RawImage>();
            autoContinueDialogueVRRawImage = new GameObject().AddComponent<RawImage>();
            autoContinueDialogueImageAnimator = autoContinueDialogueRawImage.gameObject.AddComponent<Animator>();
            autoContinueDialogueImageAnimatorVR = autoContinueDialogueVRRawImage.gameObject.AddComponent<Animator>();

            inputContinueDialogueImage = new GameObject().AddComponent<Image>();
            inputContinueDialogueVRImage = new GameObject().AddComponent<Image>();
            inputContinueDialogueImageAnimator = inputContinueDialogueImage.gameObject.AddComponent<Animator>();
            inputContinueDialogueImageAnimatorVR = inputContinueDialogueVRImage.gameObject.AddComponent<Animator>();

            Assert.IsNotNull(audioSource);
            Assert.IsNotNull(dialogueCanvas);
            Assert.IsNotNull(dialogueVRCanvas);
            Assert.IsNotNull(dialogueTreeTest);
            Assert.IsNotNull(dialogueNodes);
            Assert.IsNotNull(dialogueCanvasAnimator);
            Assert.IsNotNull(rt);
            Assert.IsNotNull(dialogueCanvasAnimatorVR);
            Assert.IsNotNull(rtVR);
            Assert.IsNotNull(autoContinueDialogueRawImage);
            Assert.IsNotNull(autoContinueDialogueVRRawImage);
            Assert.IsNotNull(inputContinueDialogueImage);
            Assert.IsNotNull(inputContinueDialogueVRImage);

            dialogueCanvasAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/DialogueBoxCanvas/DialogueBoxCanvas.controller");
            dialogueCanvasAnimatorVR.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/DialogueBoxCanvas/DialogueBoxCanvas.controller");
            autoContinueDialogueImageAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/ContinueDialogueImages/AutoContinueDialogueImage/AutoContinueDialogueImage.controller");
            autoContinueDialogueImageAnimatorVR.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/ContinueDialogueImages/AutoContinueDialogueImage/AutoContinueDialogueImage.controller");
            inputContinueDialogueImageAnimator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/ContinueDialogueImages/InputContinueDialogueImage/InputContinueDialogueImage.controller");
            inputContinueDialogueImageAnimatorVR.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/DialogueToolPackage/Animations/ContinueDialogueImages/InputContinueDialogueImage/InputContinueDialogueImage.controller");

            Assert.IsNotNull(dialogueCanvasAnimator.runtimeAnimatorController);
            Assert.IsNotNull(dialogueCanvasAnimatorVR.runtimeAnimatorController);
            Assert.IsNotNull(autoContinueDialogueImageAnimator.runtimeAnimatorController);
            Assert.IsNotNull(autoContinueDialogueImageAnimatorVR.runtimeAnimatorController);
            Assert.IsNotNull(inputContinueDialogueImageAnimator.runtimeAnimatorController);
            Assert.IsNotNull(inputContinueDialogueImageAnimatorVR.runtimeAnimatorController);
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

            if (!requireContinueButton)
            {
                Assert.IsTrue(!requireContinueButton);

                autoContinueDialogueRawImage.gameObject.SetActive(true);
                autoContinueDialogueVRRawImage.gameObject.SetActive(true);
                autoContinueDialogueRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageAnimationSpeed;
                autoContinueDialogueVRRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageAnimationSpeed;

                Assert.IsTrue(autoContinueDialogueRawImage.gameObject.activeSelf);
                Assert.IsTrue(autoContinueDialogueVRRawImage.gameObject.activeSelf);
                Assert.AreEqual(autoContinueDialogueImageAnimationSpeed, autoContinueDialogueRawImage.GetComponent<Animator>().speed);
                Assert.AreEqual(autoContinueDialogueImageAnimationSpeed, autoContinueDialogueVRRawImage.GetComponent<Animator>().speed);
            }

            // 2
            if (printDialogue)
            {
                Assert.IsTrue(printDialogue);
                
                // 2a
                if (useOpenCloseAnimation)
                {
                    Assert.IsTrue(useOpenCloseAnimation);

                    if (dialogueCanvas.activeSelf)
                    {
                        Assert.IsTrue(dialogueCanvas.activeSelf);

                        dialogueCanvasAnimator.enabled = true;
                        dialogueCanvasAnimator.SetBool("canTransition", true);
                        dialogueCanvasAnimator.SetBool("isOpen", true);

                        Assert.IsTrue(dialogueCanvasAnimator.enabled);
                        Assert.IsTrue(dialogueCanvasAnimator.GetBool("canTransition"));
                        Assert.IsTrue(dialogueCanvasAnimator.GetBool("isOpen"));
                    }
                    else if (dialogueVRCanvas.activeSelf)
                    {
                        Assert.IsTrue(dialogueVRCanvas.activeSelf);

                        dialogueCanvasAnimatorVR.enabled = true;
                        dialogueCanvasAnimatorVR.SetBool("canTransition", true);
                        dialogueCanvasAnimatorVR.SetBool("isOpen", true);

                        Assert.IsTrue(dialogueCanvasAnimatorVR.enabled);
                        Assert.IsTrue(dialogueCanvasAnimatorVR.GetBool("canTransition"));
                        Assert.IsTrue(dialogueCanvasAnimatorVR.GetBool("isOpen"));
                    }

                    if (openWithAnimation)
                    {
                        Assert.IsTrue(openWithAnimation);

                        audioSource.PlayOneShot(openWithAnimation);

                        Assert.IsTrue(audioSource.isPlaying);
                    }
                }

                // 2b
                else
                {
                    Assert.IsFalse(useOpenCloseAnimation);

                    if (dialogueCanvas.activeSelf)
                    {
                        Assert.IsTrue(dialogueCanvas.activeSelf);

                        dialogueCanvasAnimator.enabled = false;
                        rt.localScale = new Vector3(1, 1, 1);

                        Assert.IsFalse(dialogueCanvasAnimator.enabled);
                        Assert.AreEqual(new Vector3(1, 1, 1), rt.localScale);
                    }
                    else if (dialogueVRCanvas.activeSelf)
                    {
                        Assert.IsTrue(dialogueVRCanvas.activeSelf);

                        dialogueCanvasAnimatorVR.enabled = false;
                        rtVR.localScale = new Vector3(1, 1, 1);

                        Assert.IsFalse(dialogueCanvasAnimatorVR.enabled);
                        Assert.AreEqual(new Vector3(1, 1, 1), rtVR.localScale);
                    }

                    if (openWithoutAnimation)
                    {
                        Assert.IsTrue(openWithAnimation);

                        audioSource.PlayOneShot(openWithAnimation);

                        Assert.IsTrue(audioSource.isPlaying);
                    }
                }
            }

            // 3
            GameObject nameTextObject = new GameObject();
            Assert.IsNotNull(nameTextObject);

            nameText = nameTextObject.AddComponent<Text>();
            Assert.IsNotNull(nameText);
            Assert.IsEmpty(nameText.text);

            // 4
            GameObject nameVRTextObject = new GameObject();
            nameVRText = nameVRTextObject.AddComponent<Text>();
            Assert.IsNotNull(nameVRTextObject);
            Assert.IsNotNull(nameVRText);

            // 5
            dialogueNodes.Clear();
            Assert.AreEqual(0, dialogueNodes.Count);

            // 6
            foreach (DialogueTree.DialogueNode node in dialogueTreeTest.dialogueNodeElements)
            {
                dialogueNodes.Enqueue(node);
            }
            Assert.AreEqual(dialogueTreeTest.dialogueNodeElements.Count, dialogueNodes.Count);

            // 7
            DisplayNextSentenceTest();
        }

        [Test, Order(2)]
        public void DisplayNextSentenceTest()
        {
            // 1
            // Check to see if current nodeDialogueString is typing first
            if (isTypeSentenceCoroutineRunning)
            {
                Assert.IsTrue(isTypeSentenceCoroutineRunning);

                // 1a
                // Only used if input is required
                if (requireContinueButton)
                {
                    Assert.IsTrue(requireContinueButton);

                    // 1aa
                    // Instant print the rest of the current nodeDialogueString
                    if (instantPrintFinish)
                    {
                        Assert.IsTrue(instantPrintFinish);

                        //StopAllCoroutines();                    // Stop coroutine that is currently printing.

                        dialogueText.text = currentSentence;
                        dialogueVRText.text = currentSentence;

                        Assert.AreEqual(currentSentence, dialogueText.text);
                        Assert.AreEqual(currentSentence, dialogueVRText.text);

                        isTypeSentenceCoroutineRunning = false; // Make sure this is false after nodeDialogueString is done typing.

                        Assert.IsFalse(isTypeSentenceCoroutineRunning);
                    }

                    // 1ab
                    else
                    {
                        Assert.IsFalse(instantPrintFinish);

                        // 1aba
                        // Change speed of the text without changing the value for the setting. Create private copy of the value.
                        if (speedPrintFinish)
                        {
                            Assert.IsTrue(speedPrintFinish);

                            // The fastest is actually one frame.
                            currentPrintLetterDelay = 0.0f;
                            currentSentenceDelay = 0.0f;

                            Assert.AreEqual(0, currentPrintLetterDelay);
                            Assert.AreEqual(0, currentSentenceDelay);
                        }
                    }

                    // 1ac
                    if (requireContinueButton)
                    {
                        Assert.IsTrue(requireContinueButton);

                        inputContinueDialogueImage.gameObject.SetActive(true);
                        inputContinueDialogueVRImage.gameObject.SetActive(true);

                        // Update speed of animation with current settings
                        inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
                        inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;

                        Assert.IsTrue(inputContinueDialogueImage.gameObject.activeSelf);
                        Assert.IsTrue(inputContinueDialogueVRImage.gameObject.activeSelf);
                        Assert.AreEqual(inputContinueDialogueImageAnimationSpeed, inputContinueDialogueImage.GetComponent<Animator>().speed);
                        Assert.AreEqual(inputContinueDialogueImageAnimationSpeed, inputContinueDialogueVRImage.GetComponent<Animator>().speed);
                    }
                }

                return;
            }

            Assert.IsFalse(isTypeSentenceCoroutineRunning);

            // 2
            // Reset delay times
            currentPrintLetterDelay = printLetterDelay;
            currentSentenceDelay = sentenceDelay;

            Assert.AreEqual(printLetterDelay, currentPrintLetterDelay);
            Assert.AreEqual(sentenceDelay, currentSentenceDelay);

            // 3
            if (dialogueNodes.Count == 0)
            {
                Assert.AreEqual(0, dialogueNodes.Count);
                EndDialogue();
                return;
            }
            Assert.AreNotEqual(0, dialogueNodes.Count);

            // 4
            dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y);
            dialogueVRText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y);
            Assert.AreEqual(new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y), dialogueText.GetComponent<RectTransform>().sizeDelta);
            Assert.AreEqual(new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y), dialogueVRText.GetComponent<RectTransform>().sizeDelta);

            // 5
            DialogueTree.DialogueNode dialogueNode = dialogueNodes.Peek();
            int previousSentenceCount = dialogueNodes.Count;
            dialogueNodes.Dequeue();
            Assert.AreEqual(previousSentenceCount - 1, dialogueNodes.Count);
            CollectionAssert.DoesNotContain(dialogueNodes, dialogueNode);

            // 6
            if (debugComponent)
            {
                Assert.IsTrue(debugComponent);
                Debug.Log(dialogueNode.nodeCharacterName + ": " + dialogueNode.nodeDialogueString);
                LogAssert.Expect(LogType.Log, dialogueNode.nodeCharacterName + ": " + dialogueNode.nodeDialogueString);
            }

            // 7
            //StopAllCoroutines();                    // Stop coroutine before starting new one.

            // 8
            //StartCoroutine(TypeSentenceTest(sentence, clip)); // Display or type one character at a time.

            // 9
            dialogueNodeToPlay = dialogueNode;
            Assert.AreEqual(dialogueNode, dialogueNodeToPlay);
        }

        [UnityTest, Order(3)]
        public IEnumerator TypeSentenceTest()
        {
            isTypeSentenceCoroutineRunning = true;
            Assert.IsTrue(isTypeSentenceCoroutineRunning);

            if (debugComponent)
            {
                Assert.IsTrue(debugComponent);
                Debug.Log("isTypeSentenceCoroutineRunning: " + isTypeSentenceCoroutineRunning);
                LogAssert.Expect(LogType.Log, "isTypeSentenceCoroutineRunning: " + isTypeSentenceCoroutineRunning);
            }

            DialogueTree.DialogueNode dialogueNode = dialogueNodeToPlay;
            Assert.AreEqual(dialogueNodeToPlay, dialogueNode);

            string nodeCharacterName = dialogueNode.nodeCharacterName;
            string nodeDialogueString = dialogueNode.nodeDialogueString;
            AudioClip nodeDialogueAudioClip = dialogueNode.nodeDialogueAudioClip;

            Assert.AreEqual(dialogueNode.nodeCharacterName, nodeCharacterName);
            Assert.AreEqual(dialogueNode.nodeDialogueString, nodeDialogueString);
            Assert.AreEqual(dialogueNode.nodeDialogueAudioClip, nodeDialogueAudioClip);

            // Set nodeCharacterName text fields with the nodeCharacterName of the person talking in the dialogueTree
            nameText.text = nodeCharacterName;
            nameVRText.text = nodeCharacterName;

            Assert.AreEqual(nodeCharacterName, nameText.text);
            Assert.AreEqual(nodeCharacterName, nameVRText.text);

            if (requireContinueButton)
            {
                Assert.IsTrue(requireContinueButton);

                inputContinueDialogueImage.gameObject.SetActive(false);
                inputContinueDialogueVRImage.gameObject.SetActive(false);

                Assert.IsTrue(inputContinueDialogueImage.gameObject.activeSelf);
                Assert.IsTrue(inputContinueDialogueVRImage.gameObject.activeSelf);
            }

            currentSentence = nodeDialogueString;
            Assert.AreEqual(nodeDialogueString, currentSentence);

            audioSource.Stop();
            Assert.IsFalse(audioSource.isPlaying);

            if (playWithAudio)
            {
                Assert.IsTrue(playWithAudio);

                if (nodeDialogueAudioClip)
                {
                    Assert.IsNotNull(nodeDialogueAudioClip);

                    audioSource.PlayOneShot(nodeDialogueAudioClip, volume);

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

            if (instantPrintBegin)
            {
                Assert.IsTrue(instantPrintBegin);

                int punctutationCount = 0;
                Assert.AreEqual(0, punctutationCount);

                foreach (char letter in nodeDialogueString.ToCharArray())
                {
                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        Assert.IsTrue(letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!');
                        punctutationCount++;
                    }
                }

                dialogueText.text = nodeDialogueString;         // Display full sentence instantly
                dialogueVRText.text = nodeDialogueString;         // Display full sentence instantly
                Assert.AreEqual(nodeDialogueString, dialogueText.text);
                Assert.AreEqual(nodeDialogueString, dialogueVRText.text);

                float fullSentenceDelay = (printLetterDelay * nodeDialogueString.Length) + (punctutationCount * sentenceDelay) + sentenceDelay; // (CharacterCount from current dialogueTreeElement  * print delay time) + (number of punctuation characters * sentence delay time) + end of dialogueTreeElement delay time.
                Assert.AreEqual((printLetterDelay * nodeDialogueString.Length) + (punctutationCount * sentenceDelay) + sentenceDelay, fullSentenceDelay);

                if (debugComponent)
                {
                    Assert.IsTrue(debugComponent);
                    Debug.Log("fullSentenceDelay: " + fullSentenceDelay + ", nodeDialogueAudioClip.length: " + nodeDialogueAudioClip.length);
                    LogAssert.Expect(LogType.Log, "fullSentenceDelay: " + fullSentenceDelay + ", nodeDialogueAudioClip.length: " + nodeDialogueAudioClip.length);
                }

                if (!requireContinueButton)
                {
                    Assert.IsFalse(requireContinueButton);

                    if (nodeDialogueAudioClip)
                    {
                        Assert.IsNotNull(nodeDialogueAudioClip);

#if UNITY_EDITOR
                        yield return null;  // Used in edit mode only
#else
                        yield return new WaitForSeconds(nodeDialogueAudioClip.length);
#endif
                    }
                    else
                    {
                        Assert.IsNull(nodeDialogueAudioClip);

#if UNITY_EDITOR
                        yield return null;  // Used in edit mode only
#else
                        yield return new WaitForSeconds(fullSentenceDelay);
#endif
                    }

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.

                    Assert.IsFalse(isTypeSentenceCoroutineRunning);

                    DisplayNextSentenceTest();
                }
                else
                {
                    Assert.IsTrue(requireContinueButton);

                    DisplayNextSentenceTest();

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.

                    Assert.IsFalse(isTypeSentenceCoroutineRunning);
                }
            }
            else
            {
                Assert.IsFalse(instantPrintBegin);

                dialogueText.text = "";
                dialogueVRText.text = "";
                Assert.AreEqual("", dialogueText.text);
                Assert.AreEqual("", dialogueVRText.text);

                foreach (char letter in nodeDialogueString.ToCharArray())
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
#if UNITY_EDITOR
                        yield return null;  // Used in edit mode only
#else
                        yield return new WaitForSeconds(currentSentenceDelay);      // Delay next nodeDialogueString
#endif

                    }
                    else
                    {
                        Assert.IsFalse(letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!');
#if UNITY_EDITOR
                        yield return null;  // Used in edit mode only
#else
                        yield return new WaitForSeconds(currentPrintLetterDelay);   // Delay character print
#endif

                    }
                }

                // If moving on with the next dialogue to type requires input, then
                if (!requireContinueButton)
                {
                    Assert.IsFalse(requireContinueButton);

                    // If last character is not any form of punctutation, then delay next sentence
                    if (!(nodeDialogueString.EndsWith(",") || nodeDialogueString.EndsWith(";") || nodeDialogueString.EndsWith(".") || nodeDialogueString.EndsWith("?") || nodeDialogueString.EndsWith("!")))
                    {
                        Assert.IsTrue(!(nodeDialogueString.EndsWith(",") || nodeDialogueString.EndsWith(";") || nodeDialogueString.EndsWith(".") || nodeDialogueString.EndsWith("?") || nodeDialogueString.EndsWith("!")));
#if UNITY_EDITOR
                        yield return null;  // Used in edit mode only
#else
                        yield return new WaitForSeconds(currentSentenceDelay);
#endif
                    }

#if UNITY_EDITOR
                    yield return null;  // Used in edit mode only
#else
                    yield return new WaitUntil(() => !audioSource.isPlaying); // Wait until audioclip for the dialogue nodeDialogueString has stopped playing if it hasn't.
#endif

                    Assert.IsFalse(audioSource.isPlaying);

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.

                    Assert.IsFalse(isTypeSentenceCoroutineRunning);

                    DisplayNextSentenceTest();
                }
                else
                {
                    Assert.IsTrue(requireContinueButton);

                    DisplayNextSentenceTest();

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.

                    Assert.IsFalse(isTypeSentenceCoroutineRunning);
                }
            }

            if (requireContinueButton)
            {
                Assert.IsTrue(requireContinueButton);

                inputContinueDialogueImage.gameObject.SetActive(true);
                inputContinueDialogueVRImage.gameObject.SetActive(true);

                Assert.IsTrue(inputContinueDialogueImage.gameObject.activeSelf);
                Assert.IsTrue(inputContinueDialogueVRImage.gameObject.activeSelf);

                // Update speed of animation with current settings
                inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
                inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;

                Assert.AreEqual(inputContinueDialogueImageAnimationSpeed, inputContinueDialogueImage.GetComponent<Animator>().speed);
                Assert.AreEqual(inputContinueDialogueImageAnimationSpeed, inputContinueDialogueVRImage.GetComponent<Animator>().speed);
            }
            else
                Assert.IsFalse(requireContinueButton);

            if (debugComponent)
            {
                Assert.IsTrue(debugComponent);
                Debug.Log("isTypeSentenceCoroutineRunning: " + isTypeSentenceCoroutineRunning);
                LogAssert.Expect(LogType.Log, "isTypeSentenceCoroutineRunning: " + isTypeSentenceCoroutineRunning);
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

                if (dialogueCanvas.activeSelf)
                {
                    dialogueCanvasAnimator.SetBool("isOpen", false);
                    Assert.IsFalse(dialogueCanvasAnimator.GetBool("isOpen"));
                }
                else if (dialogueVRCanvas.activeSelf)
                {
                    dialogueCanvasAnimatorVR.SetBool("isOpen", false);
                    Assert.IsFalse(dialogueCanvasAnimatorVR.GetBool("isOpen"));
                }

                if (closeWithAnimation)
                {
                    Assert.IsTrue(closeWithAnimation);

                    audioSource.PlayOneShot(closeWithAnimation);

                    Assert.IsTrue(audioSource.isPlaying);
                }
            }
            else
            {
                Assert.IsFalse(useOpenCloseAnimation);

                rt.localScale = new Vector3(1, 0, 1);
                rtVR.localScale = new Vector3(1, 0, 1);

                Assert.AreEqual(new Vector3(1, 0, 1), rt.localScale);
                Assert.AreEqual(new Vector3(1, 0, 1), rtVR.localScale);

                if (closeWithoutAnimation)
                {
                    Assert.IsTrue(closeWithoutAnimation);

                    audioSource.PlayOneShot(closeWithAnimation);

                    Assert.IsTrue(audioSource.isPlaying);
                }
            }

            inputContinueDialogueImage.gameObject.SetActive(false);
            inputContinueDialogueVRImage.gameObject.SetActive(false);
            autoContinueDialogueRawImage.gameObject.SetActive(false);
            autoContinueDialogueVRRawImage.gameObject.SetActive(false);

            Assert.IsFalse(inputContinueDialogueImage.gameObject.activeSelf);
            Assert.IsFalse(inputContinueDialogueVRImage.gameObject.activeSelf);
            Assert.IsFalse(autoContinueDialogueRawImage.gameObject.activeSelf);
            Assert.IsFalse(autoContinueDialogueVRRawImage.gameObject.activeSelf);
        }

        [TearDown]
        public void Teardown()
        {
            audioSource = null;
            dialogueCanvas = null;
            dialogueVRCanvas = null;
            dialogueTreeTest = null;
            dialogueNodes = null;
            dialogueCanvasAnimator = null;
            rt = null;
            dialogueCanvasAnimatorVR = null;
            rtVR = null;

            autoContinueDialogueRawImage = null;
            autoContinueDialogueVRRawImage = null;
            autoContinueDialogueImageAnimator = null;
            autoContinueDialogueImageAnimatorVR = null;

            inputContinueDialogueImage = null;
            inputContinueDialogueVRImage = null;
            inputContinueDialogueImageAnimator = null;
            inputContinueDialogueImageAnimatorVR = null;

            Assert.IsNull(audioSource);
            Assert.IsNull(dialogueCanvas);
            Assert.IsNull(dialogueVRCanvas);
            Assert.IsNull(dialogueTreeTest);
            Assert.IsNull(dialogueNodes);
            Assert.IsNull(dialogueCanvasAnimator);
            Assert.IsNull(rt);
            Assert.IsNull(dialogueCanvasAnimatorVR);
            Assert.IsNull(rtVR);
            Assert.IsNull(autoContinueDialogueRawImage);
            Assert.IsNull(autoContinueDialogueVRRawImage);
            Assert.IsNull(autoContinueDialogueImageAnimator);
            Assert.IsNull(autoContinueDialogueImageAnimatorVR);
            Assert.IsNull(inputContinueDialogueImage);
            Assert.IsNull(inputContinueDialogueVRImage);
            Assert.IsNull(inputContinueDialogueImageAnimator);
            Assert.IsNull(inputContinueDialogueImageAnimatorVR);
        }
    }
}
