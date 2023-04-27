using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skill System/New Skill")]
public class Skill : ScriptableObject
{
    [Header("Skill Data")]
    public string skillName;
    [TextArea(1, 3)]
    public string skillDescription;

    [Header("Skill Items")]
    [Tooltip("Items the skill will give to the character on gaining it")]
    public List<Item> skillItems;

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
