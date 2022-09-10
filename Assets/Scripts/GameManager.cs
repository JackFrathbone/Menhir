using UnityEngine;
using UnityEditor.SceneManagement;

public class GameManager: Singleton<GameManager>
{
    public GameObject playerObject;
    public GameObject PlayerUIObject;

    public static PlayerActiveUI playerActiveUI;

    public bool isPaused;

    //Private checks
    private PlayerController _playerController;

    public void PauseGame(bool playerPresent)
    {
        isPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;

        if(_playerController != null && playerPresent)
        {
            _playerController.StopMovement();
        }
        else if(_playerController == null && playerPresent)
        {
            _playerController = playerObject.GetComponent<PlayerController>();
            _playerController.StopMovement();
        }
    }

    public void UnPauseGame(bool playerPresent)
    {
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;

        if (_playerController != null && playerPresent)
        {
            _playerController.StartMovement();
        }
        else if(_playerController == null && playerPresent)
        {
            _playerController = playerObject.GetComponent<PlayerController>();
            _playerController.StartMovement();
        }
    }
}
