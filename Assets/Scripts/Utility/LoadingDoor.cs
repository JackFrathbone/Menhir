using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingDoor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("The full name of the target scene to be loaded")] string _targetSceneName;
    [SerializeField, Tooltip("Name of the door the player should be loaded into")] string _targetPlayerSpawnName;

    public void ActivateLoadingDoor()
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(_targetSceneName);

        if (buildIndex == -1)
        {
            Debug.Log("Door level name is invalid");
            return;
        }

        SceneLoader.instance.LoadPlayerScene(buildIndex, _targetPlayerSpawnName, Vector3.zero, Vector3.zero);
    }
}
