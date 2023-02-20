using System.Collections.Generic;
using UnityEngine;

public enum CharacterPronouns
{
    He,
    She,
    They
}

[CreateAssetMenu(menuName = "Characters/New Character Sheet")]
public class CharacterSheet : ScriptableObject
{
    [Header("Character Bio")]
    public string characterName;
    [Tooltip("Developer description to keep track"), TextArea(3, 5)]
    public string characterDescription;
    public CharacterPronouns characterPronouns;
    public Faction characterFaction;
    [Tooltip("Passive prevents all combat, neutral checks faction relations before deciding on combat, aggresive always starts combat")]
    public Aggression characterAggression;

    [Header("Inventory & Spells")]
    public List<Item> characterInventory = new();
    public List<Spell> characterSpells = new();

    [Header("Dialogue")]
    [Tooltip("The generic greeting on starting dialogue")]
    public string characterGreeting;
    [Tooltip("The generic greeting on starting dialogue while character is wounded")]
    public string characterWoundedGreeting;

    [Tooltip("All the dialogue topics this character has")]
    public DialogueGraph characterDialogueGraph;

    [Header("Ability Scores")]
    public Abilities abilities;

    [Header("Skills")]
    public List<Skill> skills;

    [Header("World States")]
    [Tooltip("Will make the character start disabled in the world")]
    public bool startHidden;

    [Header("Visuals")]
    [Tooltip("Will randomly choose character looks")]
    public bool randomiseVisuals;
    public Color characterSkintone = Color.white;
    public Color characterHairColor = Color.black;
    public Sprite characterHair;
    public Sprite characterBeard;
}
