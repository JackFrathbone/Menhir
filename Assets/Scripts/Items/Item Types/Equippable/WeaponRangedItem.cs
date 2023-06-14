using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/New Ranged Weapon")]
public class WeaponRangedItem : Item
{
    [Header("Weapon Data")]
    public Sprite weaponModelLoaded;
    public Sprite weaponModelDrawing;
    public Sprite weaponModelFired;

    public GameObject projectilePrefab;

    public string weaponRangedType;

    public int weaponDamage;
    //How long the windup to attack is, in seconds
    public float weaponSpeed;

    [Header("Enchantments")]
    public List<Effect> enchantmentSelfEffects = new();
    public List<Effect> enchantmentTargetEffects = new();
}
