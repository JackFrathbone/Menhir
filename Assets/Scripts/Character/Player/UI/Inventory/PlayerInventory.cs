using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("References")]
    //Text headings that the items spawn under
    [SerializeField] GameObject _weaponParent;
    [SerializeField] GameObject _equipmentParent;
    [SerializeField] GameObject _potionParent;
    [SerializeField] GameObject _foodParent;
    [SerializeField] GameObject _writingParent;
    [SerializeField] GameObject _otherParent;

    //The button prefab that items are shown by
    [SerializeField] GameObject _InventoryButtonPrefab;

    //The buttons that show equipped items
    [SerializeField] Button _weaponButton;
    [SerializeField] Button _shieldButton;
    [SerializeField] Button _armourButton;
    [SerializeField] Button _capeButton;
    [SerializeField] Button _feetButton;
    [SerializeField] Button _greavesButton;
    [SerializeField] Button _handsButton;
    [SerializeField] Button _helmetButton;
    [SerializeField] Button _pantsButton;
    [SerializeField] Button _shirtButton;

    //Other inventory tabs
    [SerializeField] GameObject _searchView;
    [SerializeField] Transform _searchParent;

    //Prefab for dropping items
    [SerializeField] GameObject _itemContainerPrefab;

    private readonly List<GameObject> _buttonsToDelete = new();
    private readonly List<GameObject> _buttonsToDeleteSearch = new();

    private PlayerInventoryDescription _playerInventoryDescription;
    private PlayerCharacterManager _playerCharacterManager;

    //The character doll display
    //Base//
    [SerializeField] Transform _baseDisplayParent;
    [SerializeField] Image _hairDisplay;
    [SerializeField] Image _beardDisplay;

    //Equipment//
    [SerializeField] Image _capeDisplay;
    [SerializeField] Image _pantsDisplay;
    [SerializeField] Image _handsDisplay;
    [SerializeField] Image _shirtDisplay;
    [SerializeField] Image _armourDisplay;
    [SerializeField] Image _capeFrontDisplay;
    [SerializeField] Image _greavesDisplay;
    [SerializeField] Image _feetDisplay;
    [SerializeField] Image _helmetDisplay;

    private void Start()
    {
        _playerInventoryDescription = GetComponent<PlayerInventoryDescription>();

        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();

        foreach(Transform child in _baseDisplayParent)
        {
            Image image = child.GetComponent<Image>();
            if (image != null)
            {
                image.color = _playerCharacterManager.characterSheet.characterSkintone;
            }
        }

        if(_playerCharacterManager.characterSheet.characterHair != null)
        {
            _hairDisplay.sprite = _playerCharacterManager.characterSheet.characterHair;
            _hairDisplay.color = _playerCharacterManager.characterSheet.characterHairColor;
        }

        if (_playerCharacterManager.characterSheet.characterHair != null)
        {
            _beardDisplay.sprite = _playerCharacterManager.characterSheet.characterBeard;
            _beardDisplay.color = _playerCharacterManager.characterSheet.characterHairColor;
        }
    }

    public void RefreshInventory()
    {
        //Delete all the buttons
        for (int i = _buttonsToDelete.Count - 1; i >= 0; i--)
        {
            Destroy(_buttonsToDelete[i]);
        }

        _buttonsToDelete.Clear();

        List<Item> tempInventory = new(_playerCharacterManager.currentInventory);
        foreach (Item item in tempInventory)
        {
            PlayerInventoryButton button;
            if (item is WeaponMeleeItem)
            {
                if (item != _playerCharacterManager.equippedWeapon)
                {
                    button = Instantiate(_InventoryButtonPrefab, _weaponParent.transform.parent).GetComponent<PlayerInventoryButton>();
                    button.transform.SetSiblingIndex(_weaponParent.transform.GetSiblingIndex() + 1);
                    button.SetItem(item, this, false, null);

                    _buttonsToDelete.Add(button.gameObject);
                }
            }
            else if (item is WeaponRangedItem)
            {
                if (item != _playerCharacterManager.equippedWeapon)
                {
                    button = Instantiate(_InventoryButtonPrefab, _weaponParent.transform.parent).GetComponent<PlayerInventoryButton>();
                    button.transform.SetSiblingIndex(_weaponParent.transform.GetSiblingIndex() + 1);
                    button.SetItem(item, this, false, null);

                    _buttonsToDelete.Add(button.gameObject);
                }
            }
            else if (item is WeaponFocusItem)
            {
                if (item != _playerCharacterManager.equippedWeapon)
                {
                    button = Instantiate(_InventoryButtonPrefab, _weaponParent.transform.parent).GetComponent<PlayerInventoryButton>();
                    button.transform.SetSiblingIndex(_weaponParent.transform.GetSiblingIndex() + 1);
                    button.SetItem(item, this, false, null);

                    _buttonsToDelete.Add(button.gameObject);
                }
            }
            else if (item is ShieldItem)
            {
                if (item != _playerCharacterManager.equippedShield)
                {
                    button = Instantiate(_InventoryButtonPrefab, _equipmentParent.transform.parent).GetComponent<PlayerInventoryButton>();
                    button.transform.SetSiblingIndex(_equipmentParent.transform.GetSiblingIndex() + 1);
                    button.SetItem(item, this, false, null);

                    _buttonsToDelete.Add(button.gameObject);
                }
            }
            else if (item is EquipmentItem)
            {
                if (item != _playerCharacterManager.equippedArmour && item != _playerCharacterManager.equippedCape && item != _playerCharacterManager.equippedFeet && item != _playerCharacterManager.equippedGreaves && item != _playerCharacterManager.equippedHands && item != _playerCharacterManager.equippedHelmet && item != _playerCharacterManager.equippedPants && item != _playerCharacterManager.equippedShirt)
                {
                    button = Instantiate(_InventoryButtonPrefab, _equipmentParent.transform.parent).GetComponent<PlayerInventoryButton>();
                    button.transform.SetSiblingIndex(_equipmentParent.transform.GetSiblingIndex() + 1);
                    button.SetItem(item, this, false, null);

                    _buttonsToDelete.Add(button.gameObject);
                }
            }
            else if (item is PotionItem)
            {
                button = Instantiate(_InventoryButtonPrefab, _potionParent.transform.parent).GetComponent<PlayerInventoryButton>();
                button.transform.SetSiblingIndex(_potionParent.transform.GetSiblingIndex() + 1);
                button.SetItem(item, this, false, null);

                _buttonsToDelete.Add(button.gameObject);
            }
            else
            {
                button = Instantiate(_InventoryButtonPrefab, _otherParent.transform.parent).GetComponent<PlayerInventoryButton>();
                button.transform.SetSiblingIndex(_otherParent.transform.GetSiblingIndex() + 1);
                button.SetItem(item, this, false, null);

                _buttonsToDelete.Add(button.gameObject);
            }
        }
    }

    public void OpenSearchInventory(ItemContainer itemContainer)
    {
        _searchView.SetActive(true);

        //Delete all the buttons
        for (int i = _buttonsToDeleteSearch.Count - 1; i >= 0; i--)
        {
            Destroy(_buttonsToDeleteSearch[i]);
        }

        _buttonsToDeleteSearch.Clear();

        List<Item> tempInventory = new(itemContainer.inventory);
        foreach (Item item in tempInventory)
        {
            PlayerInventoryButton button = Instantiate(_InventoryButtonPrefab, _searchParent.parent).GetComponent<PlayerInventoryButton>();
            button.transform.SetSiblingIndex(_searchParent.transform.GetSiblingIndex() + 1);
            button.SetItem(item, this, true, itemContainer);

            _buttonsToDeleteSearch.Add(button.gameObject);
        }
    }

    public void CloseSearchInventory()
    {
        _searchView.SetActive(false);

        //Delete all the buttons
        for (int i = _buttonsToDeleteSearch.Count - 1; i >= 0; i--)
        {
            Destroy(_buttonsToDeleteSearch[i]);
        }

        _buttonsToDeleteSearch.Clear();
    }

    public void DropItem(Item i)
    {
        _playerCharacterManager.RemoveItem(i);

        ItemContainer itemContainer = Instantiate(_itemContainerPrefab, _playerCharacterManager.GetComponentInChildren<CharacterController>().transform.position ,Quaternion.identity).GetComponent<ItemContainer>();
        itemContainer.inventory.Add(i);

        RefreshInventory();
    }

    public void RefreshEquippedItemsDisplay()
    {
        RefreshInventory();

        //If something is equipped, display it in the inventory display, otherwise hide the slots
        if (_playerCharacterManager.equippedWeapon != null)
        {
            _weaponButton.gameObject.SetActive(true);

            _weaponButton.onClick.RemoveAllListeners();

            _weaponButton.image.sprite = _playerCharacterManager.equippedWeapon.itemIcon;
            _weaponButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedWeapon, false, null); });
        }
        else
        {
            _weaponButton.gameObject.SetActive(false);
        }

        if (_playerCharacterManager.equippedShield != null)
        {
            _shieldButton.gameObject.SetActive(true);

            _shieldButton.onClick.RemoveAllListeners();

            _shieldButton.image.sprite = _playerCharacterManager.equippedShield.itemIcon;
            _shieldButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedShield, false, null); });
        }
        else
        {
            _shieldButton.gameObject.SetActive(false);
        }

        if(_playerCharacterManager.equippedArmour != null)
        {
            _armourButton.gameObject.SetActive(true);

            _armourButton.onClick.RemoveAllListeners();
            _armourButton.image.sprite = _playerCharacterManager.equippedArmour.itemIcon;
            _armourButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedArmour, false, null); });

            _armourDisplay.sprite = _playerCharacterManager.equippedArmour.equipmentModel;
            _armourDisplay.color = _playerCharacterManager.equippedArmour.equipmentColor;
        }
        else
        {
            _armourButton.gameObject.SetActive(false);
            _armourDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedCape != null)
        {
            _capeButton.gameObject.SetActive(true);

            _capeButton.onClick.RemoveAllListeners();
            _capeButton.image.sprite = _playerCharacterManager.equippedCape.itemIcon;
            _capeButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedCape, false, null); });

            _capeDisplay.sprite = _playerCharacterManager.equippedCape.equipmentModel;
            _capeDisplay.color = _playerCharacterManager.equippedCape.equipmentColor;
        }
        else
        {
            _capeButton.gameObject.SetActive(false);
            _capeDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedFeet != null)
        {
            _feetButton.gameObject.SetActive(true);

            _feetButton.onClick.RemoveAllListeners();
            _feetButton.image.sprite = _playerCharacterManager.equippedFeet.itemIcon;
            _feetButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedFeet, false, null); });

            _feetDisplay.sprite = _playerCharacterManager.equippedFeet.equipmentModel;
            _feetDisplay.color = _playerCharacterManager.equippedFeet.equipmentColor;
        }
        else
        {
            _feetButton.gameObject.SetActive(false);
            _feetDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedGreaves != null)
        {
            _greavesButton.gameObject.SetActive(true);

            _greavesButton.onClick.RemoveAllListeners();
            _greavesButton.image.sprite = _playerCharacterManager.equippedGreaves.itemIcon;
            _greavesButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedGreaves, false, null); });

            _greavesDisplay.sprite = _playerCharacterManager.equippedGreaves.equipmentModel;
            _greavesDisplay.color = _playerCharacterManager.equippedGreaves.equipmentColor;
        }
        else
        {
            _greavesButton.gameObject.SetActive(false);
            _greavesDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedHands != null)
        {
            _handsButton.gameObject.SetActive(true);

            _handsButton.onClick.RemoveAllListeners();
            _handsButton.image.sprite = _playerCharacterManager.equippedHands.itemIcon;
            _handsButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedHands, false, null); });

            _handsDisplay.sprite = _playerCharacterManager.equippedHands.equipmentModel;
            _handsDisplay.color = _playerCharacterManager.equippedHands.equipmentColor;
        }
        else
        {
            _handsButton.gameObject.SetActive(false);
            _handsDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedHelmet != null)
        {
            _helmetButton.gameObject.SetActive(true);

            _helmetButton.onClick.RemoveAllListeners();
            _helmetButton.image.sprite = _playerCharacterManager.equippedHelmet.itemIcon;
            _helmetButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedHelmet, false, null); });

            _helmetDisplay.sprite = _playerCharacterManager.equippedHelmet.equipmentModel;
            _helmetDisplay.color = _playerCharacterManager.equippedHelmet.equipmentColor;
        }
        else
        {
            _helmetButton.gameObject.SetActive(false);
            _helmetDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedPants != null)
        {
            _pantsButton.gameObject.SetActive(true);

            _pantsButton.onClick.RemoveAllListeners();
            _pantsButton.image.sprite = _playerCharacterManager.equippedPants.itemIcon;
            _pantsButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedPants, false, null); });

            _pantsDisplay.sprite = _playerCharacterManager.equippedPants.equipmentModel;
            _pantsDisplay.color = _playerCharacterManager.equippedPants.equipmentColor;
        }
        else
        {
            _pantsButton.gameObject.SetActive(false);
            _pantsDisplay.color = Color.clear;
        }

        if (_playerCharacterManager.equippedShirt != null)
        {
            _shirtButton.gameObject.SetActive(true);

            _shirtButton.onClick.RemoveAllListeners();
            _shirtButton.image.sprite = _playerCharacterManager.equippedShirt.itemIcon;
            _shirtButton.onClick.AddListener(delegate { _playerInventoryDescription.SetDescription(_playerCharacterManager.equippedShirt, false, null); });

            _shirtDisplay.sprite = _playerCharacterManager.equippedShirt.equipmentModel;
            _shirtDisplay.color = _playerCharacterManager.equippedShirt.equipmentColor;
        }
        else
        {
            _shirtButton.gameObject.SetActive(false);
            _shirtDisplay.color = Color.clear;
        }
    }
}
