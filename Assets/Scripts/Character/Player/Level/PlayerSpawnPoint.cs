using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPoint : MonoBehaviour
{
    [Header("Settings")]
    //'default' will make this spawnpoint active if no valid name is used when changing levels
    [Tooltip("The name used to associate this spawner with a door")] public string spawnName = "default";

    [Header("References")]
    [SerializeField] Transform playerSpawnTransform;

    private void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null && spawnName == "default")
        {
            LoadPlayerIntoScene();
        }
    }

    private void LoadPlayerIntoScene()
    {
        SceneLoader.LoadSceneAdditive(1);

        StartCoroutine(WaitForPlayerSpawn());
    }

    public Transform SpawnPointTransform()
    {
        return playerSpawnTransform;
    }

    public void MovePlayerToPoint()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.transform.SetPositionAndRotation(playerSpawnTransform.position, playerSpawnTransform.rotation);
    }

    IEnumerator WaitForPlayerSpawn()
    {
        yield return new WaitUntil(() => SceneLoader.CheckSceneLoaded(1) == true);

        MovePlayerToPoint();
    }
}
