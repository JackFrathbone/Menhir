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
    [SerializeField] Button _buttonCraft;

    private PlayerMagic _playerMagic;

    private void Start()
    {
        _playerMagic = GetComponent<PlayerMagic>();
    }

    public void SetDescription(Spell spell)
    {
        //If the spell isnt a recipe then set it as the current spell to cast
        if (!spell.isRecipe)
        {
            _playerMagic.SetCurrentSpell(spell);
        }

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

        if (spell.castingCostItems.Count != 0 || spell.castingHealthCost > 0 || spell.castingStaminaCost > 0)
        {
            castingCost = "";

            if (spell.castingHealthCost > 0)
            {
                castingCost += "Takes " + spell.castingHealthCost + " health" + "\n";
            }

            if (spell.castingStaminaCost > 0)
            {
                castingCost += "Takes " + spell.castingStaminaCost + " stamina" + "\n";
            }

            foreach (Item castingItem in spell.castingCostItems)
            {
                castingCost += castingItem.itemName + "\n";
            }
        }
        CreateDescriptionAttributeBox("Spell Type: " + spellType);
        CreateDescriptionAttributeBox("Mind Requirement: " + spell.mindRequirement.ToString());
        CreateDescriptionAttributeBox("Casting Time: " + spell.castingTime.ToString() + " seconds");
        CreateDescriptionAttributeBox("<b>Casting Costs</b>");
        CreateDescriptionAttributeBox(castingCost);

        SetButtonEvents(spell);
    }

    //Used to enable the equip or unequip buttons, and to decide what they link to
    private void SetButtonEvents(Spell spell)
    {
        if (spell.isRecipe)
        {
            _buttonCraft.gameObject.SetActive(true);
            _buttonCraft.onClick.AddListener(delegate { _playerMagic.CraftRecipe(spell); });
            _buttonCraft.onClick.AddListener(CloseDescription);
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

        _buttonCraft.gameObject.SetActive(false);
        _buttonCraft.onClick.RemoveAllListeners();
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
