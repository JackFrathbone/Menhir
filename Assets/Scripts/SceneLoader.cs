using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private int _playerScene = 1;

    //The first scene to load after the player scene is loaded// This should be changed in save games to the last scene the player was in
    [SerializeField] int _defaultScene;

    private void Start()
    {
        LaunchDefaultScene();

        //If the scene contains the player, make their scene the active one
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }
    }

    //If the current scene is the player scene, load the default scene
    private void LaunchDefaultScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if(currentScene == _playerScene)
        {
            LoadSceneAdditive(_defaultScene);
        }
        else
        {
            return;
        }
    }

    public static void LoadSceneSingle(int i)
    {
        SceneManager.LoadScene(i, LoadSceneMode.Single);
    }

    public static void LoadSceneAdditive(int i)
    {
        SceneManager.LoadScene(i, LoadSceneMode.Additive);
 
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
