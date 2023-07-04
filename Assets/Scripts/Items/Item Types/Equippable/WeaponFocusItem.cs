using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/New Magic Focus Weapon")]
public class WeaponFocusItem : Item
{
    [Header("Weapon Data")]
    public Sprite focusModel;
    [Tooltip("The projectile launched when cast, can be null to disable")]
    public GameObject projectilePrefab;

    [Tooltip("How much Mind ability a characters needs to equip this focus")]
    public int mindRequirement;

    [Tooltip("How long the windup to attack is, in seconds")]
    public float castingSpeed;

    [Tooltip("The effects added the projectile")]
    public List<Effect> focusEffects = new();

    [Header("Enchantments")]
    public List<Effect> enchantmentEffects = new();

    public string GetEffectsDescription()
    {
        string description = "Casts: ";

        foreach(Effect effect in focusEffects)
        {
            description += effect.GetDescription() + "<br>";
        }

        description += "<br>Enchantments: ";

        foreach (Effect effect in enchantmentEffects)
        {
            description += effect.GetDescription() + "<br>";
        }

        return description;
    }
}
