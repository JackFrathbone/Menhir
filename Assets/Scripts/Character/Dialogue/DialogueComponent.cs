using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueComponent : MonoBehaviour
{
    public string CharacterName { get; set; }
    public string CharacterGreeting { get; set; }
    public string CharacterDescription { get; set; }

    public CharacterManager DialogueCharacterManager { get; set; }

    public DialogueGraph AttachedDialogueGraph { get; set; }
}
