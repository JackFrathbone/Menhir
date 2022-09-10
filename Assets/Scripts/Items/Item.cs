using UnityEngine;

[CreateAssetMenu(menuName = "Items/Base Item")]
public class Item : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    [TextArea (0,3)]
    public string itemDescription;
    public int itemValue;
    public float itemWeight;
    public Sprite itemIcon;
}
