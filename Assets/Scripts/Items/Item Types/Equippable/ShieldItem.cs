using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/New Shield Item")]
public class ShieldItem : Item
{
    [Header("Shield Data")]
    //This is a percentage bonus that adds on top of the existing block chance
    public int shieldDefence;

    [Header("Shield Visuals")]
    public Sprite shieldModel;
}
