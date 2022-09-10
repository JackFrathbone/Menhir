using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PlayerCharacterCreation : MonoBehaviour
{
    [SerializeField] CharacterSheet _playerCharacterSheet;

    [SerializeField] TextMeshProUGUI _abilityPointsLeftText;
    [SerializeField] TextMeshProUGUI _physiqueText, _agilityText, _mentalText, _socialText;
    [SerializeField] int _abilityPointsLeft;
    private int _physique = 0, _agility = 0, _mental = 0, _social = 0;

    [SerializeField] TextMeshProUGUI _skillShortWeapons, _skillLongWeapons, _skillBlock, _skillHealing, _skillRangedWeapons, _skillDodge, _skillStealth, _skillTrapsmith, _skillElementalism, _skillSoulMagic, _skillRitualism, _skillAlchemy, _skillPersuasion, _skillIntimidation, _skillBarter, _skillWisdom;
    [SerializeField] TextMeshProUGUI _majorSkillText1, _majorSkillText2, _majorSkillText3, _majorSkillText4;
    [SerializeField] TextMeshProUGUI _minorSkillText1, _minorSkillText2, _minorSkillText3, _minorSkillText4;
    [SerializeField] TextMeshProUGUI _description;

    private string _selectedSkill = "";

    //Stuff for finish screen
    [SerializeField] GameObject _finishButton;
    [SerializeField] GameObject _finishIncompleteText;

    //List of currently selected skills via string
    private List<string> selectedSkills = new List<string>();

    private void Start()
    {
        _abilityPointsLeftText.text = "Points Left: " + _abilityPointsLeft.ToString();
        DisplaySkills();

    }

    public void SpendAbilityPoint(string ability)
    {
        if (_abilityPointsLeft > 0)
        {
            _abilityPointsLeft--;

            switch (ability)
            {
                case "physique":
                    _physique++;
                    break;
                case "agility":
                    _agility++;
                    break;
                case "mental":
                    _mental++;
                    break;
                case "social":
                    _social++;
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
            case "physique":
                _physique--;
                break;
            case "agility":
                _agility--;
                break;
            case "mental":
                _mental--;
                break;
            case "social":
                _social--;
                break;
        }

        PreventAbilityOverflow(ability);
    }

    public void DisplaySkills()
    {
        _skillShortWeapons.text = Skills.GetSkillName("shortWeapons");
        _skillLongWeapons.text = Skills.GetSkillName("longWeapons");
        _skillBlock.text = Skills.GetSkillName("block");
        _skillHealing.text = Skills.GetSkillName("healing");

        _skillRangedWeapons.text = Skills.GetSkillName("rangedWeapons");
        _skillDodge.text = Skills.GetSkillName("dodge");
        _skillStealth.text = Skills.GetSkillName("stealth");
        _skillTrapsmith.text = Skills.GetSkillName("trapsmith");

        _skillElementalism.text = Skills.GetSkillName("elementalism");
        _skillSoulMagic.text = Skills.GetSkillName("soulMagic");
        _skillRitualism.text = Skills.GetSkillName("ritualism");
        _skillAlchemy.text = Skills.GetSkillName("alchemy");

        _skillPersuasion.text = Skills.GetSkillName("persuasion");
        _skillIntimidation.text = Skills.GetSkillName("intimidation");
        _skillBarter.text = Skills.GetSkillName("barter");
        _skillWisdom.text = Skills.GetSkillName("wisdom");
    }

    public void SelectSkill(string skill)
    {
        _selectedSkill = skill;

        _description.text = Skills.GetSkillDescription(Skills.GetSkillVariableName(skill));
    }

    public void SetSkillSlot(int selectedSlot)
    {
        if (_selectedSkill != "" && !selectedSkills.Contains(_selectedSkill))
        {
            switch (selectedSlot)
            {
                case 1:
                    if (_majorSkillText1.text != "")
                    {
                        EmptySkillSlot(1);
                    }
                    _majorSkillText1.text = _selectedSkill;
                    break;
                case 2:
                    if (_majorSkillText2.text != "")
                    {
                        EmptySkillSlot(2);
                    }
                    _majorSkillText2.text = _selectedSkill;
                    break;
                case 3:
                    if (_majorSkillText3.text != "")
                    {
                        EmptySkillSlot(3);
                    }
                    _majorSkillText3.text = _selectedSkill;
                    break;
                case 4:
                    if (_majorSkillText4.text != "")
                    {
                        EmptySkillSlot(4);
                    }
                    _majorSkillText4.text = _selectedSkill;
                    break;
                case 5:
                    if (_minorSkillText1.text != "")
                    {
                        EmptySkillSlot(5);
                    }
                    _minorSkillText1.text = _selectedSkill;
                    break;
                case 6:
                    if (_minorSkillText2.text != "")
                    {
                        EmptySkillSlot(6);
                    }
                    _minorSkillText2.text = _selectedSkill;
                    break;
                case 7:
                    if (_minorSkillText3.text != "")
                    {
                        EmptySkillSlot(7);
                    }
                    _minorSkillText3.text = _selectedSkill;
                    break;
                case 8:
                    if (_minorSkillText4.text != "")
                    {
                        EmptySkillSlot(8);
                    }
                    _minorSkillText4.text = _selectedSkill;
                    break;
            }
            selectedSkills.Add(_selectedSkill);
        }
        else if (_selectedSkill != "" && selectedSkills.Contains(_selectedSkill))
        {
            switch (selectedSlot)
            {
                case 1:
                    EmptySkillSlot(1);
                    break;
                case 2:
                    EmptySkillSlot(2);
                    break;
                case 3:
                    EmptySkillSlot(3);
                    break;
                case 4:
                    EmptySkillSlot(4);
                    break;
                case 5:
                    EmptySkillSlot(5);
                    break;
                case 6:
                    EmptySkillSlot(6);
                    break;
                case 7:
                    EmptySkillSlot(7);
                    break;
                case 8:
                    EmptySkillSlot(8);
                    break;
            }
        }
    }

    public void EmptySkillSlot(int selectedSlot)
    {
        switch (selectedSlot)
        {
            case 1:
                selectedSkills.Remove(_majorSkillText1.text);
                _majorSkillText1.text = "";
                break;
            case 2:
                selectedSkills.Remove(_majorSkillText2.text);
                _majorSkillText2.text = "";
                break;
            case 3:
                selectedSkills.Remove(_majorSkillText3.text);
                _majorSkillText3.text = "";
                break;
            case 4:
                selectedSkills.Remove(_majorSkillText4.text);
                _majorSkillText4.text = "";
                break;
            case 5:
                selectedSkills.Remove(_minorSkillText1.text);
                _minorSkillText1.text = "";
                break;
            case 6:
                selectedSkills.Remove(_minorSkillText2.text);
                _minorSkillText2.text = "";
                break;
            case 7:
                selectedSkills.Remove(_minorSkillText3.text);
                _minorSkillText3.text = "";
                break;
            case 8:
                selectedSkills.Remove(_minorSkillText4.text);
                _minorSkillText4.text = "";
                break;
        }


    }

    private void PreventAbilityOverflow(string ability)
    {
        switch (ability)
        {
            case "physique":
                if (_physique > 10)
                {
                    _physique = 10;
                    _abilityPointsLeft++;
                }
                else if (_physique < 0)
                {
                    _physique = 0;
                    _abilityPointsLeft--;
                }
                break;
            case "agility":
                if (_agility > 10)
                {
                    _agility = 10;
                    _abilityPointsLeft++;
                }
                else if (_agility < 0)
                {
                    _agility = 0;
                    _abilityPointsLeft--;
                }
                break;
            case "mental":
                if (_mental > 10)
                {
                    _mental = 10;
                    _abilityPointsLeft++;
                }
                else if (_mental < 0)
                {
                    _mental = 0;
                    _abilityPointsLeft--;
                }
                break;
            case "social":
                if (_social > 10)
                {
                    _social = 10;
                    _abilityPointsLeft++;
                }
                else if (_social < 0)
                {
                    _social = 0;
                    _abilityPointsLeft--;
                }
                break;
        }

        _physiqueText.text = _physique.ToString();
        _agilityText.text = _agility.ToString();
        _mentalText.text = _mental.ToString();
        _socialText.text = _social.ToString();

        _abilityPointsLeftText.text = "Points Left: " + _abilityPointsLeft.ToString();
    }
    public void CheckCreationComplete()
    {
        if(selectedSkills.Count == 8 && _abilityPointsLeft == 0)
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
        _playerCharacterSheet.abilities.body = _physique;
        _playerCharacterSheet.abilities.hands = _agility;
        _playerCharacterSheet.abilities.mind = _mental;
        _playerCharacterSheet.abilities.heart = _social;
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
