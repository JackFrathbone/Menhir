using System.Collections.Generic;
using UnityEngine;

public enum CharacterState { alive, wounded, dead };
public enum CharacterPronouns { He, She, They };

public class CharacterManager : MonoBehaviour
{
    [Header("Character Data")]
    [ReadOnly] public string characterName;
    [ReadOnly] public CharacterPronouns characterPronouns;
    [ReadOnly] public Faction characterFaction;
    [Tooltip("Passive prevents all combat, neutral checks faction relations before deciding on combat, aggresive always starts combat")]
    [ReadOnly] public Aggression characterAggression = Aggression.Neutral;

    [Header("Ability Scores")]
    [ReadOnly] public Abilities abilities;

    [Header("Status")]
    [ReadOnly] public float healthCurrent;
    [ReadOnly] public float staminaCurrent;
    [ReadOnly] public float healthTotal;
    [ReadOnly] public float staminaTotal;

    [ReadOnly] public int armourDefenceTotal;
    [ReadOnly] public int bonusDefence;
    [ReadOnly] public int bonusDamage;
    [ReadOnly] public int effectResistChance;

    [Header("Equipped Items")]
    [ReadOnly] public Item equippedWeapon;
    [ReadOnly] public ShieldItem equippedShield;
    [ReadOnly] public EquipmentItem equippedArmour;
    [ReadOnly] public EquipmentItem equippedCape;
    [ReadOnly] public EquipmentItem equippedFeet;
    [ReadOnly] public EquipmentItem equippedGreaves;
    [ReadOnly] public EquipmentItem equippedHands;
    [ReadOnly] public EquipmentItem equippedHelmet;
    [ReadOnly] public EquipmentItem equippedPants;
    [ReadOnly] public EquipmentItem equippedShirt;

    [Header("Inventory")]
    [ReadOnly] public List<Item> currentInventory = new();

    [Header("Spells")]
    [ReadOnly] public List<Spell> currentSpells = new();

    [Header("Skills")]
    [ReadOnly] public List<Skill> currentSkills = new();

    [Header("Active Effects")]
    [ReadOnly] public List<Effect> currentEffects = new();
    //Used to track effect that have ended and need to be removed
    [HideInInspector] public List<Effect> endedEffects = new();

    [Header("Character States")]
    [ReadOnly] public CharacterState characterState;
    [ReadOnly] public bool hasAdvantage;
    [ReadOnly] public bool hasDisadvantage;
    [ReadOnly] public bool inCombat;
    [ReadOnly] public bool isCrouching;
    [ReadOnly] public List<CharacterManager> inCombatWith = new();

    [Header("Skill Bonuses/Checks")]
    [ReadOnly] public int berzerkerDamageBonus;
    [ReadOnly] public int oneManArmyDefenceBonus;
    [Tooltip("How many eyes are currently on the character")]
    [ReadOnly] public List<CharacterManager> inDetectionRange = new();
    [ReadOnly] public bool isInvisible = false;
    public Effect _disablingShotEffect;

    protected virtual void Awake()
    {
    }

    protected virtual void Start()
    {
        SetCurrentStatus();
        InvokeRepeating(nameof(RunEffects), 0f, 1f);

        //Set natural effect resist from mind ability
        SetEffectResist(StatFormulas.MagicResistBonus(abilities.mind));
    }

    protected virtual void Update()
    {
        RegenStamina();
    }

    protected virtual void RegenStamina()
    {
        if (staminaCurrent >= staminaTotal)
        {
            staminaCurrent = staminaTotal;
            return;
        }

        if (staminaCurrent < 0)
        {
            staminaCurrent = 0;
        }
        else
        {
            AddStamina(StatFormulas.StaminaRegenRate(abilities.hands) * Time.deltaTime);
            return;
        }
    }

    public virtual void SetCharacterState()
    {

    }

    protected virtual void SetCurrentStatus()
    {
        healthTotal = StatFormulas.TotalCharacterHealth(abilities.body);
        healthCurrent = healthTotal;

        staminaTotal = StatFormulas.TotalCharacterStamina(abilities.hands);
        staminaCurrent = staminaTotal;
    }

    public virtual bool CheckHostility(Faction targetFaction)
    {
        if (characterAggression == Aggression.Hostile)
        {
            return true;
        }

        return false;
    }

    public virtual void TriggerBlock()
    {
    }

    public virtual int GetTotalDefence(bool isRangedAttack)
    {
        int weaponDefence = 0;
        int shieldDefence = 0;

        if (equippedWeapon != null)
        {
            if (equippedWeapon is WeaponMeleeItem)
            {
                weaponDefence = (equippedWeapon as WeaponMeleeItem).weaponDefence;
            }
            else
            {
                weaponDefence = 0;
            }
        }

        GetEquipmentDefence();

        if (equippedShield != null)
        {
            shieldDefence = equippedShield.shieldDefence;
        }

        return StatFormulas.GetTotalDefence(weaponDefence, armourDefenceTotal, shieldDefence, abilities.hands, bonusDefence, isRangedAttack);
    }

    public virtual void AddHealth(float i)
    {
        if (characterState == CharacterState.alive)
        {
            healthCurrent += i;

            if (healthCurrent > healthTotal)
            {
                healthCurrent = healthTotal;
            }
        }
    }

    public virtual void DamageHealth(int i, CharacterManager damageSource)
    {
        if (characterState == CharacterState.alive)
        {
            healthCurrent -= i;

            if (CheckSkill("Berzerker") && inCombat)
            {
                berzerkerDamageBonus += i;
                bonusDamage += i;
            }

            if (healthCurrent <= 0)
            {
                if (damageSource != null)
                {
                    //Check if the character is wounded or dead
                    if (StatFormulas.ToWound(damageSource.abilities.heart))
                    {
                        characterState = CharacterState.wounded;
                    }
                    else
                    {
                        characterState = CharacterState.dead;
                    }
                }
                else
                {
                    characterState = CharacterState.dead;
                }
            }
        }
        else if (characterState == CharacterState.wounded)
        {
            characterState = CharacterState.dead;
        }
    }

    public virtual void AddStamina(float i)
    {
        if (characterState == CharacterState.alive)
        {
            staminaCurrent += i;

            if (staminaCurrent > staminaTotal)
            {
                staminaCurrent = staminaTotal;
            }

        }
    }

    public virtual void DamageStamina(float i)
    {
        if (characterState == CharacterState.alive)
        {
            staminaCurrent -= i;

            if (staminaCurrent <= 0)
            {
                //Do something at zero stamina
            }
        }
    }

    public virtual void ChangeAbilityTotal(int i, string abilityName)
    {
        //Take away current natual bonus to resist magic
        effectResistChance -= StatFormulas.MagicResistBonus(abilities.mind);

        switch (abilityName)
        {
            case "body":
                abilities.body += i;
                break;
            case "hands":
                abilities.hands += i;
                break;
            case "mind":
                abilities.mind += i;
                break;
            case "heart":
                abilities.heart += i;
                break;
        }

        //Set natural effect resist from mind ability
        SetEffectResist(StatFormulas.MagicResistBonus(abilities.mind));
    }

    public virtual void SetSlowState(bool isSlowed)
    {

    }

    public virtual void SetParalyseState(bool isParalysed)
    {

    }

    public virtual void GetCurrentWeaponStats(out int damage, out int bluntDamage, out int defence, out float range, out float speed, out bool isRanged, out GameObject projectile, out List<Effect> effects, out float weaponWeight)
    {
        if (equippedWeapon != null)
        {
            if (equippedWeapon is WeaponMeleeItem meleeItem)
            {

                damage = (equippedWeapon as WeaponMeleeItem).weaponDamage;
                bluntDamage = (equippedWeapon as WeaponMeleeItem).weaponBlunt;
                defence = (equippedWeapon as WeaponMeleeItem).weaponDefence;
                range = (equippedWeapon as WeaponMeleeItem).weaponRange;
                speed = (equippedWeapon as WeaponMeleeItem).weaponSpeed;
                isRanged = false;
                projectile = null;
                effects = null;
                weaponWeight = equippedWeapon.itemWeight;
                return;
            }
            else if (equippedWeapon is WeaponRangedItem rangedItem)
            {
                damage = (equippedWeapon as WeaponRangedItem).weaponDamage;
                bluntDamage = 0;
                defence = 0;
                range = 15;
                speed = (equippedWeapon as WeaponRangedItem).weaponSpeed;
                isRanged = true;
                projectile = (equippedWeapon as WeaponRangedItem).projectilePrefab;
                effects = null;
                weaponWeight = equippedWeapon.itemWeight;
                return;
            }
            else if (equippedWeapon is WeaponFocusItem focusItem)
            {
                damage = 0;
                bluntDamage = 0;
                defence = 0;
                range = 15;
                speed = (equippedWeapon as WeaponFocusItem).castingSpeed;
                isRanged = true;
                projectile = (equippedWeapon as WeaponFocusItem).projectilePrefab;
                effects = (equippedWeapon as WeaponFocusItem).focusEffects;
                weaponWeight = equippedWeapon.itemWeight;
                return;
            }
        }

        damage = 0;
        bluntDamage = 0;
        defence = 0;
        range = 0;
        speed = 0;
        isRanged = false;
        projectile = null;
        effects = null;
        weaponWeight = 0;
    }

    public virtual void SetBonusDefence(int i)
    {
        bonusDefence += i;

        //Caps min bonus at 0
        if (bonusDefence < 0)
        {
            bonusDefence = 0;
        }
    }

    public virtual void SetBonusDamage(int i)
    {
        bonusDamage += i;

        //Caps min bonus at 0
        if (bonusDamage < 0)
        {
            bonusDamage = 0;
        }
    }

    public virtual void SetEffectResist(int i)
    {
        effectResistChance += i;

        if (effectResistChance > 100)
        {
            effectResistChance = 100;
        }
        else if (effectResistChance < 0)
        {
            effectResistChance = 0;
        }
    }

    public virtual void SetAdvantage(bool b)
    {
        hasAdvantage = b;
    }

    public virtual void SetDisadvantage(bool b)
    {
        hasDisadvantage = b;
    }

    public virtual void StartCombat(CharacterManager combatCharacter)
    {
        if (inCombatWith.Contains(combatCharacter))
        {
            return;
        }

        inCombat = true;
        inCombatWith.Add(combatCharacter);

        if (CheckSkill("One Man Army"))
        {
            oneManArmyDefenceBonus += 1;
            bonusDefence += 1;
        }
    }


    public virtual void StopCombat(CharacterManager combatCharacter)
    {
        inCombatWith.Remove(combatCharacter);

        CheckEndCombat();
    }

    public virtual void CheckEndCombat()
    {
        if (inCombatWith.Count == 0)
        {
            inCombat = false;

            if (CheckSkill("Berzerker"))
            {
                bonusDamage -= berzerkerDamageBonus;
                berzerkerDamageBonus = 0;
            }

            if (CheckSkill("One Man Army"))
            {
                bonusDefence -= oneManArmyDefenceBonus;
                oneManArmyDefenceBonus = 0;
            }
        }
        else
        {
            return;
        }
    }

    public virtual void GetEquipmentDefence()
    {
        armourDefenceTotal = 0;

        if (equippedArmour != null)
        {
            armourDefenceTotal += equippedArmour.equipmentDefence;
        }
        if (equippedCape != null)
        {
            armourDefenceTotal += equippedCape.equipmentDefence;
        }
        if (equippedFeet != null)
        {
            armourDefenceTotal += equippedFeet.equipmentDefence;
        }
        if (equippedGreaves != null)
        {
            armourDefenceTotal += equippedGreaves.equipmentDefence;
        }
        if (equippedHands != null)
        {
            armourDefenceTotal += equippedHands.equipmentDefence;
        }
        if (equippedHelmet != null)
        {
            armourDefenceTotal += equippedHelmet.equipmentDefence;
        }
        if (equippedPants != null)
        {
            armourDefenceTotal += equippedPants.equipmentDefence;
        }
        if (equippedShirt != null)
        {
            armourDefenceTotal += equippedShirt.equipmentDefence;
        }
    }

    public virtual void UpdateAnimationState(string stateName, int stateInt)
    {

    }

    public virtual void AddItem(Item i)
    {
        currentInventory.Add(i);
    }

    public virtual void RemoveItem(Item i)
    {
        currentInventory.Remove(i);
    }

    public virtual void UsePotionItem(PotionItem potionItem)
    {
        MessageBox.instance.Create("You drink the potion", true);

        foreach (Effect effect in potionItem.potionEffects)
        {
            effect.AddEffect(this);
        }

        RemoveItem(potionItem);
    }

    //Note that the effect should be added via Effect.AddEffect(CharacterManager) first
    public virtual void AddEffect(Effect effect)
    {
        //If the effect chance is not 100 or 0, the check if it passes
        if (effect.effectChance != 100 && effect.effectChance != 0)
        {
            float effectChance = Random.Range(0f, 100f);

            if (effect.effectChance >= effectChance)
            {
                return;
            }
        }

        //If spell shield is active chance to not apply any effect
        float resistChance = Random.Range(0f, 100f);

        if (effectResistChance >= resistChance)
        {
            return;
        }

        currentEffects.Add(effect);
    }

    public virtual void RemoveEffect(Effect effect)
    {
        if (currentEffects.Contains(effect))
        {
            currentEffects.Remove(effect);
        }
        else
        {
            return;
        }
    }

    public virtual void ClearEffects()
    {
        foreach (Effect effect in currentEffects)
        {
            effect.EndEffect(this);
        }

        currentEffects.Clear();
    }

    //Runs every second, applies effects, removes ones which have run out
    public virtual void RunEffects()
    {
        endedEffects.Clear();

        foreach (Effect effect in currentEffects)
        {
            effect.ApplyEffect(this);
        }

        foreach (Effect effect in endedEffects)
        {
            RemoveEffect(effect);
        }

        endedEffects.Clear();
    }

    public virtual void AddSkill(Skill newSkill)
    {
        currentSkills.Add(newSkill);

        if (newSkill.skillItems.Count != 0)
        {
            foreach (Item item in newSkill.skillItems)
            {
                AddItem(item);
            }
        }
    }

    public virtual bool CheckSkill(string skillName)
    {
        foreach (Skill skill in currentSkills)
        {
            if (skill.skillName == skillName)
            {
                return true;
            }
        }

        return false;
    }

    //Skill Checks//
    //If player has the skill and the other conditions mets
    public virtual bool CheckSkill_Assassinate()
    {
        if (CheckSkill("Assassinate") && !inCombat && CheckSkill_Sneak())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool CheckSkill_HonourFighter()
    {
        if (CheckSkill("Honour Fighter") && inCombatWith.Count == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool CheckSkill_Sharpshooter()
    {
        if (CheckSkill("Sharpshooter") && !inCombat && (equippedWeapon is WeaponRangedItem))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Used for checking if the player has the sneak skill, or has an invisibility effect active
    public virtual bool CheckSkill_Sneak()
    {
        if (isInvisible && isCrouching && inCombatWith.Count == 0)
        {
            return true;
        }

        if (CheckSkill("Sneak") && isCrouching && inCombatWith.Count == 0 && inDetectionRange.Count <= 2)
        {
            return true;
        }
        else
        {
            //Make the targets aware of the player
            if (inDetectionRange.Count != 0)
            {
                List<CharacterManager> currentDetectedCharacter = inDetectionRange;

                foreach (CharacterManager character in currentDetectedCharacter)
                {
                    if (character == null)
                    {
                        break;
                    }

                    CharacterAI aiController = character.GetComponent<CharacterAI>();

                    if (aiController != null)
                    {
                        aiController.DetectTarget(this);
                    }
                }
            }
            return false;
        }
    }

    public virtual bool CheckSkill_DisablingShot(CharacterManager targetCharacter)
    {
        if (CheckSkill("Disabling Shot") && (equippedWeapon is WeaponRangedItem))
        {
            _disablingShotEffect.AddEffect(targetCharacter);
            return true;
        }
        else
        {
            return false;
        }
    }
}
