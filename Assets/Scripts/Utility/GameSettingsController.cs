using UnityEngine;
using FMOD.Studio;
using UnityEngine.UI;

public class GameSettingsController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField, Range(0, 100)] float _defaultMasterVolume;
    [SerializeField, Range(0, 100)] float _defaultMusicVolume;
    [SerializeField, Range(0, 100)] float _defaultEffectsVolume;
    [SerializeField, Range(0, 100)] float _defaultAmbianceVolume;

    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _effectsSlider;
    [SerializeField] Slider _ambienceSlider;

    private void Start()
    {
        //Set the audio sliders
        _masterSlider.value = PlayerPrefs.GetFloat("masterVolume", _defaultMasterVolume);
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume", _defaultMusicVolume);
        _effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume", _defaultEffectsVolume);
        _ambienceSlider.value = PlayerPrefs.GetFloat("ambienceVolume", _defaultAmbianceVolume);

    }

    public void SetAudio(string s)
    {
        switch (s)
        {   
            case "master":
                PlayerPrefs.SetFloat("masterVolume", _masterSlider.value);
                break;
            case "music":
                PlayerPrefs.SetFloat("musicVolume", _musicSlider.value);
                break;
            case "effects":
                PlayerPrefs.SetFloat("effectsVolume", _effectsSlider.value);
                break;
            case "ambience":
                PlayerPrefs.SetFloat("ambienceVolume", _ambienceSlider.value);
                break;
        }

        AudioManager.instance.UpdateVolume();
    }
}
