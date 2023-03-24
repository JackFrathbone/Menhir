using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SceneData
{
    [Header("Data")]
    public string trackedSceneName;

    //List of the sub tracking types in this scene
    [Header("Tracked Data")]
    public List<CharacterDataTracker> characterDataTrackers = new();
    public List<ContainerDataTracker> containerDataTrackers = new();
}
