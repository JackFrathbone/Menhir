using UnityEngine;

[System.Serializable]
public class JournalEntry
{
    [Header("Data")]
    [TextArea(1, 6)]
    public string journalText;
    [ReadOnly] public float timeStamp;
    [ReadOnly] public bool isArchived = false;
}
