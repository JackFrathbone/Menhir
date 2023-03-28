using UnityEngine;
using FMOD.Studio;

public class SceneSoundController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] string musicEvent;
    [SerializeField] string ambienceEvent;

    private EventInstance _ambienceInstance;

    private void Start()
    {
        if (musicEvent.Length > 0)
        {
            AudioManager.instance.SetNewMusicTrack(musicEvent);
        }

        if (ambienceEvent.Length > 0)
        {
            _ambienceInstance = AudioManager.instance.CreateInstance(ambienceEvent);
            _ambienceInstance.start();
        }
    }

    private void OnDisable()
    {
        if (ambienceEvent.Length > 0)
        {
            _ambienceInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
