using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class ButtonHintSystemTest : MonoBehaviour
{
    public bool isTeleportHintOn = false;
    public bool isGripHintOn = false;
    public bool isMenuHintOn = false;
    public bool isPinchHintOn = false;
    public bool turnOffAllHints = false;

    public SteamVR_Action_Boolean teleportActionInput;
    public SteamVR_Action_Boolean gripActionInput;
    public SteamVR_Action_Boolean menuActionInput;
    public SteamVR_Action_Boolean isPinchActionInput;
    
    public Hand leftHand, rightHand;

    public string teleportHintString;
    public string gripHintString;
    public string menuHintString;
    public string pinchHintString;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTeleportHintOn)
        {
            ControllerButtonHints.ShowButtonHint(leftHand, teleportActionInput);
            ControllerButtonHints.ShowButtonHint(rightHand, teleportActionInput);
            ControllerButtonHints.ShowTextHint(leftHand, teleportActionInput, teleportHintString);
            ControllerButtonHints.ShowTextHint(rightHand, teleportActionInput, teleportHintString);

            isTeleportHintOn = false;
        }
        else if (isGripHintOn)
        {
            ControllerButtonHints.ShowButtonHint(leftHand, gripActionInput);
            ControllerButtonHints.ShowButtonHint(rightHand, gripActionInput);
            ControllerButtonHints.ShowTextHint(leftHand, gripActionInput, gripHintString);
            ControllerButtonHints.ShowTextHint(rightHand, gripActionInput, gripHintString);

            isGripHintOn = false;
        }
        else if (isMenuHintOn)
        {
            ControllerButtonHints.ShowButtonHint(leftHand, menuActionInput);
            ControllerButtonHints.ShowButtonHint(rightHand, menuActionInput);
            ControllerButtonHints.ShowTextHint(leftHand, menuActionInput, menuHintString);
            ControllerButtonHints.ShowTextHint(rightHand, menuActionInput, menuHintString);

            isMenuHintOn = false;
        }
        else if (isPinchHintOn)
        {
            ControllerButtonHints.ShowButtonHint(leftHand, isPinchActionInput);
            ControllerButtonHints.ShowButtonHint(rightHand, isPinchActionInput);
            ControllerButtonHints.ShowTextHint(leftHand, isPinchActionInput, pinchHintString);
            ControllerButtonHints.ShowTextHint(rightHand, isPinchActionInput, pinchHintString);

            isPinchHintOn = false;
        }

        if (turnOffAllHints)
        {
            ControllerButtonHints.HideAllButtonHints(leftHand);
            ControllerButtonHints.HideAllButtonHints(rightHand);
            ControllerButtonHints.HideAllTextHints(leftHand);
            ControllerButtonHints.HideAllTextHints(rightHand);

            turnOffAllHints = false;
        }
    }
}
