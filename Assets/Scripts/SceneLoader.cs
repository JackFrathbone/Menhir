using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Data")]
    private static readonly int _playerScene = 1;
    private static readonly int _weatherSystemScene = 2;

    private static bool _weatherSystemLoaded;

    //The first scene to load after the player scene is loaded// This should be changed in save games to the last scene the player was in
    [SerializeField] int _defaultScene;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

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

    public static void UnloadSceneAdditive(int i)
    {
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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckSceneOutdoors();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private static void CheckSceneOutdoors()
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
