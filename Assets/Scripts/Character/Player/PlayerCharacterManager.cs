using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterManager : CharacterManager
{
    [SerializeField] SpriteRenderer _playerWeapon;
    [SerializeField] SpriteRenderer _playerShield;

    //Player specific options
    [HideInInspector]
    public bool weaponOut = false;
    [HideInInspector]
    public int weaponDamage;
    [HideInInspector]
    public float weaponSpeed;
    [HideInInspector]
    public float weaponRange;

    //Update player UI
    private PlayerActiveUI _playerActiveUI;
    private PlayerCharacterStatsDisplay _PlayerCharacterStatsDisplay;

    //The global state checks list, used to see if quests and dialogue has occured
    [HideInInspector]
    public List<StateCheck> stateChecks = new List<StateCheck>();

    //References
    private PlayerCombat _PlayerCombat;

    protected override void Awake()
    {
        base.Awake();
        _PlayerCombat = GetComponent<PlayerCombat>();
    }

    protected override void Start()
    {
        _playerActiveUI = GameManager.instance.PlayerUIObject.GetComponent<PlayerActiveUI>();
        _PlayerCharacterStatsDisplay = GameManager.instance.PlayerUIObject.GetComponent<PlayerCharacterStatsDisplay>();
        base.Start();
        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
    }

    protected override void RegenStamina()
    {
        base.RegenStamina();
        _playerActiveUI.UpdateStatusStaminaUI(staminaCurrent, staminaTotal);
    }

    public void ToggleWeapon()
    {
        if (weaponOut)
        {
            LowerWeapon();
        }
        else
        {
            RaiseWeapon();
        }
    }

    public bool CheckItemEquipStatus(Item i)
    {
        if (i == equippedArmour || i == equippedCape || i == equippedFeet || i == equippedGreaves || i == equippedHands || i == equippedHelmet || i == equippedPants || i == equippedShield || i == equippedShirt || i == equippedWeapon)
        {
            return true;
        }

        return false;
    }

    public void EquipItem(string itemType, Item i)
    {
        LowerWeapon();

        switch (itemType)
        {
            case "weapon":
                equippedWeapon = i;

                //If weapon is two handed or ranged remove the shield
                if (equippedWeapon is WeaponMeleeItem)
                {
                    weaponDamage = (equippedWeapon as WeaponMeleeItem).weaponDamage;
                    weaponSpeed = (equippedWeapon as WeaponMeleeItem).weaponSpeed;
                    weaponRange = (equippedWeapon as WeaponMeleeItem).weaponRange;
                    if ((equippedWeapon as WeaponMeleeItem).twoHanded)
                    {
                        equippedShield = null;
                    }
                }
                //Also set the weapon speed to 1 (default for all ranged weapons)
                else if (equippedWeapon is WeaponRangedItem)
                {
                    equippedShield = null;
                    weaponDamage = (equippedWeapon as WeaponRangedItem).weaponDamage;
                    weaponSpeed = 1f;
                    weaponRange = 1f;
                }
                break;
            case "shield":
                //Check if appropriate weapon is equipped and then lets the shield be added
                if (equippedWeapon is WeaponMeleeItem)
                {
                    if (!(equippedWeapon as WeaponMeleeItem).twoHanded)
                    {
                        equippedShield = i as ShieldItem;
                    }
                }
                else
                {
                    Debug.Log("Tried equipping a shield with invalid weapon");
                }
                break;
            case "equipment":
                EquipmentItem equipmentItem = (EquipmentItem)i;
                switch (equipmentItem.equipmentType)
                {
                    case EquipmentType.armour:
                        equippedArmour = equipmentItem;
                        break;
                    case EquipmentType.cape:
                        equippedCape = equipmentItem;
                        break;
                    case EquipmentType.feet:
                        equippedFeet = equipmentItem;
                        break;
                    case EquipmentType.greaves:
                        equippedGreaves = equipmentItem;
                        break;
                    case EquipmentType.hands:
                        equippedHands = equipmentItem;
                        break;
                    case EquipmentType.helmet:
                        equippedHelmet = equipmentItem;
                        break;
                    case EquipmentType.pants:
                        equippedPants = equipmentItem;
                        break;
                    case EquipmentType.shirt:
                        equippedShirt = equipmentItem;
                        break;
                }
                break;
        }
    }

    protected override void SetCurrentStatus()
    {
        base.SetCurrentStatus();
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);
    }

    public void UnequipItem(string itemType, Item i)
    {
        LowerWeapon();

        switch (itemType)
        {
            case "weapon":
                //If taking away a weapon also remove the shield
                equippedWeapon = null;
                equippedShield = null;
                break;
            case "shield":
                equippedShield = null;
                break;
            case "equipment":
                EquipmentItem equipmentItem = (EquipmentItem)i;
                switch (equipmentItem.equipmentType)
                {
                    case EquipmentType.armour:
                        equippedArmour = null;
                        break;
                    case EquipmentType.cape:
                        equippedCape = null;
                        break;
                    case EquipmentType.feet:
                        equippedFeet = null;
                        break;
                    case EquipmentType.greaves:
                        equippedGreaves = null;
                        break;
                    case EquipmentType.hands:
                        equippedHands = null;
                        break;
                    case EquipmentType.helmet:
                        equippedHelmet = null;
                        break;
                    case EquipmentType.pants:
                        equippedPants = null;
                        break;
                    case EquipmentType.shirt:
                        equippedShirt = null;
                        break;
                }
                break;
        }
    }

    private void RaiseWeapon()
    {
        if (equippedWeapon != null)
        {
            bool twoHanded = false;

            if (equippedWeapon is WeaponMeleeItem)
            {
                _playerWeapon.sprite = (equippedWeapon as WeaponMeleeItem).weaponModel;
                twoHanded = (equippedWeapon as WeaponMeleeItem).twoHanded;
            }
            else
            {
                _playerWeapon.sprite = (equippedWeapon as WeaponRangedItem).weaponModel;
                twoHanded = true;
            }

            if (equippedShield != null && !twoHanded)
            {
                _playerShield.sprite = (equippedShield as ShieldItem).shieldModel;
            }

            weaponOut = true;
        }
    }

    private void LowerWeapon()
    {
        _playerWeapon.sprite = null;
        _playerShield.sprite = null;
        weaponOut = false;
    }

    public override void DamageHealth(float i)
    {
        healthCurrent -= i;

        if (healthCurrent <= 0)
        {
            healthCurrent = 0;
        }

        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
    }

    public override void DamageStamina(float i)
    {
        if (characterState == CharacterState.alive)
        {
            staminaCurrent -= i;

            if (staminaCurrent <= 0)
            {
                staminaCurrent = 0;
            }
        }

        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
    }

    public override void TriggerBlock()
    {
        _PlayerCombat.TriggerBlock();
    }
}
