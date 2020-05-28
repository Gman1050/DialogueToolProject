using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.InteractionSystem;

/// <summary>
/// A class that allows objects and UI to be interacted with the index finger tips of each hand that SteamVR provides.
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class TouchDetection : MonoBehaviour
{
    [Header("UI AudioClips: ")]
    public AudioClip buttonHighlightSound;          // The audio clip for highlighting a UI button.
    public AudioClip buttonClickSound;              // The audio clip for clicking a UI button.

    [Header("Touch Detection Settings: ")]
    public ushort hapticFeedbackDuration = 1;       // The haptic feedback duration for pointer events.
    public bool requiresOpenIndexFinger = true;     // Boolean to see if raising your index finger is required to detect objects.
    public bool debugComponent = false;             // Boolean to check for Debug.Logs associated with this class.

    private Hand hand;                              // The Valve.VR.InteractionSystem.Hand component that is required for haptic feedback.
    private Button lastButtonPressed;               // Remembers the last button pressed to disable green highlight press color if collider for finger turns off. 

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        // Check to see if the parent of this gameobject has the Valve.VR.InteractionSystem.Hand component attached to it.
        if (transform.parent.gameObject.GetComponent<Hand>())
        {
            hand = transform.parent.gameObject.GetComponent<Hand>();    // Get the reference for the Valve.VR.InteractionSystem.Hand component.
        }
        else
        {
            Debug.LogError(transform.parent.name + " does not have Valve.VR.InteractionSystem.Hand component attached to the gameobject. Please provide the required component on the gameobject.");
        } 
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        // Check if having your index finger streched is a requirement for touch detection.
        if (requiresOpenIndexFinger)
        {
            if (SteamVR_Input.GetStateDown("TouchIndex", SteamVR_Input_Sources.LeftHand) && hand.name == "LeftHand")
            {
                GetComponent<SphereCollider>().enabled = false;
            }
            else if (SteamVR_Input.GetStateUp("TouchIndex", SteamVR_Input_Sources.LeftHand) && hand.name == "LeftHand")
            {
                GetComponent<SphereCollider>().enabled = true;
            }

            if (SteamVR_Input.GetStateDown("TouchIndex", SteamVR_Input_Sources.RightHand) && hand.name == "RightHand")
            {
                GetComponent<SphereCollider>().enabled = false;
            }
            else if (SteamVR_Input.GetStateUp("TouchIndex", SteamVR_Input_Sources.RightHand) && hand.name == "RightHand")
            {
                GetComponent<SphereCollider>().enabled = true;
            }
        }

        // Check if the last button pressed is still highlighted after resting index finger on the controller (bug fix)
        if (!GetComponent<SphereCollider>().enabled)
        {
            if (lastButtonPressed)
            {
                if (lastButtonPressed.GetComponent<Image>().color != lastButtonPressed.colors.normalColor)
                {
                    lastButtonPressed.GetComponent<Image>().color = lastButtonPressed.colors.normalColor;
                }
            }
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        // Check to see if this gameobject has the UnityEngine.UI.Button component attached to it.
        if (other.GetComponent<Button>())
        {
            // Intialize a button and image objects for reference.
            Button button = other.GetComponent<Button>();
            Image image = other.GetComponent<Image>();
            lastButtonPressed = button;

            // Provide feedback by changing color of the button pressed and controller rumble for the respect hand touching the button.
            image.color = button.colors.pressedColor;
            hand.TriggerHapticPulse(hapticFeedbackDuration);

            if (buttonHighlightSound)
                AudioManager.instance.PlayUserInterfaceSound(buttonHighlightSound);

            if (debugComponent)
                Debug.Log(transform.parent.name + " is touching a button.");
        }
        else
        {
            if (debugComponent)
                Debug.Log(transform.parent.name + " is touching an object that is not a button.");
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Button>())
        {
            // Intialize a button and image objects for reference.
            Button button = other.GetComponent<Button>();
            Image image = other.GetComponent<Image>();
            
            // Provide feedback by changing color of the button pressed and controller rumble for the respect hand touching the button.
            button.onClick.Invoke();
            image.color = button.colors.normalColor;

            if (buttonClickSound)
                AudioManager.instance.PlayUserInterfaceSound(buttonClickSound);

            if (debugComponent)
                Debug.Log(transform.parent.name + " touched a button.");
        }
        else
        {
            if (debugComponent)
                Debug.Log(transform.parent.name + " touched an object that is not a button.");
        }
    }
}
