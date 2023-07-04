using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterManager : MonoBehaviour
{
    [Header("Settings")]
    public string characterName;

    [Header("Dialogue")]
    [TextArea(1, 6)]
    public string greeting;

    [Tooltip("Optional: If attached will run full dialogue, otherwill will just show box with greeting in it")]
    [SerializeField] DialogueGraph _attachedDialogueGraph;

    private void Awake()
    {
        if (_attachedDialogueGraph != null)
        {
            SetupDialogueComponent();
        }
    }

    private void OnValidate()
    {
        if (characterName != null || characterName != "")
        {
            name = characterName.Replace(" ", "_");
        }
    }

    //If there is a dialogue component attached then add the character sheet dialogue graph to it
    private void SetupDialogueComponent()
    {
        DialogueComponent dialogueComponent = gameObject.AddComponent<DialogueComponent>();

        dialogueComponent.CharacterName = characterName;
        dialogueComponent.CharacterGreeting = greeting;
        dialogueComponent.CharacterDescription = null;
        dialogueComponent.DialogueCharacterManager = null;
        dialogueComponent.AttachedDialogueGraph = _attachedDialogueGraph;
    }
}
