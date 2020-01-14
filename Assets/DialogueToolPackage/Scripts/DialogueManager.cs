using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [Header("Dialogue Canvas Elements:")]
    public GameObject dialogueCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
    public Text nameText;
    public Text dialogueText;

    [Header("Dialogue VR Canvas Elements:")]
    public GameObject dialogueVRCanvas;   // Get the BackgroundPanel gameobject from DialogueBoxVRCanvas
    public Text nameVRText;
    public Text dialogueVRText;

    [Header("Dialogue Print Settings:")]
    [Range(650, 1800)] public float textDisplayWidth = 800.0f;
    [Range(0, 0.1f)] public float printLetterDelay = 0.1f;
    public bool instantPrint = false;
    public bool printDialogue = true;

    [Header("Dialogue Input Settings:")]
    public bool requireContinueButton = false;

    [Header("Dialogue Delay Settings:")]
    [Range(0.25f, 2.0f)] public float sentenceDelay = 1.0f;

    [Header("Dialogue Animation Settings:")]
    public bool useOpenCloseAnimation = false;

    [Header("Dialogue Audio Settings:")]
    [Range(0, 1)] public float volume = 1.0f;
    public bool playWithAudio = true;
    private AudioSource audioSource;

    [Header("Dialogue Test Settings:")]
    public bool playAtStart = false;
    public DialogueTree dialogueTreeTest;

    [Header("Debug Settings:")]
    public bool debugComponent = false;

    // Dialogue Queues
    private Queue<string> sentences;
    private Queue<AudioClip> sentenceAudioClips;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        sentences = new Queue<string>();
        sentenceAudioClips = new Queue<AudioClip>();

        if (playAtStart)
        {
            StartDialogue(dialogueTreeTest);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!printDialogue)
            requireContinueButton = false;
    }

    public void StartDialogue(DialogueTree dialogueTree)
    {
        // 1
        if (!printDialogue && !playWithAudio)
        {
            Debug.LogError("Cannot play dialogue! The printDialogue and playWithAudio booleans are false. Mark at least one of these as true in the inspector to start the dialogue.");
            return;
        }

        // 2
        if (printDialogue)
        {
            // 2a
            if (useOpenCloseAnimation)
            {
                dialogueCanvas.GetComponent<Animator>().enabled = true;
                dialogueCanvas.GetComponent<Animator>().SetBool("canTransition", true);
                dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", true);

                dialogueVRCanvas.GetComponent<Animator>().enabled = true;
                dialogueVRCanvas.GetComponent<Animator>().SetBool("canTransition", true);
                dialogueVRCanvas.GetComponent<Animator>().SetBool("isOpen", true);
            }

            // 2b
            else
            {
                //dialogueCanvas.SetActive(true);
                dialogueCanvas.GetComponent<Animator>().enabled = false;
                dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

                dialogueVRCanvas.GetComponent<Animator>().enabled = false;
                dialogueVRCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
        }

        // 3
        if (debugComponent)
            Debug.Log("Start conversation with " + dialogueTree.characterName);

        // 4
        nameText.text = dialogueTree.characterName;

        // 5
        nameVRText.text = dialogueTree.characterName;

        // 6
        sentences.Clear();

        // 7
        foreach (string sentence in dialogueTree.dialogueTreeElements)
        {
            sentences.Enqueue(sentence);
        }

        // 8
        foreach (AudioClip clip in dialogueTree.dialogueTreeAudioClips)
        {
            sentenceAudioClips.Enqueue(clip);
        }

        // 9
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        dialogueText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueText.GetComponent<RectTransform>().sizeDelta.y);
        dialogueVRText.GetComponent<RectTransform>().sizeDelta = new Vector2(textDisplayWidth, dialogueVRText.GetComponent<RectTransform>().sizeDelta.y);

        string sentence = sentences.Dequeue();
        AudioClip clip = sentenceAudioClips.Dequeue();

        if (debugComponent)
            Debug.Log(sentence);

        StopAllCoroutines();                    // Stop coroutine before starting new one.
        StartCoroutine(TypeSentence(sentence, clip)); // Display or type one character at a time.
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

            foreach(char letter in sentence.ToCharArray())
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
                DisplayNextSentence();
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

                DisplayNextSentence();
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
}
