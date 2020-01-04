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

    [Header("Dialogue Settings:")]
    [Range(0, 0.1f)] public float printLetterDelay = 0.1f;
    public bool instantPrint = false;
    public bool useOpenCloseAnimation = false;

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
        dialogueCanvas.SetActive(true);

        if (useOpenCloseAnimation)
        {
            dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", true);
        }

        //Debug.Log("Start conversation with " + dialogueTree.characterName);
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
        //Debug.Log(sentence);

        if (instantPrint)
        {
            dialogueText.text = sentence;         // Display full sentence instantly
        }
        else
        {
            StopAllCoroutines();                    // Stop coroutine before starting new one.
            StartCoroutine(TypeSentence(sentence)); // Display or type one character at a time.
        }
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;

            yield return new WaitForSeconds(printLetterDelay);
            //yield return null; // Wait a single frame/tick
        }
    }

    private void EndDialogue()
    {
        Debug.Log("End of conversation.");

        if (useOpenCloseAnimation)
        {
            dialogueCanvas.GetComponent<Animator>().SetBool("isOpen", false);

        }
        else
            dialogueCanvas.SetActive(false);
    }
}
