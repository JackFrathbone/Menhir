using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeWaitController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TimeController _timeController;

    [SerializeField] GameObject _waitMenuUI;
    [SerializeField] TextMeshProUGUI _waitText;
    [SerializeField] TextMeshProUGUI _buttonText;

    [SerializeField] Slider _timeSlider;

    private PlayerCharacterManager _playerCharacterManager;

    [Header("Data")]
    private bool _canSleep;
    private int _hoursToWait;

    private void Start()
    {
        _waitMenuUI.SetActive(false);
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
    }

    public void OpenWaitMenu(bool canSleep)
    {
        if (!GameManager.instance.CheckCanPause("waitMenu"))
        {
            return;
        }

        GameManager.instance.PauseGame(true, "waitMenu");
        _waitMenuUI.SetActive(true);

        _canSleep = canSleep;

        _hoursToWait = 1;
        UpdateWaitMenu();
    }

    public void UpdateWaitMenu()
    {
        _hoursToWait = (int)_timeSlider.value;

        if (_canSleep)
        {
            _waitText.text = "<b>Sleep for " + _hoursToWait + " hours</b><br>You will restore health and stamina";
            _buttonText.text = "Sleep";
        }
        else
        {
            _waitText.text = "<b>Sleep for " + _hoursToWait + " hours</b><br>You will restore stamina";
            _buttonText.text = "Wait";
        }
    }

    public void Wait()
    {
        _timeController.AddHours(_hoursToWait);

        //Healh the player and restore stamina
        if (_canSleep)
        {
            _playerCharacterManager.AddHealth(StatFormulas.RestRestoreHealth(_hoursToWait));
        }
        _playerCharacterManager.AddStamina(StatFormulas.RestRestoreStamina(_hoursToWait));

        CloseWaitMenu();
    }

    public void CloseWaitMenu()
    {
        if (!GameManager.instance.CheckCanUnpause("waitMenu"))
        {
            return;
        }

        _waitMenuUI.SetActive(false);
        GameManager.instance.UnPauseGame(true);

        MessageBox.instance.Create("Time has Passed...", true);
    }
}
