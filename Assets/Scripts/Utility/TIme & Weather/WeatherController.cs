using System;
using System.Collections;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.Rendering.Universal;


public class WeatherController : MonoBehaviour
{
    [Header("Settings")]
    [Range(0f, 100f)]
    [SerializeField] float _weatherParticlesHeight;

    //Used for the controlling of outdoor world lights and weather effects, get disabled in indoor locations
    [Header("References")]
    [SerializeField] Light _directionalLight;
    [SerializeField] LightCycleSettingsPreset _settingsPreset;

    //For the clouds
    [SerializeField] MeshRenderer _cloudLayerRender;
    private Material _cloudLayerMat;

    //For the rain
    private Transform _playerTransform;
    [SerializeField] GameObject _rainLightPrefab;
    [SerializeField] GameObject _rainHeavyPrefab;

    private GameObject _currentRain;

    //For audio
    private EventInstance _outdoorAmbience;

    [Header("Data")]
    [Tooltip("clear, cloudyLight, cloudy, rainLight, rainHeavy")]
    [ReadOnly] public string currentWeather = "clear";

    private void Start()
    {
        _cloudLayerMat = _cloudLayerRender.material;

        UpdateLighting((TimeController.GetHours() * 60 + TimeController.GetMinutes()) / 1440f);
        GetRandomWeather();

        //Set the audio
        _outdoorAmbience = AudioManager.instance.CreateInstance("event:/AmbienceOutdoor");
        _outdoorAmbience.start();
        UpdateOutdoorAmbience();
    }

    private void OnEnable()
    {
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox;
        }

        TimeController.onLightingUpdate += UpdateLighting;
        TimeController.onWeatherUpdate += GetRandomWeather;
    }

    private void OnDisable()
    {
        TimeController.onLightingUpdate -= UpdateLighting;
        TimeController.onWeatherUpdate -= GetRandomWeather;
        RenderSettings.ambientLight = _settingsPreset.interiorAmbientColour;

        _outdoorAmbience.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void OnDestroy()
    {
        RenderSettings.ambientLight = _settingsPreset.interiorAmbientColour;
    }

    private void UpdateLighting(float timePercent)
    {
        if (_settingsPreset == null)
        {
            return;
        }

        //Sets the ambient colour based on if its raining or not
        if (currentWeather != "rainLight" && currentWeather != "rainHeavy")
        {
            RenderSettings.ambientLight = _settingsPreset.ambientColour.Evaluate(timePercent);

            if (_directionalLight != null)
            {
                _directionalLight.color = _settingsPreset.ambientColour.Evaluate(timePercent);
                _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
                Shader.SetGlobalVector("_SunDirection", _directionalLight.transform.forward);
            }
        }
        else
        {
            RenderSettings.ambientLight = _settingsPreset.ambientColourRain.Evaluate(timePercent);

            if (_directionalLight != null)
            {
                _directionalLight.color = _settingsPreset.ambientColourRain.Evaluate(timePercent);
                _directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170, 0));
                Shader.SetGlobalVector("_SunDirection", _directionalLight.transform.forward);
            }
        }

        RenderSettings.fogColor = _settingsPreset.fogColour.Evaluate(timePercent);
        RenderSettings.fogEndDistance = _settingsPreset.fogIntensity.Evaluate(timePercent);

        //For the skybox
        RenderSettings.skybox.SetColor("_SkyboxUpperColour", _settingsPreset.skyboxUpperColour.Evaluate(timePercent));
        RenderSettings.skybox.SetColor("_SkyboxLowerColour", _settingsPreset.fogColour.Evaluate(timePercent));


        //For enabling the stars
        if (TimeController.GetHours() >= 18 || TimeController.GetHours() <= 5)
        {
            RenderSettings.skybox.SetFloat("_StarDensity", 5);
        }
        else
        {
            RenderSettings.skybox.SetFloat("_StarDensity", 0);
        }
    }

    public void GetRandomWeather()
    {
        if(_currentRain != null)
        {
            Destroy(_currentRain);
            _currentRain = null;
        }

        int randomWeather = UnityEngine.Random.Range(0, 5);

        //requires weather to already be cloudy in order to rain
        switch (randomWeather)
        {
            case 0:
                SetWeatherClear();
                break;
            case 1:
                SetWeatherCloudyLight();
                break;
            case 2:
                SetWeatherCloudy();
                break;
            case 3:
                SetWeatherRainLight();
                break;
            case 4:
                SetWeatherRainHeavy();
                break;
        }

        UpdateOutdoorAmbience();
    }

    private void SetWeatherClear()
    {
        currentWeather = "clear";
        ClearRainParticles();

        StartCoroutine(CloudTransition(0.75f));
    }

    private void SetWeatherCloudyLight()
    {
        currentWeather = "cloudyLight";
        ClearRainParticles();

        StartCoroutine(CloudTransition(1f));
    }

    private void SetWeatherCloudy()
    {
        currentWeather = "cloudy";
        ClearRainParticles();

        StartCoroutine(CloudTransition(1.5f));
    }

    private void SetWeatherRainLight()
    {
        currentWeather = "rainLight";
        ClearRainParticles();

        SetRainParticles();
        StartCoroutine(CloudTransition(3f));
    }

    private void SetWeatherRainHeavy()
    {
        currentWeather = "rainHeavy";
        ClearRainParticles();

        SetRainParticles();
        StartCoroutine(CloudTransition(4f));
    }

    private void SetRainParticles()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            if (_playerTransform == null)
            {
                return;
            }
        }

        Vector3 particleOffset = new(0f, _weatherParticlesHeight, 0f);

        if (currentWeather == "rainLight")
        {
            _currentRain = Instantiate(_rainLightPrefab, _playerTransform.localPosition, Quaternion.identity, _playerTransform);
            _currentRain.transform.localPosition = particleOffset;
        }
        else if (currentWeather == "rainHeavy")
        {
            _currentRain = Instantiate(_rainHeavyPrefab, _playerTransform.localPosition, Quaternion.identity, _playerTransform);
            _currentRain.transform.localPosition = particleOffset;
        }
        else
        {
            Debug.Log("Invalid weather for rain");
        }
    }

    private void ClearRainParticles()
    {
        if (_playerTransform == null)
        {
            _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

            if (_playerTransform == null)
            {
                return;
            }
        }

        if (GameObject.FindGameObjectWithTag("Weather") != null)
        {
            Destroy(GameObject.FindGameObjectWithTag("Weather"));
        }
    }

    private IEnumerator CloudTransition(float targetCloudAlpha)
    {
        float currentCloudAlpha = _cloudLayerMat.GetFloat("_CloudsAlpha");

        if (targetCloudAlpha >= currentCloudAlpha)
        {
            for (float f = currentCloudAlpha; f <= targetCloudAlpha; f += 0.05f)
            {
                currentCloudAlpha += 0.05f;
                _cloudLayerMat.SetFloat("_CloudsAlpha", currentCloudAlpha);
                yield return new WaitForSeconds(.1f);
            }
        }
        else if (targetCloudAlpha <= currentCloudAlpha)
        {
            for (float f = currentCloudAlpha; f >= targetCloudAlpha; f -= 0.05f)
            {
                currentCloudAlpha -= 0.05f;
                _cloudLayerMat.SetFloat("_CloudsAlpha", currentCloudAlpha);
                yield return new WaitForSeconds(.1f);
            }
        }

    }

    private void UpdateOutdoorAmbience()
    {
        if (TimeController.GetHours() >= 18 || TimeController.GetHours() <= 5)
        {
            _outdoorAmbience.setParameterByName("Time", 1);
        }
        else
        {
            _outdoorAmbience.setParameterByName("Time", 0);
        }

        switch (currentWeather)
        {
            case "clear":
                _outdoorAmbience.setParameterByName("Rain", 0);
                break;
            case "cloudyLight":
                _outdoorAmbience.setParameterByName("Rain", 0);
                break;
            case "cloud":
                _outdoorAmbience.setParameterByName("Rain", 0);
                break;
            case "rainLight":
                _outdoorAmbience.setParameterByName("Rain", 1);
                break;
            case "rainHeavy":
                _outdoorAmbience.setParameterByName("Rain", 2);
                break;
        }
    }
}
