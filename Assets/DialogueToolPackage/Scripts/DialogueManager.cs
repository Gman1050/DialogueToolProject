using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public Text nameText;
    public Text dialogueText;

    private Animator animator;
    private Queue<string> sentences;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        sentences = new Queue<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDialogue(DialogueTree dialogueTree)
    {
        //animator.SetBool("isOpen", true);

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
        //dialogueText.text = sentence;         // Display full sentence instantly
        StopAllCoroutines();                    // Stop coroutine before starting new one.
        StartCoroutine(TypeSentence(sentence)); // Display or type one character at a time.
    }

    private IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null; // Wait a single frame/tick
        }
    }
    private void EndDialogue()
    {
        Debug.Log("End of conversation.");
        //animator.SetBool("isOpen", false);
    }
}
