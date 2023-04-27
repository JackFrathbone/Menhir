using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New StateCheck")]
public class StateCheck : ScriptableObject
{
    //Just used to keep track of what this state check is for
    [TextArea(3,15)]
    public string checkDescription;

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
