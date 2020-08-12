using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class MultipleChoiceAnswer : MonoBehaviour
    {
        [Header("Text fields for answer button data:")]
        public Text choiceText;
        public Text answerText;


        /// <summary>
        /// A method used to set the string data for the choice option and answer details.
        /// </summary>
        /// <param name="choiceOrder">Enter integer and will be added to starting ASCII value and converted to char.</param>
        /// <param name="answerString">The string that will be displayed in the button.</param>
        public void SetAnswerData(int choiceOrder, string answerString)
        {
            //ASCII characters start from 65 and end at 90
            int i = 65;  //int j = 90;

            // Convert the int to a char to get the actual character behind the ASCII code and set choiceText
            choiceText.text = ((char)(i + choiceOrder)).ToString() + ".";

            // Set answerText
            answerText.text = answerString;
        }

        /// <summary>
        /// A method that sets the data for the selected choice during onClick event.
        /// </summary>
        /// <param name="choice">The choice chosen in the scene.</param>
        public void SetChoice(Button choice)
        {
            MultipleChoiceTemplate multipleChoiceTemplate = transform.parent.GetComponent<MultipleChoiceTemplate>();
            multipleChoiceTemplate.SetCurrentChoice(choice.GetComponent<MultipleChoiceAnswer>().answerText.text);

            // Color selected green and color deselected white for VR Laser Pointer and TouchDetection.
            if (multipleChoiceTemplate.transform.parent.parent.name == "DialogueBoxVRCanvas" || multipleChoiceTemplate.transform.parent.parent.name == "DialogueBoxWorldSpaceCanvas")
            {
                // Find all children including selected button to turn to normalColor for button
                foreach (Transform child in multipleChoiceTemplate.transform)
                {
                    child.GetComponent<Image>().color = child.GetComponent<Button>().colors.normalColor;
                }

                // Turn selected button to selected color 
                choice.GetComponent<Image>().color = choice.colors.selectedColor;
            }
        }
    }
}