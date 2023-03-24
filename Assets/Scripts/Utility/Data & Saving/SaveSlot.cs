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

    //The scene that the save was in
    public string currentScene;
}
