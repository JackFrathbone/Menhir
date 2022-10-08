using UnityEngine;

public enum EquipmentType
{
    armour,
    cape,
    feet,
    greaves,
    hands,
    helmet,
    pants,
    shirt
};

[CreateAssetMenu(menuName = "Items/Equipment")]
public class EquipmentItem : Item
{
    [Header("Equipment Data")]
    public EquipmentType equipmentType;
    public int equipmentDefence;

    [Header("Equipment Visuals")]
    public Sprite equipmentModel;
    //Leave as white if visual already has its own set colors
    public Color equipmentColor = Color.white;
}
