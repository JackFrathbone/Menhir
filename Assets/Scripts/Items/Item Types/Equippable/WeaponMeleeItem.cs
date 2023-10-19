using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/New Melee Weapon")]
public class WeaponMeleeItem : Item
{
    [Header("Weapon Data")]
    [Tooltip("Description of weapon type, for inventory purposes")]
    public string weaponMeleeType;
    [Tooltip("If the weapon takes up both hand slots or not")]
    public bool twoHanded;

    [Tooltip("The damage dealt on a succesful hit")]
    public int weaponDamage;
    [Tooltip("How much to add to the chance to hit when attacking, in percent")]
    public int weapontToHitBonus;
    [Tooltip("How much defence to add the character when equipped, in percent")]
    public int weaponDefence;
    [Tooltip("The time it takes for the take to windup and hit, in seconds")]
    public float weaponSpeed;
    [Tooltip("Range of the attack, in meters")]
    public float weaponRange;
    [Tooltip("The amount of physics forces applied to the target on a succesful hit")]
    public float weaponKnockback;

    [Header("Weapon Visuals")]
    [Tooltip("The model used by the player and NPC animators")]
    public Sprite weaponModel;

    [Header("Enchantments")]
    [Tooltip("Enchantments applied on hit")]
    public List<Effect> enchantmentSelfEffects = new();
    [Tooltip("Enchantments applied on self")]
    public List<Effect> enchantmentTargetEffects = new();
}
