using System.Collections.Generic;
using Udar.SceneField;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterManager : CharacterManager
{
    [Header("Defaults")]
    [SerializeField] Faction _playerFaction;

    [Header("Dialogue")]
    //Uses dialogue IDs
    [ReadOnly] public List<string> alreadyRunDialogueTopics;

    [Header("Visuals")]
    public Color characterSkintone = Color.white;
    public Color characterHairColor = Color.black;
    public Sprite characterHair;
    public Sprite characterBeard;

    [Header("Visual References")]
    [SerializeField] SpriteRenderer _playerWeaponMelee;
    [SerializeField] SpriteRenderer _playerWeaponRanged;
    [SerializeField] SpriteRenderer _playerShield;

    //Player specific options
    [ReadOnly] public bool weaponOut = false;

    //Update player UI
    private PlayerActiveUI _playerActiveUI;
    private PlayerCharacterStatsDisplay _PlayerCharacterStatsDisplay;

    //Active Display Icon for equipped weapons
    [SerializeField] Image _activeUIWeaponIcon;
    [SerializeField] Sprite _activeUIEmptyIcon;

    //The global state checks list, used to see if quests and dialogue has occured
    [ReadOnly] public List<StateCheck> stateChecks = new();

    //References
    private PlayerCombat _PlayerCombat;
    private PlayerController _playerController;
    [SerializeField] PlayerMagic _playerMagic;
    private PlayerInventory _playerInventory;

    [Header("Misc Refs")]
    [ReadOnly] public List<JournalEntry> journalEntries = new();

    [Header("Misc Refs")]
    [SerializeField] SceneField _deathScene;

    [Header("Debug")]
    [SerializeField] bool _godMode;

    protected override void Awake()
    {
        //Set the player defaults
        characterFaction = _playerFaction;

        base.Awake();

        _PlayerCombat = GetComponent<PlayerCombat>();
        _playerController = GetComponent<PlayerController>();
        _playerInventory = _playerMagic.GetComponent<PlayerInventory>();
    }

    protected override void Start()
    {
        _playerActiveUI = GameManager.instance.PlayerUIObject.GetComponent<PlayerActiveUI>();
        _PlayerCharacterStatsDisplay = GameManager.instance.PlayerUIObject.GetComponent<PlayerCharacterStatsDisplay>();
        //base.Start();
    }

    //Used when loading player data, to make all the related functions onyl start when everything is updated
    public void LoadPlayer()
    {
        SetCurrentStatus();
        InvokeRepeating(nameof(RunEffects), 0f, 1f);
        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);

        _playerInventory.Load();

        _playerInventory.RefreshEquippedItemsDisplay();
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);

        //Set natural effect resist from mind ability
        SetEffectResist(StatFormulas.MagicResistBonus(abilities.mind));
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
        if (i == null)
        {
            return;
        }

        LowerWeapon();

        switch (itemType)
        {
            case "weapon":
                equippedWeapon = i;
                //Set active UI Icon
                _activeUIWeaponIcon.sprite = equippedWeapon.itemIcon;

                //If weapon is two handed or ranged remove the shield
                if (equippedWeapon is WeaponMeleeItem)
                {
                    if ((equippedWeapon as WeaponMeleeItem).twoHanded)
                    {
                        equippedShield = null;
                    }

                    //Add enchantments
                    foreach (Effect effect in (equippedWeapon as WeaponMeleeItem).enchantmentSelfEffects)
                    {
                        //Set the effect to be permanent
                        effect.permanentEffect = true;
                        //Add to the character
                        AddEffect(effect);
                    }
                }
                else if (equippedWeapon is WeaponRangedItem)
                {
                    equippedShield = null;

                    //Add enchantments
                    foreach (Effect effect in (equippedWeapon as WeaponRangedItem).enchantmentSelfEffects)
                    {
                        //Set the effect to be permanent
                        effect.permanentEffect = true;
                        //Add to the character
                        AddEffect(effect);
                    }
                }
                break;
            case "shield":
                //Check if appropriate weapon is equipped and then lets the shield be added
                if (equippedWeapon is WeaponMeleeItem)
                {
                    if (!(equippedWeapon as WeaponMeleeItem).twoHanded)
                    {
                        equippedShield = (i as ShieldItem);

                        //Add enchantments
                        foreach (Effect effect in equippedShield.enchantmentEffects)
                        {
                            //Set the effect to be permanent
                            effect.permanentEffect = true;
                            //Add to the player
                            AddEffect(effect);
                        }
                    }
                }
                else
                {
                    Debug.Log("Tried equipping a shield with invalid weapon");
                }
                break;
            case "equipment":
                //Add magic resist
                EquipmentItem equipmentItem = (EquipmentItem)i;
                SetEffectResist(equipmentItem.magicResist);

                //Add enchantments
                foreach (Effect effect in equipmentItem.enchantmentEffects)
                {
                    //Set the effect to be permanent
                    effect.permanentEffect = true;
                    //Add to the player
                    AddEffect(effect);
                }

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
                _activeUIWeaponIcon.sprite = _activeUIEmptyIcon;

                if (equippedWeapon is WeaponMeleeItem)
                {
                    //Remove enchantments
                    foreach (Effect effect in (equippedWeapon as WeaponMeleeItem).enchantmentSelfEffects)
                    {
                        if (currentEffects.Contains(effect))
                        {
                            RemoveEffect(effect);
                        }
                    }
                }
                else if (equippedWeapon is WeaponRangedItem)
                {
                    //Remove enchantments
                    foreach (Effect effect in (equippedWeapon as WeaponRangedItem).enchantmentSelfEffects)
                    {
                        if (currentEffects.Contains(effect))
                        {
                            RemoveEffect(effect);
                        }
                    }
                }

                if (equippedShield != null)
                {
                    //Remove enchantments
                    foreach (Effect effect in equippedShield.enchantmentEffects)
                    {
                        if (currentEffects.Contains(effect))
                        {
                            RemoveEffect(effect);
                        }
                    }
                }

                //If taking away a weapon also remove the shield
                equippedWeapon = null;
                equippedShield = null;
                break;
            case "shield":
                equippedShield = null;
                break;
            case "equipment":
                //Remove the effect resist
                EquipmentItem equipmentItem = (EquipmentItem)i;
                SetEffectResist(-equipmentItem.magicResist);

                //Remove enchantments
                foreach (Effect effect in equipmentItem.enchantmentEffects)
                {
                    if (currentEffects.Contains(effect))
                    {
                        RemoveEffect(effect);
                    }
                }

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
                _playerWeaponMelee.sprite = (equippedWeapon as WeaponMeleeItem).weaponModel;
                twoHanded = (equippedWeapon as WeaponMeleeItem).twoHanded;
            }
            else if (equippedWeapon is WeaponRangedItem)
            {
                _playerWeaponRanged.sprite = (equippedWeapon as WeaponRangedItem).weaponModelLoaded;
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
        _playerWeaponMelee.sprite = null;
        _playerWeaponRanged.sprite = null;
        _playerShield.sprite = null;
        weaponOut = false;
    }

    public override void AddHealth(float i)
    {
        base.AddHealth(i);

        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);
    }

    public override void DamageHealth(int i, CharacterManager damageSource)
    {
        //Do nothing if paused
        if (GameManager.instance.isPaused)
        {
            return;
        }

        //if godmode then just dont take damage
        if (_godMode)
        {
            return;
        }

        //Instant kill blocker bool
        bool oneHealthLeft = false;

        //If the player has one health left they can be killed
        if(healthCurrent == 1)
        {
            oneHealthLeft = true;
        }

        base.DamageHealth(i, damageSource);

        //If the player loses all their health but had more than one health left, give them one health back so they don't die
        if(healthCurrent <= 0 && oneHealthLeft != true)
        {
            healthCurrent = 1;
            characterState = CharacterState.alive;
        }

        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);

        //If the player is dead then move them to the death screen
        if (characterState == CharacterState.dead || characterState == CharacterState.wounded)
        {
            GameManager.instance.UnlockCursor();
            SceneLoader.instance.LoadMenuScene(_deathScene.BuildIndex);
        }
    }

    public override void AddStamina(float i)
    {
        base.AddStamina(i);

        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);
    }

    public override void DamageStamina(float i)
    {
        base.DamageStamina(i);

        _playerActiveUI.UpdateStatusUI(healthCurrent, healthTotal, staminaCurrent, staminaTotal);
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);
    }

    public override void SetSlowState(bool isSlowed)
    {
        if (isSlowed)
        {
            _playerController.SlowMovement();
        }
        else
        {
            _playerController.NormalMovement();
        }
    }

    public override void SetParalyseState(bool isParalysed)
    {
        if (isParalysed)
        {
            _playerController.StopMovement();
        }
        else
        {
            _playerController.StartMovement();
        }
    }

    public override void SetCanAttack(bool canAttack)
    {
        _PlayerCombat.SetCanAttack(canAttack);
    }

    public override void TriggerBlock()
    {
        _PlayerCombat.TriggerBlock();
    }

    //Used for changing ranged sprite during combat
    public void SetRangedSprite(Sprite s)
    {
        _playerWeaponRanged.sprite = s;
    }

    public void AddJournalEntry(JournalEntry entry)
    {
        entry.timeStamp = TimeController.GetDays() + TimeController.GetHours() + TimeController.GetMinutes();
        entry.isArchived = false;
        journalEntries.Add(entry);
    }

    public override void AddSkill(Skill newSkill)
    {
        base.AddSkill(newSkill);
        _PlayerCharacterStatsDisplay.AddSkill(newSkill);
    }

    public PlayerMagic GetPlayerMagic()
    {
        return _playerMagic;
    }

    public override void ChangeAbilityTotal(int i, string abilityName)
    {
        base.ChangeAbilityTotal(i, abilityName);
        _PlayerCharacterStatsDisplay.UpdateStatDisplay(this);
    }

    public override void RemoveItem(Item i)
    {
        //Remove the item if player has it equipped first
        if (CheckItemEquipStatus(i))
        {
            if (i == equippedWeapon)
            {
                UnequipItem("weapon", i);
            }
            else if (i == equippedShield)
            {
                UnequipItem("shield", i);
            }
            else if (i is EquipmentItem)
            {
                UnequipItem("equipment", i);
            }
        }

        base.RemoveItem(i);
    }

    public override bool CheckSkill_SecondWind()
    {
        bool result = base.CheckSkill_SecondWind();

        if (result)
        {
            MessageBox.instance.Create("You gain a second wind and restore half your health!", true);
        }

        return result;
    }
}
