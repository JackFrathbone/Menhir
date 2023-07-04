using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInventoryDescription : MonoBehaviour
{
    [SerializeField] GameObject _descriptionParent;

    [Header("Description")]
    [SerializeField] TextMeshProUGUI _itemLabel;
    [SerializeField] TextMeshProUGUI _itemDescription;

    [Header("Stats")]
    [SerializeField] Transform _attributeTextParent;
    [SerializeField] GameObject _attributeTextBoxPrefab;

    [Header("Action Buttons")]
    [SerializeField] Button _buttonUse;
    [SerializeField] Button _buttonEquip;
    [SerializeField] Button _buttonUnequip;
    [SerializeField] Button _buttonDrop;
    [SerializeField] Button _buttonTake;

    private PlayerCharacterManager _playerCharacterManager;
    private PlayerInventory _playerInventory;
    private PlayerMagic _playerMagic;

    private void Start()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _playerMagic = GetComponent<PlayerMagic>();

        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();

    }

    public void SetDescription(Item item, bool isSearch, ItemContainer itemContainer)
    {
        ResetText();

        _descriptionParent.SetActive(true);

        //Base item
        _itemLabel.text = item.itemName;
        _itemDescription.text = item.itemDescription;

        //If melee weapon
        if (item is WeaponMeleeItem)
        {
            CreateDescriptionAttributeBox("Weapon Type: " + (item as WeaponMeleeItem).weaponMeleeType);
            CreateDescriptionAttributeBox("Damage: " + "D" + (item as WeaponMeleeItem).weaponDamage.ToString());
            CreateDescriptionAttributeBox("To Hit Bonus: " + (item as WeaponMeleeItem).weapontToHitBonus.ToString() + "%");
            CreateDescriptionAttributeBox("Defence: " + (item as WeaponMeleeItem).weaponDefence.ToString("+#;-#;0") + "%");
            CreateDescriptionAttributeBox("Range: " + (item as WeaponMeleeItem).weaponRange.ToString());
            CreateDescriptionAttributeBox("Speed: " + (item as WeaponMeleeItem).weaponSpeed.ToString());

            SetButtonEvents("weapon", item, isSearch, itemContainer);
        }
        else if (item is WeaponRangedItem)
        {
            CreateDescriptionAttributeBox("Weapon Type: " + (item as WeaponRangedItem).weaponRangedType);
            CreateDescriptionAttributeBox("Damage: " + "D" + (item as WeaponRangedItem).weaponDamage.ToString());
            CreateDescriptionAttributeBox("Speed: " + (item as WeaponRangedItem).weaponSpeed.ToString());

            SetButtonEvents("weapon", item, isSearch, itemContainer);
        }
        else if (item is WeaponFocusItem)
        {
            CreateDescriptionAttributeBox("Mind Required: " + (item as WeaponFocusItem).mindRequirement);
            CreateDescriptionAttributeBox((item as WeaponFocusItem).GetEffectsDescription());
            CreateDescriptionAttributeBox("Casting Speed: " + (item as WeaponFocusItem).castingSpeed.ToString());

            SetButtonEvents("weapon", item, isSearch, itemContainer);
        }
        else if (item is ShieldItem)
        {
            CreateDescriptionAttributeBox("Defence: " + (item as ShieldItem).shieldDefence.ToString("+#;-#;0"));
            CreateDescriptionAttributeBox("Magic Resist: " + (item as ShieldItem).magicResist.ToString("+#;-#;0"));

            SetButtonEvents("shield", item, isSearch, itemContainer);
        }
        //If equipment
        else if (item is EquipmentItem)
        {
            SetButtonEvents("equipment", item, isSearch, itemContainer);

            CreateDescriptionAttributeBox("Defence: " + (item as EquipmentItem).equipmentDefence.ToString("+#;-#;0"));
            CreateDescriptionAttributeBox("Magic Resist: " + (item as EquipmentItem).magicResist.ToString("+#;-#;0"));
        }
        else if (item is PotionItem)
        {
            CreateDescriptionAttributeBox((item as PotionItem).GetEffectsDescription());
            SetButtonEvents("potion", item, isSearch, itemContainer);
        }
        else if (item is SpellItem)
        {
            SetButtonEvents("spell", item, isSearch, itemContainer);

            CreateDescriptionAttributeBox("Mind Requirement: " + (item as SpellItem).spell.mindRequirement.ToString());
        }
        else
        {
            SetButtonEvents("item", item, isSearch, itemContainer);
        }
    }

    //Used to enable the equip or unequip buttons, and to decide what they link to
    private void SetButtonEvents(string itemType, Item item, bool isSearch, ItemContainer itemContainer)
    {
        if (isSearch)
        {
            _buttonTake.gameObject.SetActive(true);
            _buttonDrop.gameObject.SetActive(false);
            _buttonTake.onClick.AddListener(delegate { _playerCharacterManager.AddItem(item); });
            _buttonTake.onClick.AddListener(delegate { itemContainer.RemoveItem(item); });
            _buttonTake.onClick.AddListener(delegate { _playerInventory.OpenSearchInventory(itemContainer); });
            _buttonTake.onClick.AddListener(CloseDescription);
            return;
        }
        else
        {
            _buttonDrop.onClick.AddListener(delegate { _playerInventory.DropItem(item); });
            _buttonDrop.onClick.AddListener(CloseDescription);
        }

        if (!_playerCharacterManager.CheckItemEquipStatus(item) && ((item is WeaponMeleeItem) || (item is WeaponRangedItem) || (item is WeaponFocusItem) || (item is EquipmentItem) || (item is ShieldItem)))
        {
            //If the item is a focus, dont display equip if your mind isnt high enough
            if (item is WeaponFocusItem && _playerCharacterManager.abilities.mind < (item as WeaponFocusItem).mindRequirement)
            {
                return;
            }

            _buttonEquip.gameObject.SetActive(true);
            _buttonEquip.onClick.AddListener(delegate { _playerCharacterManager.EquipItem(itemType, item); });
            _buttonEquip.onClick.AddListener(_playerInventory.RefreshEquippedItemsDisplay);
            _buttonEquip.onClick.AddListener(CloseDescription);
        }
        else if (_playerCharacterManager.CheckItemEquipStatus(item) && ((item is WeaponMeleeItem) || (item is WeaponRangedItem) || (item is WeaponFocusItem) || (item is EquipmentItem) || (item is ShieldItem)))
        {
            _buttonUnequip.gameObject.SetActive(true);
            _buttonUnequip.onClick.AddListener(delegate { _playerCharacterManager.UnequipItem(itemType, item); });
            _buttonUnequip.onClick.AddListener(_playerInventory.RefreshEquippedItemsDisplay);
            _buttonUnequip.onClick.AddListener(CloseDescription);
        }
        else if (item is PotionItem)
        {
            _buttonUse.gameObject.SetActive(true);
            _buttonUse.onClick.AddListener(delegate { _playerCharacterManager.UsePotionItem(item as PotionItem); _playerInventory.RefreshInventory(); });
            _buttonUse.onClick.AddListener(CloseDescription);
        }
        else if (item is SpellItem)
        {
            _buttonUse.gameObject.SetActive(true);
            _buttonUse.onClick.AddListener(delegate { _playerCharacterManager.currentSpells.Add((item as SpellItem).spell); _playerCharacterManager.RemoveItem(item); _playerInventory.RefreshInventory(); _playerMagic.RefreshSpells(); });
            _buttonUse.onClick.AddListener(CloseDescription);
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
        _buttonEquip.gameObject.SetActive(false);
        _buttonUnequip.gameObject.SetActive(false);
        _buttonTake.gameObject.SetActive(false);

        //Brings back the drop button if it was disabled
        _buttonDrop.gameObject.SetActive(true);

        //Remove the button evenets from the buttons
        _buttonUse.onClick.RemoveAllListeners();
        _buttonEquip.onClick.RemoveAllListeners();
        _buttonUnequip.onClick.RemoveAllListeners();
        _buttonDrop.onClick.RemoveAllListeners();
        _buttonTake.onClick.RemoveAllListeners();

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
