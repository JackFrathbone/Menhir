using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/New Melee Weapon")]
public class WeaponMeleeItem : Item
{
    [Header("Weapon Data")]
    public string weaponMeleeType;
    public bool twoHanded;

    public int weaponDamage;
    public int weapontToHitBonus;
    public int weaponDefence;
    //How long the windup to attack is, in seconds
    public float weaponSpeed;
    //Range in meter/unity units
    public float weaponRange;

    [Header("Weapon Visuals")]
    public Sprite weaponModel;

    [Header("Enchantments")]
    public List<Effect> enchantmentSelfEffects = new();
    public List<Effect> enchantmentTargetEffects = new();
}
