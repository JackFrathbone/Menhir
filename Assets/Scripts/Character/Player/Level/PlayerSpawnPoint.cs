using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnPoint : MonoBehaviour
{
    private void Update()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            MovePlayerToPoint();
        }
        else
        {
            LoadPlayerIntoScene();
        }
    }

    private void MovePlayerToPoint()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        playerObject.transform.position = transform.position;
        playerObject.transform.rotation = transform.rotation;

        Destroy(gameObject, 1f);
    }

    private void LoadPlayerIntoScene()
    {
        SceneLoader.LoadSceneAdditive(1);
    }
}
