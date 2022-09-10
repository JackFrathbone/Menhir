using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/Character Sheet")]
public class CharacterSheet : ScriptableObject
{
    [Header("Character Bio")]
    public string characterName;
    [TextArea(3,5)]
    public string characterDescription;
    public Faction characterFaction;
    public Aggression characterAggression;

    [Header("Inventory & Spells")]
    public List<Item> characterInventory = new List<Item>();
    public List<Spell> characterSpellbook = new List<Spell>();

    [Header("Dialogue")]
    public string characterGreeting;
    public List<Dialogue> characterDialogueTopics;

    [Header("Ability Scores")]
    public Abilities abilities;

    [Header("Visuals")]
    public bool randomiseVisuals;
    public Color characterSkintone;
    public Color characterHairColor;
    public Sprite characterHair;
    public Sprite characterBeard;
}
