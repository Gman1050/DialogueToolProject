using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    /// <summary>
    /// A class that holds the references to each used element in the canvas.
    /// </summary>
    public class DialogueBoxCanvasElements : MonoBehaviour
    {
        [Header("Dialogue Canvas Elements:")]
        public GameObject backgroundPanel;   // Get the BackgroundPanel gameobject from DialogueBoxCanvas
        public Text nameText;
        public Text dialogueText;
        public RawImage autoContinueDialogueRawImage;
        public Image inputContinueDialogueImage;
        public GameObject multipleChoiceTemplate;

        [Header("Test Buttons:")]
        public GameObject testButtons;
    }
}