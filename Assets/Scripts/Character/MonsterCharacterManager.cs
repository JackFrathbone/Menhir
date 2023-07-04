using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCharacterManager : CharacterManager
{
    [Header("Monster Sheet")]
    [SerializeField] private MonsterSheet _baseMonsterSheet;
    [HideInInspector] private MonsterSheet _monsterSheet = null;

    [Header("Monster Factions and Aggression")]
    //Aggression and faction is set on the prefab and not in the sheet, to avoid multiple of the same monster for different factions and aggresions
    [SerializeField] Faction _monsterFaction;
    [SerializeField] Aggression _monsterAggression = Aggression.Hostile;

    [Header("Monster Combat")]
    [ReadOnly] public int damage;
    [ReadOnly] public int toHitBonus;
    [ReadOnly] public int range;
    [ReadOnly] public int attackSpeed;

    [ReadOnly] public bool isRanged;
    [ReadOnly] public GameObject projectilePrefab;
    [ReadOnly] public List<Effect> projectileEffects = new();

    [ReadOnly] public float moveSpeed;

    [Header("States")]
    [ReadOnly] public bool isHidden;

    [Header("Mosnter Visuals")]
    [ReadOnly] public Color spriteColorOverride;

    [ReadOnly] public Sprite idleSprite;
    [ReadOnly] public Sprite walk1Sprite;
    [ReadOnly] public Sprite walk2Sprite;
    [ReadOnly] public Sprite holdSprite;
    [ReadOnly] public Sprite attackSprite;
    [ReadOnly] public Sprite hurtSprite;
    [ReadOnly] public Sprite blockSprite;
    [ReadOnly] public Sprite deadSprite;

    [Header("Monster References")]
    private MonsterAnimationController _monsterAnimationController;

    private void OnEnable()
    {
        DataManager.instance.AddActiveCharacter(this);
    }

    private void OnValidate()
    {
        if (_baseMonsterSheet != null && name == "Empty Monster")
        {
            name = _baseMonsterSheet.monsterName + "_" + UnityEngine.Random.Range(0, 100);
        }
        else if (_baseMonsterSheet == null)
        {
            name = "Empty Monster";
        }
    }

    protected override void Awake()
    {
        _monsterSheet = Instantiate(_baseMonsterSheet);
        SetMonsterRelations(_monsterFaction, _monsterAggression);
        SetDataFromMonsterSheet();

        _monsterAnimationController = GetComponent<MonsterAnimationController>();
    }

    protected override void Start()
    {
        base.Start();
    }

    public void SetMonsterRelations(Faction setFaction, Aggression setAggresion)
    {
        characterFaction = setFaction;
        characterAggression = setAggresion;
    }

    private void SetDataFromMonsterSheet()
    {
        characterName = _monsterSheet.monsterName;

        currentInventory = new List<Item>(_monsterSheet.monsterInventory);

        //Combat//
        healthTotal = _monsterSheet.health;

        damage = _monsterSheet.damage;
        toHitBonus = _monsterSheet.toHitBonus;
        range = _monsterSheet.range;
        attackSpeed = _monsterSheet.attackSpeed;

        isRanged = _monsterSheet.isRanged;
        projectilePrefab = _monsterSheet.projectilePrefab;
        projectileEffects = _monsterSheet.projectileEffects;

        totalDefence = _monsterSheet.defence;
        moveSpeed = _monsterSheet.moveSpeed;

        isHidden = _monsterSheet.startHidden;
        characterState = _monsterSheet.startState;

        spriteColorOverride = _monsterSheet.spriteColorOverride;
        idleSprite = _monsterSheet.idleSprite;
        walk1Sprite = _monsterSheet.walk1Sprite;
        walk2Sprite = _monsterSheet.walk2Sprite;
        holdSprite = _monsterSheet.holdSprite;
        attackSprite = _monsterSheet.attackSprite;
        hurtSprite = _monsterSheet.hurtSprite;
        blockSprite = _monsterSheet.blockSprite;
        deadSprite = _monsterSheet.deadSprite;
    }

    public override int GetTotalDefence()
    {
        return totalDefence;
    }

    public override void DamageHealth(int i, CharacterManager damageSource)
    {
        base.DamageHealth(i, damageSource);

        if (healthCurrent > 0)
        {
            _monsterAnimationController.HitReaction();
        }

        SetCharacterState();
    }

    public override void GetCurrentWeaponStats(out int damage, out int bluntDamage, out float range, out float speed, out bool isRanged, out GameObject projectile, out List<Effect> effects, out List<Effect> enchantmentsEffects, out float weaponWeight)
    {
        damage = this.damage;
        bluntDamage = this.toHitBonus;
        if (this.isRanged)
        {
            range = 15f;
        }
        else
        {
            range = this.range;
        }
        speed = this.attackSpeed;
        isRanged = this.isRanged;
        projectile = this.projectilePrefab;
        effects = this.projectileEffects;
        enchantmentsEffects = null;

        weaponWeight = 0;
    }

    public override void SetCharacterState()
    {
        if (isHidden)
        {
            gameObject.SetActive(false);
            return;
        }

        //Monsters cant be wounded
        if (characterState == CharacterState.wounded)
        {
            characterState = CharacterState.dead;
        }

        switch (characterState)
        {
            case CharacterState.alive:
                break;
            case CharacterState.dead:
                DestroyScripts();
                if (GetComponent<ItemContainer>() == null)
                {
                    _monsterAnimationController.SetState(1);
                    GetComponent<CapsuleCollider>().isTrigger = true;
                    ItemContainer newContainer = gameObject.AddComponent<ItemContainer>();
                    newContainer.SetInventory(currentInventory);
                }
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

    protected override void SetCurrentStatus()
    {
        healthCurrent = healthTotal;

        staminaTotal = 1;
        staminaCurrent = staminaTotal;
    }
}
