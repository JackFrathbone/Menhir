using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Shield")]
public class ShieldItem : Item
{
    [Header("Shield Data")]
    //This is a percentage bonus that adds on top of the existing block chance
    public int shieldDefence;

    [Header("Shield Visuals")]
    public Sprite shieldModel;
}
