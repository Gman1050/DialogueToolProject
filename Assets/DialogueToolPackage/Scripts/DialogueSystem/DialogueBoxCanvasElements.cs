using System;
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

        void Update()
        {
            // If this canvas is in WorldSpace render mode, then set the x-value of the localScale to its negative value and always look at the camera tagged as MainCamera.
            if (GetComponent<Canvas>().renderMode == RenderMode.WorldSpace)
            {
                transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                transform.LookAt(Camera.main.transform.position);
                // May only want to rotate on the y-axis only.
            }
        }

        public void SetWorldSpaceCanvasPosition(Vector3 newPosition, bool isPlayerPosition = false)
        {
            // If this is a world space object and if this transform is a child, then set it to have no parent temporarily to have its position set properly in world space.
            if (GetComponent<Canvas>().renderMode == RenderMode.WorldSpace)
            {
                if (transform.parent)
                    transform.parent = null;
            }

            if (isPlayerPosition)
            {
                Camera playerCamera = Camera.main;
                GameObject tempChild = playerCamera.transform.GetChild(0).gameObject;   // Make sure that the child's z-value of its position is a small value...around 0.5f.

                float cameraTargetDistance = Vector3.Distance(playerCamera.transform.position, tempChild.transform.position);
                
                // Use camera position (A) and child of the camera (B) to find normalize position (C).
                Vector3 normalizedChildPos = tempChild.transform.position;
                normalizedChildPos.y = playerCamera.transform.position.y;
               
                float distanceOne = Vector3.Distance(playerCamera.transform.position, normalizedChildPos);

                // Calculate percentage of (total distance - distance A to C / total distance).
                float percentage = cameraTargetDistance / distanceOne;
                Debug.Log("percentage: " + percentage);

                // Set desired position and rotation.
                transform.position = Vector3.LerpUnclamped(playerCamera.transform.position, normalizedChildPos, percentage);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, playerCamera.transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else
                transform.position = newPosition;

            // Set the parent again to the DialogueManager instance
            transform.parent = DialogueManager.instance.transform;
        }
    }
}