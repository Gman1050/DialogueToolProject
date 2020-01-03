using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTreeAsset", menuName = "ScriptableObjects/DialogueTreeScriptableObject", order = 1)]
public class DialogueTree : ScriptableObject
{
    [TextArea(10, 15)]
    public List<string> dialogueTreeElements = new List<string>();

    public DialogueTree nextDialogueTree;
}
