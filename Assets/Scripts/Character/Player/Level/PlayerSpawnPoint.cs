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
        //If no player in scene, load one in
        if (GameObject.FindGameObjectWithTag("Player") == null && spawnName == "default" && FindObjectOfType<SceneLoader>() == null)
        {
            SceneLoader.instance.LoadPlayerScene(gameObject.scene.buildIndex, "default", Vector3.zero, Vector3.zero, false, false);
        }
    }
}
