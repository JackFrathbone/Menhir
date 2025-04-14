using Udar.SceneField;
using UnityEngine;

public class LoadingDoor : MonoBehaviour
{
    //Loading doors move the player to a new scene

    [Header("Settings")]
    [SerializeField, Tooltip("If locked this door is unusable")] bool _isLocked;

    [SerializeField, Tooltip("The full name of the target scene to be loaded")] SceneField _targetScene;
    [SerializeField, Tooltip("Name of the door the player should be loaded into")] string _targetPlayerSpawnName;

    public bool GetLockedStatus()
    {
        return _isLocked;
    }

    public void ActivateLoadingDoor()
    {
        int buildIndex = _targetScene.BuildIndex;

        if (buildIndex == -1)
        {
            Debug.Log("Door level name is invalid");
            return;
        }

        SceneLoader.instance.LoadPlayerScene(_targetScene.BuildIndex, _targetPlayerSpawnName, Vector3.zero, Vector3.zero, true, true);
    }
}
