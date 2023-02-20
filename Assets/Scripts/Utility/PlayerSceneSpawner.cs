using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSceneSpawner : MonoBehaviour
{
    [Header("Data")]
    private string spawnPointName;

    public void StartPlayerSpawn(string spawnName,int buildIndex)
    {
        spawnPointName = spawnName;

        StartCoroutine(WaitForSceneLoad(buildIndex));
    }

    private void MovePlayerToPoint()
    {
        PlayerSpawnPoint[] playerSpawnPoints = FindObjectsOfType<PlayerSpawnPoint>();

        PlayerSpawnPoint targetSpawnPoint = null;

        foreach(PlayerSpawnPoint spawnPoint in playerSpawnPoints)
        {
            if(spawnPoint.spawnName == spawnPointName)
            {
                targetSpawnPoint = spawnPoint;
            }

            if(targetSpawnPoint == null && spawnPoint.spawnName == "default")
            {
                targetSpawnPoint = spawnPoint;
            }
        }

        //Move the player
        if(targetSpawnPoint != null)
        {
            CharacterController playerController = GameManager.instance.playerObject.GetComponent<CharacterController>();

            playerController.enabled = false;

            GameManager.instance.playerObject.transform.SetPositionAndRotation(targetSpawnPoint.SpawnPointTransform().position, targetSpawnPoint.SpawnPointTransform().rotation);

            playerController.enabled = true;
        }

        spawnPointName = null;
    }

    IEnumerator WaitForSceneLoad(int buildIndex)
    {
        yield return new WaitUntil(() => SceneLoader.CheckSceneLoaded(buildIndex) == true);

        MovePlayerToPoint();
    }
}
