using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Ranged Weapon")]
public class WeaponRangedItem : Item
{
    [Header("Weapon Data")]
    public Sprite weaponModel;

    public string weaponRangedType;

    public int weaponDamage;
    public int weaponRollAmount;
    //How long the windup to attack is, in seconds
    public float weaponSpeed;
}
