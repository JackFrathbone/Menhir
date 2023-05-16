using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

//The non-player character manager is for managing human NPCs, with references for animation and AI
public class NonPlayerCharacterManager : CharacterManager
{
    [Header("Character Sheet")]
    [SerializeField] protected CharacterSheet _baseCharacterSheet;
    [HideInInspector] private CharacterSheet _characterSheet = null;

    [Header("Dialogue")]
    [ReadOnly] public string characterDescription;
    [ReadOnly] public string characterGreeting;
    [ReadOnly] public string characterWoundedGreeting;
    [ReadOnly] public DialogueGraph characterDialogueGraph;

    [Header("States")]
    [ReadOnly] public bool isHidden;

    [Header("Visuals")]
    [ReadOnly] public bool randomiseVisuals;
    [ReadOnly] public Color characterSkintone = Color.white;
    [ReadOnly] public Color characterHairColor = Color.black;
    [ReadOnly] public Sprite characterHair;
    [ReadOnly] public Sprite characterBeard;

    [Header("References")]
    private CharacterMovementController _characterMovementController;
    private CharacterAnimationController _animationController;
    private CharacterVisualUpdater _VisualUpdater;
    private CharacterCombatController _characterCombatController;

    protected override void Awake()
    {
        _characterSheet = Instantiate(_baseCharacterSheet);
        SetDataFromCharacterSheet();

        _animationController = GetComponentInChildren<CharacterAnimationController>();
        _VisualUpdater = GetComponentInChildren<CharacterVisualUpdater>();
        _characterCombatController = GetComponentInChildren<CharacterCombatController>();
        EquipItems();
    }

    protected override void Start()
    {
        base.Start();

        characterState = _characterSheet.startState;
        SetCharacterState();
        _characterMovementController = GetComponent<CharacterMovementController>();
    }


    private void OnEnable()
    {
        DataManager.instance.AddActiveCharacter(this);
    }

/*#if UNITY_STANDALONE
    private void OnDisable()
    {
        DataManager.instance.RemoveActiveCharacter(this);
    }

#endif*/

    private void OnValidate()
    {
        if (_baseCharacterSheet != null)
        {
            name = _baseCharacterSheet.characterName;
        }
    }

    private void SetDataFromCharacterSheet()
    {
        characterName = _characterSheet.characterName;
        characterDescription = _characterSheet.characterDescription;
        characterPronouns = _characterSheet.characterPronouns;
        characterFaction = _characterSheet.characterFaction;
        characterAggression = _characterSheet.characterAggression;

        currentInventory = new List<Item>(_characterSheet.characterInventory);
        currentSpells = new List<Spell>(_characterSheet.characterSpells);

        characterGreeting = _characterSheet.characterGreeting;
        characterWoundedGreeting = _characterSheet.characterWoundedGreeting;
        characterDialogueGraph = _characterSheet.characterDialogueGraph;

        abilities = _characterSheet.abilities;

        foreach (Skill skill in _characterSheet.skills)
        {
            AddSkill(skill);
        }

        isHidden = _characterSheet.startHidden;
        characterState = _characterSheet.startState;

        randomiseVisuals = _characterSheet.randomiseVisuals;
        characterSkintone = _characterSheet.characterSkintone;
        characterHairColor = _characterSheet.characterHairColor;
        characterHair = _characterSheet.characterHair;
        characterBeard = _characterSheet.characterBeard;
    }

    public override void SetCharacterState()
    {
        if (isHidden)
        {
            gameObject.SetActive(false);
            return;
        }

        switch (characterState)
        {
            case CharacterState.alive:
                _animationController.SetState(0);
                //Allows a healed wounded character to get back to their job
                GetComponentInChildren<CharacterMovementController>().enabled = true;
                GetComponentInChildren<CharacterCombatController>().enabled = true;
                GetComponentInChildren<NavMeshAgent>().enabled = true;
                GetComponentInChildren<CharacterAI>().enabled = true;
                break;
            case CharacterState.wounded:
                _animationController.SetState(1);
                GetComponentInChildren<CharacterMovementController>().enabled = false;
                GetComponentInChildren<CharacterCombatController>().enabled = false;
                GetComponentInChildren<NavMeshAgent>().enabled = false;
                GetComponentInChildren<CharacterAI>().enabled = false;
                break;
            //Is dead so set model and add a container for items
            case CharacterState.dead:
                _animationController.SetState(2);
                if (GetComponent<ItemContainer>() == null)
                {
                    ItemContainer newContainer = gameObject.AddComponent<ItemContainer>();
                    newContainer.SetInventory(currentInventory);
                }
                DestroyScripts();
                break;
        }
    }

    //Destroy scripts that still do stuff when dead
    private void DestroyScripts()
    {
        Destroy(GetComponent<CharacterMovementController>());
        Destroy(GetComponent<CharacterCombatController>());
        Destroy(GetComponent<CharacterAI>());
        Destroy(GetComponent<NavMeshAgent>());
    }

    //Equips the highest value items in inventory to equipment slots
    private void EquipItems()
    {
        EquipWeapon();
        EquipEquipment();
        _VisualUpdater.SetVisuals(this);
    }

    //Equips the most appropriate weapon and shield if needed
    private void EquipWeapon()
    {
        foreach (Item weapon in currentInventory)
        {
            if (weapon is WeaponMeleeItem || weapon is WeaponRangedItem || weapon is WeaponFocusItem)
            {
                equippedWeapon = weapon;

                if (weapon is WeaponMeleeItem && !(weapon as WeaponMeleeItem).twoHanded)
                {
                    _animationController.SetEquipType(1);
                }
                else if (weapon is WeaponMeleeItem && (weapon as WeaponMeleeItem).twoHanded)
                {
                    _animationController.SetEquipType(2);
                }
                else if (weapon is WeaponRangedItem)
                {
                    _animationController.SetEquipType(3);
                }
                else if (weapon is WeaponFocusItem)
                {
                    _animationController.SetEquipType(1);
                }

                break;
            }
        }

        //Equip shield item if using one handed melee weapon
        if (equippedWeapon is WeaponMeleeItem && !(equippedWeapon as WeaponMeleeItem).twoHanded)
        {
            foreach (Item shieldItem in currentInventory)
            {
                equippedShield = shieldItem as ShieldItem;
                break;
            }
        }


        if (equippedShield != null)
        {
            _animationController.SetShield(true);
        }
        else
        {
            _animationController.SetShield(false);
        }
    }

    //Equips the equipment slots with the highest value equipment that fits
    private void EquipEquipment()
    {
        foreach (Item item in currentInventory)
        {
            if (item is EquipmentItem equipmentItem)
            {
                switch (equipmentItem.equipmentType)
                {
                    case EquipmentType.armour:
                        if (equippedArmour == null)
                        {
                            equippedArmour = equipmentItem;
                        }
                        break;
                    case EquipmentType.cape:
                        if (equippedCape == null)
                        {
                            equippedCape = equipmentItem;
                        }
                        break;
                    case EquipmentType.feet:
                        if (equippedFeet == null)
                        {
                            equippedFeet = equipmentItem;
                        }
                        break;
                    case EquipmentType.greaves:
                        if (equippedGreaves == null)
                        {
                            equippedGreaves = equipmentItem;
                        }
                        break;
                    case EquipmentType.hands:
                        if (equippedHands == null)
                        {
                            equippedHands = equipmentItem;
                        }
                        break;
                    case EquipmentType.helmet:
                        if (equippedHelmet == null)
                        {
                            equippedHelmet = equipmentItem;
                        }
                        break;
                    case EquipmentType.pants:
                        if (equippedPants == null)
                        {
                            equippedPants = equipmentItem;
                        }
                        break;
                    case EquipmentType.shirt:
                        if (equippedShirt == null)
                        {
                            equippedShirt = equipmentItem;
                        }
                        break;
                }
            }
        }
    }

    public override void TriggerBlock()
    {
        if (_characterCombatController != null)
        {
            _animationController.TriggerBlock();
            _characterCombatController.DecideNextAction();
        }
    }

    public override void DamageHealth(int i)
    {
        base.DamageHealth(i);

        if (healthCurrent > 0)
        {
            _animationController.HitReaction();
        }

        SetCharacterState();
    }

    public override void SetSlowState(bool isSlowed)
    {
        if (isSlowed)
        {
            _characterMovementController.SlowMovement();
        }
        else
        {
            _characterMovementController.NormalMovment();
        }
    }

    public override void SetParalyseState(bool isParalysed)
    {
        if (isParalysed)
        {
            _characterMovementController.StopMovement();
        }
        else
        {
            _characterMovementController.StartMovement();
        }
    }
}
