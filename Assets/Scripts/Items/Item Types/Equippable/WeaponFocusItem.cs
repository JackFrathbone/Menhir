using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Magic Focus Weapon")]
public class WeaponFocusItem : Item
{
    [Header("Weapon Data")]
    public Sprite focusModel;
    public GameObject projectilePrefab;

    [Tooltip("How much Mind ability a characters needs to equip this focus")]
    public int mindRequirement;

    public string effectDescription;
    //list of effects
    public List<Effect> focusEffects = new List<Effect>();

    //How long the windup to attack is, in seconds
    public float castingSpeed;
}
