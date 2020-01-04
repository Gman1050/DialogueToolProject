using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue Canvas Elements:")]
    public GameObject dialogueCanvas;   // Get the BackgroundPanel gameobject from DialogueCanvas
    public Text nameText;
    public Text dialogueText;

    [Header("Dialogue Print Settings:")]
    [Range(0, 0.1f)] public float printLetterDelay = 0.1f;
    public bool instantPrint = false;

    [Header("Dialogue Input Settings:")]
    public bool requireContinueButton = false;

    [Header("Dialogue Delay Settings:")]
    [Range(0.25f, 2.0f)] public float sentenceDelay = 1.0f;
    public bool delaySentences = false;

    [Header("Dialogue Animation Settings:")]
    public bool useOpenCloseAnimation = false;

    [Header("Debug Settings:")]
    public bool debugComponent = false;

    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue(DialogueTree dialogueTree)
    {
        if (useOpenCloseAnimation)
        {
            dialogueCanvas.GetComponent<Animator>().enabled = true;
            dialogueCanvas.GetComponent<Animator>().SetBool("canTransition", true);
            dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", true);
        }
        else
        {
            //dialogueCanvas.SetActive(true);
            dialogueCanvas.GetComponent<Animator>().enabled = false;
            dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }

        if (debugComponent)
            Debug.Log("Start conversation with " + dialogueTree.characterName);

        nameText.text = dialogueTree.characterName;

        sentences.Clear();

        foreach(string sentence in dialogueTree.dialogueTreeElements)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        if (debugComponent)
            Debug.Log(sentence);

        StopAllCoroutines();                    // Stop coroutine before starting new one.
        StartCoroutine(TypeSentence(sentence)); // Display or type one character at a time.
    }

    private IEnumerator TypeSentence(string sentence)
    {
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

            foreach(char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;

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

    private IEnumerator DelayNextSentence()
    {
        yield return new WaitForSeconds(sentenceDelay);
        DisplayNextSentence();
    }

    private void EndDialogue()
    {
        if (debugComponent)
            Debug.Log("End of conversation.");

        if (useOpenCloseAnimation)
        {
            dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", false);

        }
        else
        {
            //dialogueCanvas.SetActive(false);
            dialogueCanvas.GetComponent<RectTransform>().localScale = new Vector3(1, 0, 1);
        }
    }
}
