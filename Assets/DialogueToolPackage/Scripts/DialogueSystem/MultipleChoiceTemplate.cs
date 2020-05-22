using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class MultipleChoiceTemplate : MonoBehaviour
    {
        public Text questionText;
        public Button answerButtonPrefab;

        private string currentChoice;
        private DialogueTree.MultipleChoiceNode currentMultipleChoiceNode;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// A method that will set the layout of the particular multiple choice question.
        /// </summary>
        public void SetTemplate(DialogueTree.MultipleChoiceNode multipleChoiceNode)
        {
            currentMultipleChoiceNode = multipleChoiceNode;

            // Instantiate
            Button answerButtonClone = Instantiate(answerButtonPrefab, transform.position, Quaternion.identity);

            // Set Position

            // Set Properties (answer text, onClick functionalities, etc.)
            answerButtonClone.onClick.AddListener(() => SetChoice(answerButtonClone));
        }

        /// <summary>
        /// A method that sets the data for the selected choice during onClick event.
        /// </summary>
        /// <param name="choice">The choice chosen in the scene.</param>
        public void SetChoice(Button choice)
        {
            // Needs work
            foreach (Transform child in choice.transform)
            {
                if (child.name.Contains("Choice"))
                    currentChoice = child.GetComponent<Text>().text;
            }
        }

        public void SubmitChoice()
        {
            // Play next dialogue ... needs work
            switch (currentChoice)
            {
                case " A.":
                    DialogueManager.instance.StartDialogue(currentMultipleChoiceNode.answers[0].dialogueTreeResponse);
                    break;
                case " B.":
                    DialogueManager.instance.StartDialogue(currentMultipleChoiceNode.answers[1].dialogueTreeResponse);
                    break;
                case " C.":
                    DialogueManager.instance.StartDialogue(currentMultipleChoiceNode.answers[2].dialogueTreeResponse);
                    break;
                case " D.":
                    DialogueManager.instance.StartDialogue(currentMultipleChoiceNode.answers[3].dialogueTreeResponse);
                    break;
            }

            // Delete answerButtonClones generated in SetTemplate method
            foreach (Transform child in transform)
            {
                if (child.GetComponent<Button>())
                    Destroy(gameObject);
            }

            // Deactivate MultipleChoiceTemplate gameobject
            gameObject.SetActive(false);
        }
    }
}
