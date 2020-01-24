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
        [Range(0, 1)] public float inputContinueDialogueImageFadeSpeed = 0.15f;
        [Range(0, 1)] public float autoContinueDialogueImageScrollSpeed = 2.0f;
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
        private Queue<string> sentences;
        private Queue<AudioClip> sentenceAudioClips;

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
            sentences = new Queue<string>();
            sentenceAudioClips = new Queue<AudioClip>();

            // Initialize private fields with these settings
            currentPrintLetterDelay = printLetterDelay;
            currentSentenceDelay = sentenceDelay;

            // Update speed of animation with current settings
            inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageFadeSpeed;
            //inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageFadeSpeed;

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
        /// <param name="dialogueTree">The scriptable object that will be used to extract string and audioclip data for dialogue.</param>
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
                autoContinueDialogueRawImage.GetComponent<Animator>().speed = autoContinueDialogueImageScrollSpeed;
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

            if (debugComponent)
                Debug.Log("Start conversation with " + dialogueTree.characterName);

            // Set name text fields with the name of the person talking in the dialogueTree
            nameText.text = dialogueTree.characterName;
            nameVRText.text = dialogueTree.characterName;

            // Clear queue for new dialogue to be used.
            sentences.Clear();

            // Get each sentence and audio clip in the queues
            foreach (string sentence in dialogueTree.dialogueTreeElements)
            {
                sentences.Enqueue(sentence);
            }
            foreach (AudioClip clip in dialogueTree.dialogueTreeAudioClips)
            {
                sentenceAudioClips.Enqueue(clip);
            }

            // Display First Sentence of the queues
            DisplayNextSentence();
        }

        /// <summary>
        /// A method to play the next string and audioclip in the queues.
        /// </summary>
        public void DisplayNextSentence()
        {

            // Check to see if current sentence is typing first
            if (isTypeSentenceCoroutineRunning)
            {
                // Only used if input is required
                if (requireContinueButton)
                {
                    // Instant print the rest of the current sentence
                    if (instantPrintFinish)
                    {
                        StopAllCoroutines();                    // Stop coroutine that is currently printing.

                        dialogueText.text = currentSentence;
                        dialogueVRText.text = currentSentence;

                        isTypeSentenceCoroutineRunning = false; // Make sure this is false after sentence is done typing.
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

                    if (requireContinueButton)
                    {
                        inputContinueDialogueImage.gameObject.SetActive(true);
                        //inputContinueDialogueVRImage.gameObject.SetActive(true);

                        // Update speed of animation with current settings
                        inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageFadeSpeed;
                        //inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageFadeSpeed;
                    }
                }

                return;
            }

            // Reset delay times
            currentPrintLetterDelay = printLetterDelay;
            currentSentenceDelay = sentenceDelay;

            // End dialogue if queues are empty
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            // Adjust textDisplayWidth to fit more center with the camera screen.
            dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y);
            dialogueVRText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y);

            // Save sentence and audioclip that is being dequeued
            string sentence = sentences.Dequeue();
            AudioClip clip = sentenceAudioClips.Dequeue();

            if (debugComponent)
                Debug.Log(sentence);


            StopAllCoroutines();                            // Stop coroutine before starting new one.
            StartCoroutine(TypeSentence(sentence, clip));   // Display or type one character at a time.
        }

        /// <summary>
        /// A coroutine method that will type each sentence one character at a time at the set speed and play the audioclip if it is available.
        /// </summary>
        /// <param name="sentence">The current sentence that is out of the queue.</param>
        /// <param name="clip">The current audioclip that is out of the queue.</param>
        /// <returns></returns>
        private IEnumerator TypeSentence(string sentence, AudioClip clip)
        {
            isTypeSentenceCoroutineRunning = true;

            if (requireContinueButton)
            {
                inputContinueDialogueImage.gameObject.SetActive(false);
                //inputContinueDialogueVRImage.gameObject.SetActive(false);
            }

            currentSentence = sentence;

            audioSource.Stop();

            if (playWithAudio)
            {
                if (clip)
                    audioSource.PlayOneShot(clip, volume);
                else
                    Debug.LogError("No audioclip for string displayed! Please place audioclip in AudioClip List for respective string element.");
            }

            // Print full sentence or type each character individually.
            if (instantPrintBegin)
            {
                int punctutationCount = 0;

                foreach (char letter in sentence.ToCharArray())
                {
                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        punctutationCount++;    // Keep track of punctuation in each node
                    }
                }

                dialogueText.text = sentence;         // Display full sentence instantly
                dialogueVRText.text = sentence;         // Display full sentence instantly

                float fullSentenceDelay = (currentPrintLetterDelay * sentence.Length) + (punctutationCount * currentSentenceDelay) + currentSentenceDelay; // (CharacterCount from current dialogueTreeElement  * print delay time) + (number of punctuation characters * sentence delay time) + end of dialogueTreeElement delay time.

                if (debugComponent)
                {
                    Debug.Log("fullSentenceDelay: " + fullSentenceDelay);
                    Debug.Log("clip.length: " + clip.length);
                }

                // Play next sentence without button input
                if (!requireContinueButton)
                {
                    if (clip)
                        yield return new WaitForSeconds(clip.length);
                    else
                        yield return new WaitForSeconds(fullSentenceDelay);

                    isTypeSentenceCoroutineRunning = false;
                    
                    DisplayNextSentence();
                }
            }
            else
            {
                // Clear text fields before printing
                dialogueText.text = "";
                dialogueVRText.text = "";

                foreach (char letter in sentence.ToCharArray())
                {
                    dialogueText.text += letter;
                    dialogueVRText.text += letter;

                    // If character is any form of punctutation, then delay next sentence. Otherwise, print normally. 
                    if (letter == ',' || letter == ';' || letter == '.' || letter == '?' || letter == '!')
                    {
                        yield return new WaitForSeconds(currentSentenceDelay);      // Delay next sentence
                        //yield return null; // Wait a single frame/tick
                    }
                    else
                        yield return new WaitForSeconds(currentPrintLetterDelay);   // Delay character print
                }

                // If moving on with the next dialogue to type requires input, then
                if (!requireContinueButton)
                {
                    // If last character is not any form of punctutation, then delay next sentence
                    if (!(sentence.EndsWith(",") || sentence.EndsWith(";") || sentence.EndsWith(".") || sentence.EndsWith("?") || sentence.EndsWith("!")))
                    {
                        yield return new WaitForSeconds(currentSentenceDelay);
                    }

                    isTypeSentenceCoroutineRunning = false;

                    DisplayNextSentence();
                }
            }

            if (requireContinueButton)
            {
                inputContinueDialogueImage.gameObject.SetActive(true);
                //inputContinueDialogueVRImage.gameObject.SetActive(true);

                // Update speed of animation with current settings
                inputContinueDialogueImage.GetComponent<Animator>().speed = inputContinueDialogueImageFadeSpeed;
                //inputContinueDialogueVRImage.GetComponent<Animator>().speed = inputContinueDialogueImageFadeSpeed;
            }

            isTypeSentenceCoroutineRunning = false; // This ensures that you can check if the coroutine is done.
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

            autoContinueDialogueRawImage.gameObject.SetActive(false);
        }
    }
}