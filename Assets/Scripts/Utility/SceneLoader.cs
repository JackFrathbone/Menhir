using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Data")]
    private static readonly int _playerScene = 1;
    private static readonly int _weatherSystemScene = 2;

    private static bool _weatherSystemLoaded;

    private static Scene _currentScene;

    //The first scene to load after the player scene is loaded// This should be changed in save games to the last scene the player was in
    [SerializeField] int _defaultScene;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void Start()
    {
        LaunchDefaultScene();

        //If the scene contains the player, make their scene the active one
        if(GameObject.FindGameObjectWithTag("Player") != null)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
            GameManager.instance.playerObject.GetComponent<PlayerSceneSpawner>().StartPlayerSpawn("default", _defaultScene);
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

    public static void UnloadSceneAdditive(int i)
    {
        //Saves all the data from a scene being unloaded
        GameManager.instance.dataManager.SaveSceneData(i);

        SceneManager.UnloadSceneAsync(i);
    }

    public static bool CheckSceneLoaded(int i)
    {
        Scene sceneToCheck = SceneManager.GetSceneByBuildIndex(i);

        for (int sceneNum = 0; sceneNum < SceneManager.sceneCount; sceneNum++)
        {
            Scene scene = SceneManager.GetSceneAt(sceneNum);

            if ((scene == sceneToCheck && scene.isLoaded))
            {
                return true;
            }
        }

        return false;
    }

    public static void MoveObjectToScene(GameObject gameObject)
    {
        SceneManager.MoveGameObjectToScene(gameObject, _currentScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneOutdoors();

        //Dont check if is playerScene
        if (scene.buildIndex == 0 || scene.buildIndex == 1 || scene.buildIndex == 2 || scene.buildIndex == 3)
        {
            return;
        }

        _currentScene = scene;

        if (mode == LoadSceneMode.Additive)
        {
            GameManager.instance.dataManager.CheckLoadedScene(scene);
        }
    }

    private void OnSceneUnloaded(Scene scene)
    {
        CheckSceneOutdoors();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void CheckSceneOutdoors()
    {
        if (FindObjectOfType<Terrain>() != null)
        {
            if (!_weatherSystemLoaded)
            {
                _weatherSystemLoaded = true;
                LoadSceneAdditive(_weatherSystemScene);
            }
        }
        else
        {
            if (_weatherSystemLoaded)
            {
                UnloadSceneAdditive(_weatherSystemScene);
                _weatherSystemLoaded = false;
            }
        }
    }
}
