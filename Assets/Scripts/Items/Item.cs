using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Items/New Base Item")]
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
