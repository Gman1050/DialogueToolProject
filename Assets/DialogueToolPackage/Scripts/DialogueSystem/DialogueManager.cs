using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    /// <summary>
    /// A class that manages and modifies the entire dialogue system via settings.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager instance; // Static instance of the monobehavior

        [Header("Dialogue Canvas Elements:")]
        public GameObject dialogueCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public Text nameText;
        public Text dialogueText;
        public RawImage autoContinueDialogueRawImage;
        public Image inputContinueDialogueImage;

        [Header("Dialogue VR Canvas Elements:")]
        public GameObject dialogueVRCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxVRCanvas
        public Text nameVRText;
        public Text dialogueVRText;
        public RawImage autoContinueDialogueVRRawImage;
        public Image inputContinueDialogueVRImage;

        [Header("Dialogue Print Settings:")]
        [Range(650, 1800)] public float textDisplayWidth = 800.0f;
        [Range(0, 0.1f)] public float printLetterDelay = 0.1f;
        public bool instantPrintBegin = false;
        public bool printDialogue = true;
        private float currentPrintLetterDelay;

        [Header("Dialogue Input Settings:")]
        public bool requireContinueButton = false;

        // Requires requireContinueButton to be true
        public bool instantPrintFinish = true;  // Won't apply if instantPrintBegin is true
        public bool speedPrintFinish = false;   // Won't apply if instantPrintFinish is true

        [Header("Dialogue Delay Settings:")]
        [Range(0.25f, 2.0f)] public float sentenceDelay = 1.0f;
        private float currentSentenceDelay;

        [Header("Dialogue Animation/Image Settings:")]
        public bool useOpenCloseAnimation = false;
        [Range(0, 1)] public float inputContinueDialogueImageAnimationSpeed = 0.15f;
        [Range(0, 1)] public float autoContinueDialogueImageAnimationSpeed = 2.0f;
        public AudioClip openWithAnimation;
        public AudioClip closeWithAnimation;
        public AudioClip openWithoutAnimation;
        public AudioClip closeWithoutAnimation;

        [Header("Dialogue Audio Settings:")]
        [Range(0, 1)] public float volume = 1.0f;
        public bool playWithAudio = true;
        private AudioSource audioSource;

        [Header("Dialogue Test Settings:")]
        public bool playAtStart = false;
        public DialogueTree dialogueTreeTest;
        public bool useTestButtons = false;
        public GameObject testButtons;
        public GameObject testVRButtons;

        [Header("Debug Settings:")]
        public bool debugComponent = false;

        // Dialogue Queues
        private Queue<DialogueTree.DialogueNode> dialogueNodes; 

        private bool isTypeSentenceCoroutineRunning = false;
        private string currentSentence;

        void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            // Initialize Queues and AudioSource 
            audioSource = GetComponent<AudioSource>();
            dialogueNodes = new Queue<DialogueTree.DialogueNode>();

            // Initialize private fields with these settings
            currentPrintLetterDelay = printLetterDelay;
            currentSentenceDelay = sentenceDelay;

            // Update speed of animations with current settings
            inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
            inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
            autoContinueDialogueRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageAnimationSpeed;
            autoContinueDialogueVRRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageAnimationSpeed;

            // Used for testing
            if (playAtStart)
            {
                StartDialogue(dialogueTreeTest);
            }
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        void Update()
        {
            // Set values to false based on prequisite values
            if (!printDialogue)
                requireContinueButton = false;

            if (instantPrintFinish)
                speedPrintFinish = false;

           
            testButtons.SetActive(useTestButtons);
            testVRButtons.SetActive(useTestButtons);
        }

        /// <summary>
        /// A method to initiate the dialogueTree into a displayable UI.
        /// </summary>
        /// <param nodeCharacterName="dialogueTree">The scriptable object that will be used to extract string and audioclip data for dialogue.</param>
        public void StartDialogue(DialogueTree dialogueTree)
        {
            if (!printDialogue && !playWithAudio)
            {
                Debug.LogError("Cannot play dialogue! The printDialogue and playWithAudio booleans are false. Mark at least one of these as true in the inspector to start the dialogue.");
                return;
            }

            if (!requireContinueButton)
            {
                autoContinueDialogueRawImage.gameObject.SetActive(true);
                autoContinueDialogueRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageAnimationSpeed;
                autoContinueDialogueVRRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageAnimationSpeed;
            }

            // Choose to print string and play audio or just play audio without a respective string
            if (printDialogue)
            {
                // Open Dialogue Box with animation or setting local scale
                if (useOpenCloseAnimation)
                {
                    if (dialogueCanvas.activeSelf)
                    {
                        dialogueCanvas.GetComponent<Animator>().enabled = true;
                        dialogueCanvas.GetComponent<Animator>().SetBool("canTransition", true);
                        dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", true);
                    }
                    else if (dialogueVRCanvas.activeSelf)
                    {
                        dialogueVRCanvas.GetComponent<Animator>().enabled = true;
                        dialogueVRCanvas.GetComponent<Animator>().SetBool("canTransition", true);
                        dialogueVRCanvas.GetComponent<Animator>().SetBool("isOpen", true);
                    }

                    if (openWithAnimation)
                        audioSource.PlayOneShot(openWithAnimation);
                }
                else
                {
                    if (dialogueCanvas.activeSelf)
                    {
                        dialogueCanvas.GetComponent<Animator>().enabled = false;
                        dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    }
                    else if (dialogueVRCanvas.activeSelf)
                    {
                        dialogueVRCanvas.GetComponent<Animator>().enabled = false;
                        dialogueVRCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    }

                    if (openWithoutAnimation)
                        audioSource.PlayOneShot(openWithoutAnimation);
                }
            }

            // Clear queue for new dialogue to be used.
            dialogueNodes.Clear();

            foreach (DialogueTree.DialogueNode node in dialogueTree.dialogueNodeElements)
            {
                dialogueNodes.Enqueue(node);
            }

            // Display First Sentence of the queues
            DisplayNextSentence();
        }

        /// <summary>
        /// A method to play the next string and audioclip in the queues.
        /// </summary>
        public void DisplayNextSentence()
        {
            // Check to see if current nodeDialogueString is typing first
            if (isTypeSentenceCoroutineRunning)
            {
                // Only used if input is required
                if (requireContinueButton)
                {
                    // Instant print the rest of the current nodeDialogueString
                    if (instantPrintFinish)
                    {
                        StopAllCoroutines();                    // Stop coroutine that is currently printing.

                        dialogueText.text = currentSentence;
                        dialogueVRText.text = currentSentence;

                        isTypeSentenceCoroutineRunning = false; // Make sure this is false after nodeDialogueString is done typing.
                    }
                    else
                    {
                        // Change speed of the text without changing the value for the setting. Create private copy of the value.
                        if (speedPrintFinish)
                        {
                            // The fastest is actually one frame.
                            currentPrintLetterDelay = 0.0f;
                            currentSentenceDelay = 0.0f;
                        }
                    }

                    inputContinueDialogueImage.gameObject.SetActive(true);
                    inputContinueDialogueVRImage.gameObject.SetActive(true);

                    // Update speed of animation with current settings
                    inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
                    inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
                }
                return;
            }

            // Reset delay times
            currentPrintLetterDelay = printLetterDelay;
            currentSentenceDelay = sentenceDelay;

            // End dialogue if queues are empty
            if (dialogueNodes.Count == 0)
            {
                EndDialogue();
                return;
            }

            // Adjust textDisplayWidth to fit more center with the camera screen.
            dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y);
            dialogueVRText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y);

            // Save nodeDialogueString and audioclip that is being dequeued
            DialogueTree.DialogueNode dialogueNode = dialogueNodes.Peek();

            // Dequeue the current node so your not stuck on it for next call.
            dialogueNodes.Dequeue();

            if (debugComponent)
                Debug.Log(dialogueNode.nodeCharacterName + ": " + dialogueNode.nodeDialogueString);

            StopAllCoroutines();                            // Stop coroutine before starting new one.
            StartCoroutine(TypeNodeDialogueString(dialogueNode));   // Display or type one character at a time.
        }

        /// <summary>
        /// A coroutine method that will type each nodeDialogueString one character at a time at the set speed and play the audioclip if it is available.
        /// </summary>
        /// <param nodeCharacterName="nodeDialogueString">The current nodeDialogueString that is out of the queue.</param>
        /// <param nodeCharacterName="nodeDialogueAudioClip">The current audioclip that is out of the queue.</param>
        /// <returns></returns>
        private IEnumerator TypeNodeDialogueString(DialogueTree.DialogueNode dialogueNode)
        {
            isTypeSentenceCoroutineRunning = true;

            if (debugComponent)
                Debug.Log("isTypeSentenceCoroutineRunning: " + isTypeSentenceCoroutineRunning);

            string nodeCharacterName = dialogueNode.nodeCharacterName;
            string nodeDialogueString = dialogueNode.nodeDialogueString;
            AudioClip nodeDialogueAudioClip = dialogueNode.nodeDialogueAudioClip;

            // Set nodeCharacterName text fields with the nodeCharacterName of the person talking in the dialogueTree
            nameText.text = nodeCharacterName;
            nameVRText.text = nodeCharacterName;

            if (requireContinueButton)
            {
                inputContinueDialogueImage.gameObject.SetActive(false);
                inputContinueDialogueVRImage.gameObject.SetActive(false);
            }

            currentSentence = nodeDialogueString;

            audioSource.Stop();

            if (playWithAudio)
            {
                if (nodeDialogueAudioClip)
                    audioSource.PlayOneShot(nodeDialogueAudioClip, volume);
                else
                    Debug.LogError("No audioclip for string displayed! Please place audioclip in AudioClip List for respective string element.");
            }

            // Print full nodeDialogueString or type each character individually.
            if (instantPrintBegin)
            {
                int punctutationCount = 0;

                foreach (char letter in nodeDialogueString.ToCharArray())
                {
                    // If character is any form of punctutation, then delay next nodeDialogueString. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        punctutationCount++;    // Keep track of punctuation in each node
                    }
                }

                dialogueText.text = nodeDialogueString;         // Display full nodeDialogueString instantly
                dialogueVRText.text = nodeDialogueString;         // Display full nodeDialogueString instantly

                float fullSentenceDelay = (currentPrintLetterDelay * nodeDialogueString.Length) + (punctutationCount * currentSentenceDelay) + currentSentenceDelay; // (CharacterCount from current dialogueTreeElement  * print delay time) + (number of punctuation characters * nodeDialogueString delay time) + end of dialogueTreeElement delay time.

                if (debugComponent)
                    Debug.Log("fullSentenceDelay: " + fullSentenceDelay + ", nodeDialogueAudioClip.length: " + nodeDialogueAudioClip.length);

                // Play next nodeDialogueString without button input
                if (!requireContinueButton)
                {
                    if (nodeDialogueAudioClip)
                        yield return new WaitForSeconds(nodeDialogueAudioClip.length);
                    else
                        yield return new WaitForSeconds(fullSentenceDelay);

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.

                    DisplayNextSentence();
                }
                else
                {
                    DisplayNextSentence();

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.
                }
            }
            else
            {
                // Clear text fields before printing
                dialogueText.text = "";
                dialogueVRText.text = "";

                foreach (char letter in nodeDialogueString.ToCharArray())
                {
                    dialogueText.text += letter;
                    dialogueVRText.text += letter;

                    // If character is any form of punctutation, then delay next nodeDialogueString. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                        yield return new WaitForSeconds(currentSentenceDelay);      // Delay next nodeDialogueString
                    else
                        yield return new WaitForSeconds(currentPrintLetterDelay);   // Delay character print
                }

                // If moving on with the next dialogue to type requires input, then
                if (!requireContinueButton)
                {

                    // If last character is not any form of punctutation, then delay next nodeDialogueString
                    if (!(nodeDialogueString.EndsWith(",") || nodeDialogueString.EndsWith(";") || nodeDialogueString.EndsWith(".") || nodeDialogueString.EndsWith("?") || nodeDialogueString.EndsWith("!")))
                        yield return new WaitForSeconds(currentSentenceDelay);

                    yield return new WaitUntil(() => !audioSource.isPlaying); // Wait until audioclip for the dialogue nodeDialogueString has stopped playing if it hasn't.

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.

                    DisplayNextSentence();
                }
                else
                {
                    DisplayNextSentence();

                    isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.
                }
            }

            if (requireContinueButton)
            {
                inputContinueDialogueImage.gameObject.SetActive(true);
                inputContinueDialogueVRImage.gameObject.SetActive(true);

                // Update speed of animation with current settings
                inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
                inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageAnimationSpeed;
            }

            if (debugComponent)
                Debug.Log("isTypeSentenceCoroutineRunning: " + isTypeSentenceCoroutineRunning);
        }

        /// <summary>
        /// A method for ending current dialogue tree.
        /// </summary>
        private void EndDialogue()
        {
            // Stop audio
            audioSource.Stop();

            if (debugComponent)
                Debug.Log("End of conversation.");

            // Close the dialogue box with animation or local scale change.
            if (useOpenCloseAnimation)
            {
                if (dialogueCanvas.activeSelf)
                    dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", false);
                else if (dialogueVRCanvas.activeSelf)
                    dialogueVRCanvas.GetComponent<Animator>().SetBool("isOpen", false);

                if (closeWithAnimation)
                    audioSource.PlayOneShot(closeWithAnimation);
            }
            else
            {
                if (dialogueCanvas.activeSelf)
                    dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
                else if (dialogueVRCanvas.activeSelf)
                    dialogueVRCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);

                if (closeWithoutAnimation)
                    audioSource.PlayOneShot(closeWithoutAnimation);
            }

            inputContinueDialogueImage.gameObject.SetActive(false);
            inputContinueDialogueVRImage.gameObject.SetActive(false);
            autoContinueDialogueRawImage.gameObject.SetActive(false);
            autoContinueDialogueVRRawImage.gameObject.SetActive(false);
        }
    }
}