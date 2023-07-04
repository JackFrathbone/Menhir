using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMagicDescription : MonoBehaviour
{
    [SerializeField] GameObject _descriptionParent;

    [Header("Description")]
    [SerializeField] TextMeshProUGUI _spellLabel;
    [SerializeField] TextMeshProUGUI _spellDescription;

    [Header("Stats")]
    [SerializeField] Transform _attributeTextParent;
    [SerializeField] GameObject _attributeTextBoxPrefab;

    [Header("Action Buttons")]
    [SerializeField] Button _buttonUse;
    [SerializeField] Button _buttonPrepare;
    [SerializeField] Button _buttonCancel;
    [SerializeField] Button _buttonLearn;
    [SerializeField] Button _buttonForget;

    private PlayerMagic _playerMagic;

    private void Start()
    {
        _playerMagic = GetComponent<PlayerMagic>();
    }

    public void SetDescription(Spell spell)
    {
        ResetText();

        _descriptionParent.SetActive(true);

        //Base item
        _spellLabel.text = spell.spellName;
        _spellDescription.text = spell.GetEffectsDescription();

        string spellType = "Cast on Self";

        if (spell.castTarget)
        {
            spellType = "Cast on Target";
        }

        string castingCost = "No costs";

        if (spell.castingCostItems.Count != 0)
        {
            castingCost = "";

            foreach (Item castingItem in spell.castingCostItems)
            {
                castingCost += castingItem.itemName + "\n";
            }
        }
        CreateDescriptionAttributeBox("Spell Type: " + spellType);
        CreateDescriptionAttributeBox("Mind Requirement: " + spell.mindRequirement.ToString());
        CreateDescriptionAttributeBox("<b>Casting Costs</b>");
        CreateDescriptionAttributeBox(castingCost);

        SetButtonEvents(spell);
    }

    //Used to enable the equip or unequip buttons, and to decide what they link to
    private void SetButtonEvents(Spell spell)
    {

        if (_playerMagic.CheckSpellPrepared(spell))
        {
            _buttonPrepare.gameObject.SetActive(false);
            _buttonCancel.gameObject.SetActive(true);
            _buttonCancel.onClick.AddListener(delegate { _playerMagic.UnPrepareSpell(spell); });
            _buttonCancel.onClick.AddListener(CloseDescription);
        }
        else
        {
            _buttonPrepare.gameObject.SetActive(true);
            _buttonPrepare.onClick.AddListener(delegate { _playerMagic.PrepareSpell(spell); });
            _buttonPrepare.onClick.AddListener(CloseDescription);
        }

        if (_playerMagic.CheckSpellLearned(spell) && _playerMagic.CheckWarlockSkill())
        {
            _buttonForget.gameObject.SetActive(true);
            _buttonForget.onClick.AddListener(delegate { _playerMagic.UnLearnSpell(spell); });
            _buttonForget.onClick.AddListener(CloseDescription);
        }
        else if (!_playerMagic.CheckSpellLearned(spell) && _playerMagic.CheckWarlockSkill())
        {
            _buttonLearn.gameObject.SetActive(true);
            _buttonLearn.onClick.AddListener(delegate { _playerMagic.LearnSpell(spell); });
            _buttonLearn.onClick.AddListener(CloseDescription);
        }

    }

    //Hide all the gameobjects and the buttons
    private void ResetText()
    {
        //Clear all the attribute description boxes
        int childCount = _attributeTextParent.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = _attributeTextParent.transform.GetChild(i);
            Destroy(child.gameObject);
        }

        _buttonUse.gameObject.SetActive(false);
        _buttonPrepare.gameObject.SetActive(false);
        _buttonCancel.gameObject.SetActive(false);
        _buttonLearn.gameObject.SetActive(false);
        _buttonForget.gameObject.SetActive(false);

        //Remove the button evenets from the buttons
        _buttonUse.onClick.RemoveAllListeners();
        _buttonPrepare.onClick.RemoveAllListeners();
        _buttonLearn.onClick.RemoveAllListeners();
        _buttonForget.onClick.RemoveAllListeners();
    }

    private void CreateDescriptionAttributeBox(string text)
    {
        Instantiate(_attributeTextBoxPrefab, _attributeTextParent).GetComponent<TextMeshProUGUI>().text = text;
    }

    public void CloseDescription()
    {
        ResetText();
        _descriptionParent.SetActive(false);
    }
}
