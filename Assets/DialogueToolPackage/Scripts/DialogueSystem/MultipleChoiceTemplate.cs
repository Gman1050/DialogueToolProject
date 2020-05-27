using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class MultipleChoiceTemplate : MonoBehaviour
    {
        [Header("Template UI Elements:")]
        public Text questionText;
        public Button answerButtonPrefab;
        public Button submitButton;

        private string currentChoice = "";
        private DialogueTree.MultipleChoiceNode currentMultipleChoiceNode;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            questionText.text = "";
            submitButton.gameObject.SetActive(false);
        }

        /// <summary>
        /// A method to set the value of currentChoice as the set parameter from MultipleChoiceAnswer gameobject.
        /// </summary>
        /// <param name="choice">The string to be set as the current choice of the multiple choice question.</param>
        public void SetCurrentChoice(string choice) { currentChoice = choice; }

        /// <summary>
        /// A method that will set the layout of the particular multiple choice question.
        /// </summary>
        public void SetTemplate(DialogueTree.MultipleChoiceNode multipleChoiceNode)
        {
            currentMultipleChoiceNode = multipleChoiceNode;
            questionText.text = multipleChoiceNode.question;
            Vector3 newPosition = answerButtonPrefab.transform.position;

            for (int i = 0; i < currentMultipleChoiceNode.answers.Count; i++)
            {
                // Instantiate and set position
                Button answerButtonClone = Instantiate(answerButtonPrefab, answerButtonPrefab.transform.position, Quaternion.identity, transform);
                answerButtonClone.GetComponent<RectTransform>().anchoredPosition = newPosition;
                Debug.Log(answerButtonClone.GetComponent<RectTransform>().position);

                // Set Properties (answer text, onClick functionalities, etc.)
                answerButtonClone.GetComponent<MultipleChoiceAnswer>().SetAnswerData(i, currentMultipleChoiceNode.answers[i].answer);

                // Set next position
                float newPositionY = newPosition.y;
                newPosition = new Vector3(0, newPositionY - 85, 0);
            }

            submitButton.gameObject.SetActive(true);
        }

        /// <summary>
        /// A method that is used to submit the selected answer to the question and will play the dialogueTree associated with the answer.
        /// </summary>
        public void SubmitChoice()
        {
            // If a choice has not been made yet, then don't continue.
            if (currentChoice == "")
                return;

            // Play next dialogue
            for (int i = 0; i < currentMultipleChoiceNode.answers.Count; i++)
            {
                if (currentChoice == currentMultipleChoiceNode.answers[i].answer)
                {
                    DialogueManager.instance.StartDialogue(currentMultipleChoiceNode.answers[i].dialogueTreeResponse);
                    break;
                }
            }

            // Delete answerButtonClones generated in SetTemplate method
            foreach (Transform child in transform)
            {
                if (child.GetComponent<MultipleChoiceAnswer>())
                    Destroy(child.gameObject);
            }

            // Clear all visible fields in MultipleChoiceTemplate
            questionText.text = "";
            currentChoice = "";

            submitButton.gameObject.SetActive(false);
        }
    }
}
