using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [Header("Settings")]
    //The scene to load the player into when starting from the Player Scene for testing purposes
    [SerializeField] int _defaultScene;


    [Header("Data")]
    private int _playerScene = 1;
    private int _weatherSystemScene = 2;

    private int _currentScene;


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
        keepAlive = true;
    }


    //If the current scene is the player scene, load the default scene
    private void LaunchDefaultScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene == _playerScene)
        {
            LoadPlayerScene(_defaultScene, "default", Vector3.zero, Vector3.zero);
        }
        else
        {
            return;
        }
    }

    //For loading scenes without additional additive scenes
    public void LoadMenuScene(int i)
    {
        SceneManager.LoadScene(i, LoadSceneMode.Single);
    }

    //Load the scene, then load the player scene in afterwards
    public void LoadPlayerScene(int i, string spawnPointName, Vector3 overrideTransform, Vector3 overrideRotation)
    {
        _currentScene = i;

        StartCoroutine(WaitForLoadPlayerScene(spawnPointName, overrideTransform, overrideRotation));
    }

    public bool CheckSceneLoaded(int i)
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

    public void MoveObjectToScene(GameObject gameObject)
    {
        Scene currentScene = SceneManager.GetSceneByBuildIndex(_currentScene);

        if(currentScene.buildIndex == -1)
        {
            Debug.Log("Invalid current scene");
            return;
        }

        SceneManager.MoveGameObjectToScene(gameObject, currentScene);
    }

    public int GetCurrentScene()
    {
        return _currentScene;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    private void OnSceneUnloaded(Scene scene)
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private bool CheckSceneOutdoors()
    {
        //If there is a terrain object, load in the weather scene additively
        if (FindObjectOfType<Terrain>() != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator WaitForLoadPlayerScene(string spawnPointName, Vector3 overrideTransform, Vector3 overrideRotation)
    {
        //Saves all the data from a scene being unloaded
        yield return new WaitUntil(() => DataManager.instance.SaveSceneData(_currentScene) == true);

        AsyncOperation asyncLoad;

        Scene currentScene = SceneManager.GetSceneByBuildIndex(_currentScene);

        //Dont load scene if already loaded
        if (currentScene.buildIndex == -1)
        {
            //Loads the target scene
            asyncLoad = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Single);

            // Wait until the asynchronous scene fully loads
            yield return new WaitUntil(() => asyncLoad.isDone);
        }

        //Load the player scene
        asyncLoad = SceneManager.LoadSceneAsync(_playerScene, LoadSceneMode.Additive);

        // Wait until the asynchronous scene fully loads
        yield return new WaitUntil(() => asyncLoad.isDone);


        //Checks if scene is outdoors
        if (CheckSceneOutdoors())
        {
            asyncLoad = SceneManager.LoadSceneAsync(_weatherSystemScene, LoadSceneMode.Additive);

            // Wait until the asynchronous scene fully loads
            yield return new WaitUntil(() => asyncLoad.isDone);
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(_playerScene));
        DataManager.instance.CheckLoadedScene();


        //Moves player to set spawn point
        CharacterController charC = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        if(charC == null)
        {
            Debug.Log("No player found!");
        }

        //If a spawnpoint has been declared and there is not set vector3 move the player to a spawn point
        if(spawnPointName != null)
        {
            if (spawnPointName == "")
            {
                spawnPointName = "default";
            }

            List<PlayerSpawnPoint> playerSpawnPoints = new(GameObject.FindObjectsOfType<PlayerSpawnPoint>());

            foreach(PlayerSpawnPoint playerSpawnPoint in playerSpawnPoints)
            {
                if(playerSpawnPoint.spawnName == spawnPointName)
                {
                    charC.enabled = false;
                    charC.transform.SetPositionAndRotation(playerSpawnPoint.transform.position, playerSpawnPoint.transform.rotation);
                    charC.enabled = true;
                }
            }
        }
        //If no spawnpoint specified then move player to their saved position
        else if(spawnPointName == null)
        {
            charC.enabled = false;
            charC.transform.SetPositionAndRotation(overrideTransform, Quaternion.Euler(overrideRotation));
            charC.enabled = true;
        }
        else
        {
            Debug.Log("Invalid spawn or player position");
        }
    }
}
