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
    [SerializeField] TextMeshProUGUI _itemWeaponType;
    [SerializeField] TextMeshProUGUI _itemDamage;
    [SerializeField] TextMeshProUGUI _itemBlunt;
    [SerializeField] TextMeshProUGUI _itemDefence;
    [SerializeField] TextMeshProUGUI _itemRange;
    [SerializeField] TextMeshProUGUI _itemSpeed;

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
            _itemWeaponType.gameObject.SetActive(true);
            _itemDamage.gameObject.SetActive(true);
            _itemBlunt.gameObject.SetActive(true);
            _itemRange.gameObject.SetActive(true);
            _itemSpeed.gameObject.SetActive(true);
            _itemDefence.gameObject.SetActive(true);

            _itemWeaponType.text = "Weapon Type: " + (item as WeaponMeleeItem).weaponMeleeType;
            _itemDamage.text = "Damage: " + "D" + (item as WeaponMeleeItem).weaponDamage.ToString();
            _itemBlunt.text = "Blunt:" + (item as WeaponMeleeItem).weapontToHitBonus.ToString();
            _itemDefence.text = "Defence: " + (item as WeaponMeleeItem).weaponDefence.ToString("+#;-#;0");
            _itemRange.text = "Range: " + (item as WeaponMeleeItem).weaponRange.ToString();
            _itemSpeed.text = "Speed: " + (item as WeaponMeleeItem).weaponSpeed.ToString();

            SetButtonEvents("weapon", item, isSearch, itemContainer);
        }
        else if (item is WeaponRangedItem)
        {
            _itemWeaponType.gameObject.SetActive(true);
            _itemDamage.gameObject.SetActive(true);
            _itemSpeed.gameObject.SetActive(true);

            _itemWeaponType.text = "Weapon Type: " + (item as WeaponRangedItem).weaponRangedType;
            _itemDamage.text = "Damage: " + "D" + (item as WeaponRangedItem).weaponDamage.ToString();
            _itemSpeed.text = "Speed: " + (item as WeaponRangedItem).weaponSpeed.ToString();

            SetButtonEvents("weapon", item, isSearch, itemContainer);
        }
        else if (item is WeaponFocusItem)
        {
            _itemWeaponType.gameObject.SetActive(true);
            _itemDamage.gameObject.SetActive(true);
            _itemSpeed.gameObject.SetActive(true);

            _itemWeaponType.text = "Mind Required: " + (item as WeaponFocusItem).mindRequirement;
            _itemDamage.text = "Spell: " + (item as WeaponFocusItem).effectDescription;
            _itemSpeed.text = "Casting Speed: " + (item as WeaponFocusItem).castingSpeed.ToString();

            SetButtonEvents("weapon", item, isSearch, itemContainer);
        }
        else if (item is ShieldItem)
        {
            _itemDefence.gameObject.SetActive(true);
            _itemDefence.text = "Defence: " + (item as ShieldItem).shieldDefence.ToString("+#;-#;0");

            SetButtonEvents("shield", item, isSearch, itemContainer);
        }
        //If equipment
        else if (item is EquipmentItem)
        {
            SetButtonEvents("equipment", item, isSearch, itemContainer);
            _itemDefence.text = "Defence: " + (item as EquipmentItem).equipmentDefence.ToString("+#;-#;0");
        }
        else if (item is PotionItem)
        {
            SetButtonEvents("potion", item, isSearch, itemContainer);
        }
        else if (item is SpellItem)
        {
            _itemWeaponType.gameObject.SetActive(true);
            _itemWeaponType.text = "Mind Requirement: " + (item as SpellItem).spell.mindRequirement.ToString();

            SetButtonEvents("spell", item, isSearch, itemContainer);
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
        _itemWeaponType.gameObject.SetActive(false);
        _itemDamage.gameObject.SetActive(false);
        _itemBlunt.gameObject.SetActive(false);
        _itemRange.gameObject.SetActive(false);
        _itemSpeed.gameObject.SetActive(false);
        _itemDefence.gameObject.SetActive(false);

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

    public void CloseDescription()
    {
        ResetText();
        _descriptionParent.SetActive(false);
    }
}
