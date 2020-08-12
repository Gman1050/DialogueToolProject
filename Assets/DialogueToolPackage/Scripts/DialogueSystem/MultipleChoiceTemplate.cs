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

        [Header("Template UI Settings:")]
        [Range(80, 100)] public float answerButtonSpacing = 95;             // Values to set the spacing between answer buttons that will be spawned on the template.

        private string currentChoice = "";
        private DialogueTree.MultipleChoiceNode currentMultipleChoiceNode;
        private float originalBackgroundPanelHeight;
        private Rect desiredBackgroundSize;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            questionText.text = "";
            submitButton.gameObject.SetActive(false);
            desiredBackgroundSize = transform.parent.GetComponent<RectTransform>().rect;
            originalBackgroundPanelHeight = desiredBackgroundSize.height;
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
            float currentBackgroundPanelHeight = originalBackgroundPanelHeight;

            for (int i = 0; i < currentMultipleChoiceNode.answers.Count; i++)
            {
                // Instantiate and set position
                Button answerButtonClone = Instantiate(answerButtonPrefab, answerButtonPrefab.transform.position, new Quaternion(), transform);
                answerButtonClone.GetComponent<RectTransform>().anchoredPosition3D = newPosition;

                // Set Properties (answer text, choice text (A,B,C,D, ...), etc.)
                answerButtonClone.GetComponent<MultipleChoiceAnswer>().SetAnswerData(i, currentMultipleChoiceNode.answers[i].answer);

                // Set next position
                newPosition = new Vector3(newPosition.x, newPosition.y - answerButtonSpacing, newPosition.z);

                // Calculate room for backgroundPanelSize
                currentBackgroundPanelHeight += answerButtonSpacing;
            }

            // Set new backgroundPanelSize
            transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(desiredBackgroundSize.width, currentBackgroundPanelHeight - answerButtonSpacing);

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
                    DialogueManager.instance.StartDialogue(currentMultipleChoiceNode.answers[i].dialogueTreeResponse, DialogueManager.instance.currentDialogueSpeakerLocation);
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

            // Set original backgroundPanelSize
            transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(desiredBackgroundSize.width, originalBackgroundPanelHeight);
        }
    }
}
