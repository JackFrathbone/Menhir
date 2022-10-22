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
    public int weaponRollAmount;
    //How long the windup to attack is, in seconds
    public float weaponSpeed;
}
