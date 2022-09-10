using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(2, 3)]
    public string dialogueTopic;
    [TextArea(3, 5)]
    public List<string> dialogueLines = new List<string>();

    [Header("Behaviour")]
    public bool dialogueRunOnce;
    //Use this to set unique dialogue for when a character is wounded ie interrogation
    public bool dialogueRunOnCharacterWounded;

    [Header("Checks")]
    //Check the global list to see if a check is active
    public List<StateCheck> dialogueStateChecks = new List<StateCheck>();
    //Check players inventory for specific item
    public List<Item> dialogueRequiredItems = new List<Item>();
    //Checks ability
    public Abilities dialogueAbilityChecks;

    [Header("Dialogue Outcomes")]
    //Add a check to the global list
    public List<StateCheck> dialogueAddStateCheck = new List<StateCheck>();

    [Header("Sub Topics")]
    public List<Dialogue> dialogueSubTopics = new List<Dialogue>();
}
