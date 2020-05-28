using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.Extras;

/// <summary>
/// A class that handles the SteamVR_LaserPointer events for each hand.
/// </summary>
public class LaserSystem : MonoBehaviour
{
    [Header("SteamVR Laser Pointer References: ")]
    public SteamVR_LaserPointer leftLaserPointer;   // SteamVR_LaserPointer object for left hand.
    public SteamVR_LaserPointer rightLaserPointer;  // SteamVR_LaserPointer object for right hand.
    public Hand leftHand;                           // Hand objects for left and.
    public Hand rightHand;                          // Hand objects for right hand.
    public bool startInRightHand = true;            // Mark true if you want the laser pointer to be toggled to the right hand at the start. Otherwise, false for the left hand.
    private bool currentlyInRightHand = false;      // Boolean to confirm which hand the laser pointer is in.

    [Header("UI AudioClips: ")]
    public AudioClip buttonHighlightSound;          // The audio clip for highlighting a UI button.
    public AudioClip buttonClickSound;              // The audio clip for clicking a UI button.

    [Header("Laser System Settings: ")]
    public ushort hapticFeedbackDuration = 1;       // The haptic feedback duration for pointer events.
    public bool debugComponent = false;             // Boolean to check for Debug.Logs associated with this class.
    private bool canUseLaserPointer = false;        // Boolean to confirm that the user can use and toggle the laser pointer between each hand.

    /// <summary>
    /// A method that executes when the instance of the class is created.
    /// </summary>
    void Awake()
    {
        // Initialize left hand pointer events
        leftLaserPointer.PointerIn += LeftPointerInside;
        leftLaserPointer.PointerOut += LeftPointerOutside;
        leftLaserPointer.PointerClick += LeftPointerClick;

        // Initialize right hand pointer events
        rightLaserPointer.PointerIn += RightPointerInside;
        rightLaserPointer.PointerOut += RightPointerOutside;
        rightLaserPointer.PointerClick += RightPointerClick;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        if (startInRightHand)
        {
            currentlyInRightHand = true;
        }
        else
        {
            currentlyInRightHand = false;
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        LaserPointerHandToggle();
    }

    /// <summary>
    /// A method for detecting the object that the left laser pointer is colliding with and clicked on.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void LeftPointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>())
        {
            if (debugComponent)
                Debug.Log("Button was clicked with Left Pointer!");

            // Initialize the button and image properties of the UI element
            Button button = e.target.GetComponent<Button>();
            Image image = e.target.GetComponent<Image>();

            if (button.interactable)
            {
                // Activate button behaviors when it is clicked
                button.onClick.Invoke();
                image.color = button.colors.pressedColor;

                // Include clicked sound in the OnClick() event (optional)
                // May require calling AudioSource or AudioManager that will play and sound
                if (buttonClickSound)
                    AudioManager.instance.PlayUserInterfaceSound(buttonClickSound);
            }
        }
    }

    /// <summary>
    /// A method for detecting the object that the left laser pointer has collided with.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void LeftPointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>())
        {
            if (debugComponent)
                Debug.Log("Button was entered with Left Pointer!");

            // Initialize the button and image properties of the UI element
            Button button = e.target.GetComponent<Button>();
            Image image = e.target.GetComponent<Image>();

            if (button.interactable)
            {
                // Activate button behaviors similar to when PointerEnters
                image.color = button.colors.highlightedColor;
                leftHand.TriggerHapticPulse(hapticFeedbackDuration);

                // May require calling AudioSource or AudioManager that will play and sound
                if (buttonHighlightSound)
                    AudioManager.instance.PlayUserInterfaceSound(buttonHighlightSound);
            }
        }
    }

    /// <summary>
    /// A method for detecting the object that the left laser pointer has left collision with.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void LeftPointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>())
        {
            if (debugComponent)
                Debug.Log("Button was exited with Left Pointer!");

            // Initialize the button and image properties of the UI element
            Button button = e.target.GetComponent<Button>();
            Image image = e.target.GetComponent<Image>();

            if (button.interactable)
            {
                // Activate button behaviors similar to when PointerExits
                image.color = button.colors.normalColor;
                leftHand.TriggerHapticPulse(hapticFeedbackDuration);

                // May require calling AudioSource or AudioManager that will play and sound
            }
        }
    }

    /// <summary>
    /// A method for detecting the object that the right laser pointer is colliding with and clicked on.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void RightPointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>())
        {
            if (debugComponent)
                Debug.Log("Button was clicked with Right Pointer!");

            // Initialize the button and image properties of the UI element
            Button button = e.target.GetComponent<Button>();
            Image image = e.target.GetComponent<Image>();

            if (button.interactable)
            {
                // Activate button behaviors when it is clicked
                button.onClick.Invoke();
                image.color = button.colors.pressedColor;

                // Include clicked sound in the OnClick() event (optional)
                // May require calling AudioSource or AudioManager that will play and sound
                if (buttonClickSound)
                    AudioManager.instance.PlayUserInterfaceSound(buttonClickSound);
            }
        }
    }

    /// <summary>
    /// A method for detecting the object that the right laser pointer has collided with.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void RightPointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>())
        {
            if (debugComponent)
                Debug.Log("Button was entered with Right Pointer!");

            // Initialize the button and image properties of the UI element
            Button button = e.target.GetComponent<Button>();
            Image image = e.target.GetComponent<Image>();

            if (button.interactable)
            {
                // Activate button behaviors similar to when PointerEnters
                image.color = button.colors.highlightedColor;
                rightHand.TriggerHapticPulse(hapticFeedbackDuration);

                // May require calling AudioSource or AudioManager that will play and sound
                if (buttonHighlightSound)
                    AudioManager.instance.PlayUserInterfaceSound(buttonHighlightSound);
            }
        }
    }

    /// <summary>
    /// A method for detecting the object that the right laser pointer has left collision with.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void RightPointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.GetComponent<Button>())
        {
            if (debugComponent)
                Debug.Log("Button was exited with Right Pointer!");

            // Initialize the button and image properties of the UI element
            Button button = e.target.GetComponent<Button>();
            Image image = e.target.GetComponent<Image>();

            if (button.interactable)
            {
                // Activate button behaviors similar to when PointerExits
                image.color = button.colors.normalColor;
                rightHand.TriggerHapticPulse(hapticFeedbackDuration);

                // May require calling AudioSource or AudioManager that will play and sound
            }
        }
    }

    /// <summary>
    /// A method to toggle the hand the user wants the laser pointer to be in.
    /// Requires using the SteamVR InteractUI default input to call.
    /// </summary>
    private void LaserPointerHandToggle()
    {
        if (canUseLaserPointer)
        {
            if (SteamVR_Input.GetStateDown("InteractUI", SteamVR_Input_Sources.LeftHand))
            {
                currentlyInRightHand = false;
                leftLaserPointer.enabled = true;
                rightLaserPointer.enabled = false;

                // SteamVR update requires turning on/off laser pointer renderes this way.
                leftHand.transform.Find("New Game Object").gameObject.SetActive(true);
                rightHand.transform.Find("New Game Object").gameObject.SetActive(false);
            }
            else if (SteamVR_Input.GetStateDown("InteractUI", SteamVR_Input_Sources.RightHand))
            {
                currentlyInRightHand = true;
                rightLaserPointer.enabled = true;
                leftLaserPointer.enabled = false;

                // SteamVR update requires turning on/off laser pointer renderes this way.
                leftHand.transform.Find("New Game Object").gameObject.SetActive(false);
                rightHand.transform.Find("New Game Object").gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// A method to turn on the laser pointers.
    /// </summary>
    public void TurnOnLaserPointers()
    {
        canUseLaserPointer = true;

        if (currentlyInRightHand)
        {
            rightLaserPointer.enabled = true;
            leftLaserPointer.enabled = false;

            // SteamVR update requires turning on/off laser pointer renderes this way.
            leftHand.transform.Find("New Game Object").gameObject.SetActive(false);
            rightHand.transform.Find("New Game Object").gameObject.SetActive(true);
        }
        else
        {
            leftLaserPointer.enabled = true;
            rightLaserPointer.enabled = false;

            // SteamVR update requires turning on/off laser pointer renderes this way.
            leftHand.transform.Find("New Game Object").gameObject.SetActive(true);
            rightHand.transform.Find("New Game Object").gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// A method to turn off the laser pointers.
    /// </summary>
    public void TurnOffLaserPointers()
    {
        canUseLaserPointer = false;

        leftLaserPointer.enabled = false;
        rightLaserPointer.enabled = false;

        // SteamVR update requires turning on/off laser pointer renderes this way.
        leftHand.transform.Find("New Game Object").gameObject.SetActive(false);
        rightHand.transform.Find("New Game Object").gameObject.SetActive(false);
    }
}
