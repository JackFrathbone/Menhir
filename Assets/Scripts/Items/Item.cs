using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Base Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    [Header("Item Data")]
    public string itemName;
    [TextArea(0, 3)]
    public string itemDescription;
    public float itemWeight;
    public Sprite itemIcon;

    [Header("Tracking")]
    [ReadOnly] public string uniqueID;

    public string GetUniqueID()
    {
        return uniqueID;
    }

#if UNITY_EDITOR
    [InspectorButton("ClearID")]
    public bool clearID;

    public void ClearID()
    {
        uniqueID = "";
    }
#endif
}
