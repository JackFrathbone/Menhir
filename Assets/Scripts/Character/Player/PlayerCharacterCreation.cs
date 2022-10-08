using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCharacterCreation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterSheet _playerCharacterSheet;

    //Abilities
    [SerializeField] TextMeshProUGUI _abilityPointsLeftText;
    [SerializeField] TextMeshProUGUI _bodyText, _handsText, _mindText, _heartText;
    [SerializeField] int _abilityPointsLeft;
    private int _body = 3, _hands = 3, _mind = 3, _heart = 3;

    //Skills
    [SerializeField] GameObject _skillButtonPrefab;
    [SerializeField] Transform _skillListParent;

    [SerializeField] TextMeshProUGUI _skillDescription;
    [SerializeField] TextMeshProUGUI _skillSlot1Text;
    [SerializeField] TextMeshProUGUI _skillSlot2Text;

    [SerializeField] List<Skill> _availableSkills = new List<Skill>();

    private Skill _selectedSkill;
    private Skill _setSkill1;
    private Skill _setSkill2;

    //Stuff for finish screen
    [SerializeField] GameObject _finishButton;
    [SerializeField] GameObject _finishIncompleteText;

    private void Start()
    {
        _abilityPointsLeftText.text = "Points Left: " + _abilityPointsLeft.ToString();
        SetAbilityText();
        SetupSkills();
    }

    private void SetupSkills()
    {
        foreach (Skill skill in _availableSkills)
        {
            Button newButton = Instantiate(_skillButtonPrefab, _skillListParent).GetComponent<Button>();
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = skill.skillName;
            newButton.onClick.AddListener(delegate { SelectSkill(skill); });
        }
    }

    public void SelectSkill(Skill skill)
    {
        _skillDescription.text = skill.skillDescription;

        _selectedSkill = skill;
    }

    public void SetSkill(int slot)
    {
        if(_selectedSkill == null)
        {
            return;
        }

        if (slot == 1 && _setSkill2 != _selectedSkill)
        {
            _setSkill1 = _selectedSkill;
            _skillSlot1Text.text = _setSkill1.skillName;
        }
        else if (slot == 2 && _setSkill1 != _selectedSkill)
        {
            _setSkill2 = _selectedSkill;
            _skillSlot2Text.text = _setSkill2.skillName;
        }

        _selectedSkill = null;
    }

    public void SpendAbilityPoint(string ability)
    {
        if (_abilityPointsLeft > 0)
        {
            _abilityPointsLeft--;

            switch (ability)
            {
                case "body":
                    _body++;
                    break;
                case "hands":
                    _hands++;
                    break;
                case "mind":
                    _mind++;
                    break;
                case "heart":
                    _heart++;
                    break;
            }

            PreventAbilityOverflow(ability);
        }
        else
        {
            return;
        }
    }

    public void RemoveAbilityPoint(string ability)
    {
        _abilityPointsLeft++;

        switch (ability)
        {
            case "body":
                _body--;
                break;
            case "hands":
                _hands--;
                break;
            case "mind":
                _mind--;
                break;
            case "heart":
                _heart--;
                break;
        }

        PreventAbilityOverflow(ability);
    }

    private void PreventAbilityOverflow(string ability)
    {
        switch (ability)
        {
            case "body":
                if (_body > 10)
                {
                    _body = 10;
                    _abilityPointsLeft++;
                }
                else if (_body < 0)
                {
                    _body = 0;
                    _abilityPointsLeft--;
                }
                break;
            case "hands":
                if (_hands > 10)
                {
                    _hands = 10;
                    _abilityPointsLeft++;
                }
                else if (_hands < 0)
                {
                    _hands = 0;
                    _abilityPointsLeft--;
                }
                break;
            case "mind":
                if (_mind > 10)
                {
                    _mind = 10;
                    _abilityPointsLeft++;
                }
                else if (_mind < 0)
                {
                    _mind = 0;
                    _abilityPointsLeft--;
                }
                break;
            case "heart":
                if (_heart > 10)
                {
                    _heart = 10;
                    _abilityPointsLeft++;
                }
                else if (_heart < 0)
                {
                    _heart = 0;
                    _abilityPointsLeft--;
                }
                break;
        }

        SetAbilityText();

        _abilityPointsLeftText.text = "Points Left: " + _abilityPointsLeft.ToString();
    }

    private void SetAbilityText()
    {
        _bodyText.text = _body.ToString();
        _handsText.text = _hands.ToString();
        _mindText.text = _mind.ToString();
        _heartText.text = _heart.ToString();
    }

    public void CheckCreationComplete()
    {
        if (_abilityPointsLeft == 0 && _setSkill1 != null && _setSkill2 != null)
        {
            _finishButton.SetActive(true);
            _finishIncompleteText.SetActive(false);
        }
        else
        {
            _finishButton.SetActive(false);
            _finishIncompleteText.SetActive(true);
        }
    }

    public void SetCharacterSheet()
    {
        //Abilities
        _playerCharacterSheet.abilities.body = _body;
        _playerCharacterSheet.abilities.hands = _hands;
        _playerCharacterSheet.abilities.mind = _mind;
        _playerCharacterSheet.abilities.heart = _heart;

        //Skills
        _playerCharacterSheet.skills.Clear();

        _playerCharacterSheet.skills.Add(_setSkill1);
        _playerCharacterSheet.skills.Add(_setSkill2);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
