using UnityEngine;
using FMOD.Studio;
using UnityEngine.UI;

public class GameSettingsController : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] Slider _masterSlider;
    [SerializeField] Slider _musicSlider;
    [SerializeField] Slider _effectsSlider;
    [SerializeField] Slider _ambienceSlider;

    private void Start()
    {
        //Set the audio sliders
        _masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        _effectsSlider.value = PlayerPrefs.GetFloat("effectsVolume");
        _ambienceSlider.value = PlayerPrefs.GetFloat("ambienceVolume");

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
