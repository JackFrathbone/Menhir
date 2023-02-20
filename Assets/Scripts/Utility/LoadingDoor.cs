using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingDoor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Tooltip("The full name of the target scene to be loaded")] string _targetSceneName;
    [SerializeField, Tooltip("Name of the door the player should be loaded into")] string _targetPlayerSpawnName;

    [Header("References")]
    private SceneLoader sceneLoader;
    private PlayerSceneSpawner playerSceneSpawner;

    public void ActivateLoadingDoor()
    {
        playerSceneSpawner = GameManager.instance.playerObject.GetComponent<PlayerSceneSpawner>();

        int buildIndex = SceneUtility.GetBuildIndexByScenePath(_targetSceneName);

        if (buildIndex == -1)
        {
            Debug.Log("Door level name is invalid");
            return;
        }

        SceneLoader.LoadSceneAdditive(buildIndex);

        playerSceneSpawner.StartPlayerSpawn(_targetPlayerSpawnName, buildIndex);

        SceneLoader.UnloadSceneAdditive(gameObject.scene.buildIndex);
    }
}
