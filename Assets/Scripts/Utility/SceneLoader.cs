using System.Collections;
using System.Collections.Generic;
using Udar.SceneField;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [Header("Settings")]
    //The scene to load the player into when starting from the Player Scene for testing purposes
    private int _defaultScene;

    //The loading scene to show between scenes
    [SerializeField] SceneField _loadingScene;

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
        //Set the default scene for debugging
        _defaultScene = 4;

        LaunchDefaultScene();
        keepAlive = true;
    }


    //If the current scene is the player scene, load the default scene
    private void LaunchDefaultScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        if (currentScene == _playerScene)
        {
            LoadPlayerScene(_defaultScene, "default", Vector3.zero, Vector3.zero, false, true);
        }
        else
        {
            return;
        }
    }

    //For loading scenes without additional additive scenes
    public void LoadMenuScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex, LoadSceneMode.Single);
    }

    //Load the scene, then load the player scene in afterwards
    public void LoadPlayerScene(int targetScene, string spawnPointName, Vector3 overrideTransform, Vector3 overrideRotation, bool saveData, bool loadData)
    {
        StartCoroutine(WaitForLoadPlayerScene(targetScene, spawnPointName, overrideTransform, overrideRotation, saveData, loadData));
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
        if (GameObject.FindGameObjectWithTag("Weather") != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    IEnumerator WaitForLoadPlayerScene(int targetScene, string spawnPointName, Vector3 overrideTransform, Vector3 overrideRotation, bool saveData, bool loadData)
    {
        //Saves all the data from a scene being unloaded
        if (saveData)
        {
            yield return new WaitUntil(() => DataManager.instance.SaveSceneData(_currentScene) == true);
        }

        //Set the new currrent scene to be the scene thats getting loaded
        _currentScene = targetScene;

        AsyncOperation asyncLoad;

        //If in scene debug there will be no loading screen to load so skip
        if(_loadingScene != null)
        {
        //Enable the loading screen and unload current content
        asyncLoad = SceneManager.LoadSceneAsync(_loadingScene.BuildIndex, LoadSceneMode.Single);
        // Wait until the asynchronous scene fully loads
        yield return new WaitUntil(() => asyncLoad.isDone);
        }


        Scene currentScene = SceneManager.GetSceneByBuildIndex(_currentScene);

        //Check if current scene is loaded, and if not then load it in
        if (currentScene.buildIndex == -1)
        {
            //Clear the active characters in DataManager instance
            DataManager.instance.ClearActiveCharacters();

            //Loads the target scene
            asyncLoad = SceneManager.LoadSceneAsync(_currentScene, LoadSceneMode.Additive);

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

        if (loadData)
        {
            DataManager.instance.CheckLoadedScene();
        }

        //Moves player to set spawn point
        CharacterController charC = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        if (!loadData)
        {
            charC.GetComponent<PlayerCharacterManager>().LoadPlayer();
        }

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
                    charC.transform.SetPositionAndRotation(playerSpawnPoint.GetSpawnTransform().transform.position, playerSpawnPoint.GetSpawnTransform().transform.rotation);
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
        
        //If debug in a scene no loading screen will be set so skip this
        if(_loadingScene != null)
        {
        //Waits at least two seconds for the scene to finish loading up
        yield return new WaitForSeconds(2f);

        //Finishes loading and remove the loading screen
        SceneManager.UnloadSceneAsync(_loadingScene.BuildIndex);
        }
    }
}
