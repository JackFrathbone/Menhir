using UnityEngine;

[System.Serializable]
public class QuestEntry
{
    [Header("Quest Entry Data")]
    [Tooltip("Name/short description of the entry for better legibility")]
    public string questEntryName;
    [Tooltip("Number used to identify order of quest entry, and to enable or disable it via external functions")]
    public int questEntryStage;
    [Tooltip("The description shown to the player"), TextArea(1,4)]
    public string questEntryJournalText;

    [Header("Quest Entry States")]
    [Tooltip("If this entry is visible to the player")]
    [ReadOnly] public bool questEntryActive = false;
    [Tooltip("Marks the quest as completed if this entry is activated")]
    public bool questEntryCompleteQuest = false;
}
