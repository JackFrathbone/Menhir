using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spells/New Spell")]
public class Spell : ScriptableObject
{
    [Header("Spell Data")]
    public string spellName;
    [TextArea(0, 3)]
    public string effectDescription;
    [Tooltip("If the spell should be placed in a seperate category in the spell list")]
    public bool isRecipe;
    public Sprite spellIcon;
    [Tooltip("If the spell effects are applied to the user, or become a projectile")]
    public bool castTarget;
    [Tooltip("If a self cast spell with an area effect ignores the player")]
    public bool effectSelf;
    [Tooltip("Only works if castTarget is true")]
    public GameObject projectilePrefab;
    [Tooltip("How much Mind ability a characters needs to equip this focus")]
    public int mindRequirement;
    [Tooltip("How long a slot takes to refill after being cast in seconds")]
    public int castingTime;
    [Tooltip("The radius the spell effects when cast")]
    [Range(0, 10)] public int spellArea = 0;

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

#if UNITY_EDITOR
    [InspectorButton("ClearID")]
    public bool clearID;

    public void ClearID()
    {
        uniqueID = "";
    }
#endif
}
