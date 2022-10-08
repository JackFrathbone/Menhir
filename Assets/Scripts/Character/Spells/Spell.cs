using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/New Spell")]
public class Spell : ScriptableObject
{
    [Header("Spell Data")]
    public string spellName;
    [TextArea(0, 3)]
    public string effectDescription;
    public Sprite spellIcon;
    [Tooltip("If the spell effects are applied to the user, or become a projectile")]
    public bool castTarget;
    [Tooltip("Only works if castTarget is true")]
    public GameObject projectilePrefab;
    [Tooltip("How much Mind ability a characters needs to equip this focus")]
    public int mindRequirement;

    [Header("Spell Effects")]
    [Tooltip("Spell effects")]
    public List<Effect> spellEffects = new List<Effect>();
    [Tooltip("Items that are required to prepare the spell")]
    public List<Item> castingCostItems = new List<Item>();
}
