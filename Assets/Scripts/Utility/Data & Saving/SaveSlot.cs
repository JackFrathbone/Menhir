using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveSlot
{
    [Header("Data")]
    //Which save slot this data takes up
    public int saveSlotIndex;

    //List of all tracked scenes associated with this save
    public List<SceneData> savedScenes = new();

    //The saved player data
    public PlayerDataTracker playerData;
}
