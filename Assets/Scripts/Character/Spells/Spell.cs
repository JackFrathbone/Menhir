using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/New Spell")]
public class Spell : ScriptableObject
{
    [Header("Spell Data")]
    public string spellName;
    [Tooltip("If the spell should be placed in a seperate category in the spell list")]
    public bool isRecipe;
    public Sprite spellIcon;

    [Tooltip("Only works if castTarget is true")]
    public GameObject projectilePrefab;

    [Header("Spell Settings")]
    [Tooltip("How much Mind ability a characters needs to equip this focus")]
    public int mindRequirement;

    [Tooltip("If the spell effects are applied to the user, or become a projectile")]
    public bool castTarget;
    [Tooltip("If a self cast spell with an area effect ignores the player")]
    public bool effectSelf;

    [Tooltip("The radius the spell effects when cast")]
    [Range(0, 10)] public int spellArea = 0;

    [Tooltip("How long a slot takes to refill after being cast in seconds")]
    public float cooldown;

    [Header("Spell Effects")]
    [Tooltip("Spell effects")]
    public List<Effect> spellEffects = new();
    [Tooltip("Items that are required to prepare the spell")]
    public List<Item> castingCostItems = new();

    [Header("Tracking")]
    [ReadOnly] public string uniqueID;

    public string GetUniqueID()
    {
        return uniqueID;
    }

    public string GetEffectsDescription()
    {
        string description = "Casts: ";

        foreach (Effect effect in spellEffects)
        {
            description += effect.GetDescription() + "<br>";
        }

        return description;
    }

#if UNITY_EDITOR
    [InspectorButton("ClearID")]
    public bool clearID;

    public void ClearID()
    {
        uniqueID = "";
    }
#endif
}
