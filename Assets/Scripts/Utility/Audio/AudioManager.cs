using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Data")]
    private List<EventInstance> eventInstances = new();
    private EventInstance _currentMusic;

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _effectsBus;
    private Bus _ambienceBus;

    private void Start()
    {
        DontDestroyOnLoad(this);

        _masterBus = RuntimeManager.GetBus("Bus:/");
        _musicBus = RuntimeManager.GetBus("Bus:/Music");
        _effectsBus = RuntimeManager.GetBus("Bus:/SFX");
        _ambienceBus = RuntimeManager.GetBus("Bus:/Ambience");

        UpdateVolume();
    }

    public void SetNewMusicTrack(string track)
    {
        //Check if the instance has been created yet
        if (!_currentMusic.isValid())
        {
            _currentMusic = RuntimeManager.CreateInstance(track);
            eventInstances.Add(_currentMusic);
        }
        else
        {
            //Stop the current track
            _currentMusic.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _currentMusic.release();
            eventInstances.Remove(_currentMusic);

            //Set the new track
            _currentMusic = RuntimeManager.CreateInstance(track);
            eventInstances.Add(_currentMusic);
        }

        //Check if previous track was stopped and start playing again
        _currentMusic.getPlaybackState(out PLAYBACK_STATE playbackstate);

        if (playbackstate.Equals(PLAYBACK_STATE.STOPPED))
        {
            _currentMusic.start();
        }
    }

    public void UpdateVolume()
    {
        _masterBus.setVolume(PlayerPrefs.GetFloat("masterVolume"));
        _musicBus.setVolume(PlayerPrefs.GetFloat("musicVolume"));
        _effectsBus.setVolume(PlayerPrefs.GetFloat("effectsVolume"));
        _ambienceBus.setVolume(PlayerPrefs.GetFloat("ambienceVolume"));
    }
    public void PlayOneShot(string sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(string sound)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(sound);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
