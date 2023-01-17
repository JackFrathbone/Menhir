using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest System/New Quest")]
public class Quest : ScriptableObject
{
    [Header("Quest Data")]
    [TextArea(0, 1)]
    public string questName;
    [Tooltip("All the entries that make up the quest")]
    public List<QuestEntry> questEntries;

    [Header("Quest States")]
    [Tooltip("If the quest has been finished and should not show up as active in the journal")]
    [ReadOnly] public bool questCompleted = false;
}
