using UnityEngine;
using FMOD.Studio;

public class SceneSoundController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] FMODUnity.EventReference musicEvent;
    [SerializeField] FMODUnity.EventReference ambienceEvent;

    private EventInstance _ambienceInstance;

    private void Start()
    {
        if (musicEvent.Path.Length >0)
        {
            AudioManager.instance.SetNewMusicTrack(musicEvent);
        }

        if (ambienceEvent.Path.Length > 0)
        {
            _ambienceInstance = AudioManager.instance.CreateInstance(ambienceEvent.Path);
            _ambienceInstance.start();
        }
    }

    private void OnDisable()
    {
        if (ambienceEvent.Path != null)
        {
            _ambienceInstance.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }
}
