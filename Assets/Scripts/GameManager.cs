using UnityEngine;

public class GameManager: Singleton<GameManager>
{
    [Header("References")]
    public GameObject playerObject;
    public GameObject PlayerUIObject;

    public static PlayerActiveUI playerActiveUI;

   [ReadOnly] public bool isPaused;
    //Which screen this pause action is associated with, so other screens can overwrite
    [ReadOnly] public string pauseOrigin;

    //Private checks
    private PlayerController _playerController;

    public void PauseGame(bool playerPresent, string origin)
    {
        isPaused = true;

        if(pauseOrigin == "")
        {
            pauseOrigin = origin;
        }

        UnlockCursor();

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
        pauseOrigin = "";

        Time.timeScale = 1;

        if (_playerController != null && playerPresent)
        {
            _playerController.StartMovement();
            LockCursor();
        }
        else if(_playerController == null && playerPresent)
        {
            _playerController = playerObject.GetComponent<PlayerController>();
            _playerController.StartMovement();
            LockCursor();
        }
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool CheckCanPause(string pauseCompare)
    {
        if (!isPaused && pauseOrigin == "")
        {
            return true;
        }
        else if(isPaused && pauseOrigin == pauseCompare)
        {
            return true;
        }
        else if(!isPaused && pauseOrigin == pauseCompare)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckCanUnpause(string pauseCompare)
    {
        if (!isPaused && pauseOrigin == "")
        {
            return true;
        }
        else if (isPaused && pauseOrigin == pauseCompare)
        {
            return true;
        }
        else if (!isPaused && pauseOrigin == pauseCompare)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
