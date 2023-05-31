using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEditor;
using Udar.SceneField;

public class PlayerCharacterCreation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] SceneField _startingScene;

    //All the basic items all characters should have
    [SerializeField] List<Item> _defaultItems = new();
    [SerializeField] Item _defaultShirt;
    [SerializeField] Item _defaultPants;
    [SerializeField] Item _defaultShoes;

    [SerializeField] List<Item> _backgroundListBanesman = new();
    [SerializeField] List<Item> _backgroundListDiplomat = new();
    [SerializeField] List<Item> _backgroundListHero = new();
    [SerializeField] List<Item> _backgroundListScout = new();
    [SerializeField] List<Item> _backgroundListScholar = new();
    [SerializeField] List<Item> _backgroundListTrader = new();
    [SerializeField] List<Item> _backgroundListWarrior = new();

    [SerializeField] int _startingHour;

    [SerializeField] Color _playerSkintone;

    [Header("References")]
    private PlayerDataTracker _playerDataTracker = new();

    //Bio
    private string _characterName;
    private CharacterPronouns _characterPronouns;

    //Looks tab
    [SerializeField] List<Sprite> _hairOptions = new();
    [SerializeField] List<Sprite> _beardOptions = new();
    [SerializeField] List<Color> _hairColourOptions = new();

    [SerializeField] Image _hairImage;
    [SerializeField] Image _beardImage;

    private int _onHair = 0;
    private int _onBeard = 0;
    private int _onColour = 0;

    //Background
    [SerializeField] TextMeshProUGUI _backgroundDescription;
    private List<Item> backgroundListFinal = new();
    private string _selectedBackground;

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

    [SerializeField] List<Skill> _availableSkills = new();

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

    public void SetName(TMP_InputField inputField)
    {
        _characterName = inputField.text;
    }

    public void SetPronouns(string pronouns)
    {
        switch (pronouns)
        {
            case "he":
                _characterPronouns = CharacterPronouns.He;
                break;
            case "she":
                _characterPronouns = CharacterPronouns.She;
                break;
            case "they":
                _characterPronouns = CharacterPronouns.They;
                break;
        }
    }

    public void CycleHair(bool cycleUp)
    {
        if (cycleUp)
        {
            _onHair++;

            if (_onHair > _hairOptions.Count - 1)
            {
                _onHair = 0;
            }
        }
        else
        {
            _onHair--;

            if (_onHair < 0)
            {
                _onHair = _hairOptions.Count - 1;
            }
        }

        if (_hairOptions[_onHair] != null)
        {
            _hairImage.sprite = _hairOptions[_onHair];
            _hairImage.color = _hairColourOptions[_onColour];
        }
        else
        {
            _hairImage.color = Color.clear;
        }
    }

    public void CycleBeard(bool cycleUp)
    {
        if (cycleUp)
        {
            _onBeard++;

            if (_onBeard > _beardOptions.Count - 1)
            {
                _onBeard = 0;
            }
        }
        else
        {
            _onBeard--;

            if (_onBeard < 0)
            {
                _onBeard = _beardOptions.Count - 1;
            }
        }

        if (_hairOptions[_onBeard] != null)
        {
            _beardImage.sprite = _beardOptions[_onBeard];
            _beardImage.color = _hairColourOptions[_onColour];
        }
        else
        {
            _beardImage.color = Color.clear;
        }
    }

    public void CycleColour(bool cycleUp)
    {
        if (cycleUp)
        {
            _onColour++;

            if (_onColour > _hairColourOptions.Count - 1)
            {
                _onColour = 0;
            }
        }
        else
        {
            _onColour--;

            if (_onColour < 0)
            {
                _onColour = _hairColourOptions.Count - 1;
            }
        }

        if (_hairOptions[_onHair] != null)
        {
            _hairImage.color = _hairColourOptions[_onColour];
        }
        else
        {
            _hairImage.color = Color.clear;
        }

        if (_hairOptions[_onBeard] != null)
        {
            _beardImage.color = _hairColourOptions[_onColour];
        }
        else
        {
            _beardImage.color = Color.clear;
        }
    }

    public void GetBackgroundDescription(string backgroundName)
    {
        switch (backgroundName)
        {
            case "Banesman":
                _backgroundDescription.text = "You had a reputation as a scoundrel, playing your hands at dirty trading and political manoeuvring which earned you few friends but some influence behind the scenes, and more wealth than a minor noble's younger child would have. Your parents sent you on the voyage more to avoid future scandal than any belief you would be of benefit.";
                break;
            case "Diplomat":
                _backgroundDescription.text = "You have always had a keen interest in the wider world and its people, from learning languages to chatting with foreign traders in the marketplace. Your family, feeling embarrassed by your keen interests, hoped the hardship of a long voyage would get you to focus on things back home.";
                break;
            case "Hero":
                _backgroundDescription.text = "Falling out of trees, play-fighting in the streets and putting yourself in harm's way at every opportunity, your family was not keen to see you put yourself in real danger on a long expedition, but your begging was convincing enough in the end. This is your chance to prove yourself as the true hero you always saw yourself as.";
                break;
            case "Scout":
                _backgroundDescription.text = "Going on a long expedition would hardly affect your family, who barely saw you while you lived in the city. The wilderness was your home, on hikes, hunts and week long camps by yourself. Due to this love of the outdoors you always failed to make friends, and at least your family hoped being stuck in a boat might help your social life.";
                break;
            case "Scholar":
                _backgroundDescription.text = "From a young age you found your true passion in the clay tablets and scrolls of your parents library, learning about the world and mastering letters. Your decision to strike out across the world may have been partially influenced by the fact you ran out of things to read and learn, and your father was happy to see you leave the house, even if that was on a dangerous expedition.";
                break;
            case "Trader":
                _backgroundDescription.text = "Throughout your life you never found greater pleasure than in the marketplace, figuring out how to exchange modest goods into treasures through bartering. While your family never appreciated the advice you gave on how they could grow the fortune of your noble house, you hope a long expedition would give you the chance to prove your skills, and maybe grab some unique items along the way.";
                break;
            case "Warrior":
                _backgroundDescription.text = "While a noble child is expected to learn the craft of war and honour duelling, your keen interest on things sharp and dangerous at times scared your family and even your weapons instructor. You took the first chance to get real experience and spill blood, and the expedition seemed perfect to master the art of fighting and killing.";
                break;
        }

        _selectedBackground = backgroundName;
    }

    public void SetBackground()
    {
        switch (_selectedBackground)
        {
            case "Banesman":
                backgroundListFinal = _backgroundListBanesman;
                break;
            case "Diplomat":
                backgroundListFinal = _backgroundListDiplomat;
                break;
            case "Hero":
                backgroundListFinal = _backgroundListHero;
                break;
            case "Scout":
                backgroundListFinal = _backgroundListScout;
                break;
            case "Scholar":
                backgroundListFinal = _backgroundListScholar;
                break;
            case "Trader":
                backgroundListFinal = _backgroundListTrader;
                break;
            case "Warrior":
                backgroundListFinal = _backgroundListWarrior;
                break;
            case null:
                return;
        }
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
        if (_selectedSkill == null)
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
        if (_characterName != "" && _abilityPointsLeft == 0 && _setSkill1 != null && _setSkill2 != null && _selectedBackground != "")
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
        //Bio
        _playerDataTracker.playerName = _characterName;
        _playerDataTracker.pronounInt = (int)_characterPronouns;

        //Looks
        _playerDataTracker.colorSkin = ColorUtility.ToHtmlStringRGBA(_playerSkintone);
        _playerDataTracker.colorHair = ColorUtility.ToHtmlStringRGBA(_hairColourOptions[_onColour]);
        if(_hairOptions[_onHair] != null) { _playerDataTracker.hairSprite = _hairOptions[_onHair].name; };
        if (_beardOptions[_onBeard] != null) { _playerDataTracker.beardSprite = _beardOptions[_onBeard].name; };
        

        //Make a final list of all items
        foreach (Item item in _defaultItems)
        {
            backgroundListFinal.Add(item);
        }

        //Add them to the inventory
        foreach(Item item in backgroundListFinal)
        {
            _playerDataTracker.currentInventory.Add(item.uniqueID);
        }

        _playerDataTracker.currentInventory.Add(_defaultShirt.uniqueID);
        _playerDataTracker.currentInventory.Add(_defaultPants.uniqueID);
        _playerDataTracker.currentInventory.Add(_defaultShoes.uniqueID);

        //Make the player where pants and a shirt
        _playerDataTracker.equippedShirt = _defaultShirt.uniqueID;
        _playerDataTracker.equippedPants = _defaultPants.uniqueID;
        _playerDataTracker.equippedFeet = _defaultShoes.uniqueID;


        //Abilities
        _playerDataTracker.bodyLevel = _body;
        _playerDataTracker.handsLevel = _hands;
        _playerDataTracker.mindLevel = _mind;
        _playerDataTracker.heartLevel = _heart;

        //Set the skills
        _playerDataTracker.currentSkills.Add(_setSkill1.uniqueID);
        _playerDataTracker.currentSkills.Add(_setSkill2.uniqueID);

        //Set the time
        _playerDataTracker.currentHour = _startingHour;

        //Save the tracker
        DataManager.instance.SetPlayerTracker(_playerDataTracker);

        SceneLoader.instance.LoadPlayerScene(_startingScene.BuildIndex, "default", Vector3.zero, Vector3.zero, false, true);
    }

    public void SetImageColourGreen(Image image)
    {
        image.color = Color.green;
    }

    public void SetImageColourRed(Image image)
    {
        image.color = Color.red;
    }
}
