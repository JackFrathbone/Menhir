using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerCharacterStatsDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerName;
    [SerializeField] TextMeshProUGUI _playerHealth;
    [SerializeField] TextMeshProUGUI _playerStamina;

    [SerializeField] TextMeshProUGUI _abilityPhyisque, _abilityAgility, _abilityMental, _abilitySocial;

    public void UpdateStatDisplay(PlayerCharacterManager playerCharacterManager)
    {
        _playerName.text = playerCharacterManager.characterSheet.characterName;

        _playerHealth.text = "Total Health: " + playerCharacterManager.healthTotal.ToString();
        _playerStamina.text = "Total Stamina: " + playerCharacterManager.staminaTotal.ToString();

        _abilityPhyisque.text = "Physique " + playerCharacterManager.characterSheet.abilities.body;
        _abilityAgility.text = "Agility " + playerCharacterManager.characterSheet.abilities.hands;
        _abilityMental.text = "Mental " + playerCharacterManager.characterSheet.abilities.mind;
        _abilitySocial.text = "Social " + playerCharacterManager.characterSheet.abilities.heart;
    }
}
