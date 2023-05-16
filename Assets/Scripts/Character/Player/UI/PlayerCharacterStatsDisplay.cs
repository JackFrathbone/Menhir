using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerCharacterStatsDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] TextMeshProUGUI _playerHealth;
    [SerializeField] TextMeshProUGUI _playerStamina;

    [SerializeField] TextMeshProUGUI _abilityPhyisque, _abilityAgility, _abilityMental, _abilitySocial;

    //For the skills
    [SerializeField] GameObject _skillButtonPrefab;
    [SerializeField] Transform _skillListParent;
    [SerializeField] GameObject _skillDescriptionBox;
    [SerializeField] TextMeshProUGUI _skillDescriptionBoxLabel;
    [SerializeField] TextMeshProUGUI _skillDescriptionBoxText;

    private void Start()
    {
        _skillDescriptionBox.SetActive(false);
    }

    public void UpdateStatDisplay(PlayerCharacterManager playerCharacterManager)
    {
        _playerName.text = playerCharacterManager.characterName;

        _playerHealth.text = "Health: " + playerCharacterManager.healthCurrent.ToString() +"/"+ playerCharacterManager.healthTotal.ToString();
        _playerStamina.text = "Stamina: " + playerCharacterManager.staminaCurrent.ToString() + "/" + playerCharacterManager.staminaTotal.ToString();

        _abilityPhyisque.text = "Body " + playerCharacterManager.abilities.body;
        _abilityAgility.text = "Hands " + playerCharacterManager.abilities.hands;
        _abilityMental.text = "Mind " + playerCharacterManager.abilities.mind;
        _abilitySocial.text = "Heart " + playerCharacterManager.abilities.heart;
    }

    public void AddSkill(Skill skill)
    {
        GameObject newSkillButton = Instantiate(_skillButtonPrefab, _skillListParent);
        newSkillButton.GetComponent<Button>().onClick.AddListener(delegate { ShowSkillDescription(skill); });
        newSkillButton.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;
    }

    private void ShowSkillDescription(Skill skill)
    {
        _skillDescriptionBox.SetActive(true);
        _skillDescriptionBoxLabel.text = skill.skillName;
        _skillDescriptionBoxText.text = skill.skillDescription;
    }
}
