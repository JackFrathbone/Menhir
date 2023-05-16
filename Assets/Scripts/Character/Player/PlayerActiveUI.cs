using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerActiveUI : MonoBehaviour
{
    [Header("Player Status UI")]
    [SerializeField] Slider _playerHealth;
    [SerializeField] Slider _playerStamina;

    [Header("Target Status UI")]
    [SerializeField] Slider _targetHealth;
    [SerializeField] TextMeshProUGUI _targetName;
    [SerializeField] GameObject _targetStatusParent;

    [Header("Crosshair UI")]
    [SerializeField] TextMeshProUGUI _crosshairText;

    public void UpdateStatusUI(float healthCurrent, float healthTotal, float staminaCurrent, float staminaTotal)
    {
        _playerHealth.maxValue = healthTotal;
        _playerHealth.value = healthCurrent;

        _playerStamina.maxValue = staminaTotal;
        _playerStamina.value = staminaCurrent;
    }

    public void UpdateStatusStaminaUI(float staminaCurrent, float staminaTotal)
    {
        _playerStamina.maxValue = staminaTotal;
        _playerStamina.value = staminaCurrent;
    }

    public void UpdateTargetStatusUI(CharacterManager target)
    {
        _targetHealth.maxValue = target.healthTotal;
        _targetHealth.value = target.healthCurrent;

        _targetName.text = target.characterName;

        _targetStatusParent.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(FadeTargetStatus());
    }

    private void HideTargetStatusUI()
    {
        _targetStatusParent.SetActive(false);
    }

    public void EnableCrosshairText(string s)
    {
        _crosshairText.transform.parent.gameObject.SetActive(true);
        _crosshairText.text = s;
    }

    public void DisableCrosshairText()
    {
        _crosshairText.transform.parent.gameObject.SetActive(false);
    }

    private IEnumerator FadeTargetStatus()
    {
        yield return new WaitForSeconds(5f);
        HideTargetStatusUI();
    }
}
