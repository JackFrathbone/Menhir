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
    [ReadOnly] public int totalDefence;

    [ReadOnly] public int bonusDefence;
    [ReadOnly] public int bonusDamage;
    [ReadOnly] public int effectResistChance;
    [ReadOnly] public float castingBonus;

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
    /*[ReadOnly]*/
    public List<Effect> currentEffects = new();
    //Used to track effect that have ended and need to be removed
    /*[HideInInspector]*/
    public List<Effect> endedEffects = new();

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

        if (healthCurrent == 0)
        {
            healthCurrent = healthTotal;
        }

        staminaTotal = StatFormulas.TotalCharacterStamina(abilities.hands);
        if (staminaCurrent == 0)
        {
            staminaCurrent = staminaTotal;
        }
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

    public virtual int GetTotalDefence()
    {
        //Get equipment total
        int equipmentDefenceTotal = GetEquipmentDefence(); ;

        //Get weapon defence
        int weaponDefence = 0;
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

        //Get shield defence
        int shieldDefence = 0;
        if (equippedShield != null)
        {
            shieldDefence = equippedShield.shieldDefence;
        }

        totalDefence = StatFormulas.GetTotalDefence(weaponDefence, equipmentDefenceTotal, shieldDefence, bonusDefence);

        return totalDefence;
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
                //Remove previous berzerker damage
                bonusDamage -= berzerkerDamageBonus;

                //Get the damage bonus and add to the total
                berzerkerDamageBonus = StatFormulas.CalculateBerzerkerDamageBonus(healthTotal, healthCurrent, GetWeaponDamage());
                bonusDamage += berzerkerDamageBonus;

                //Debug.Log("Adding berzerker damage bonus: " + berzerkerDamageBonus);
            }

            if (healthCurrent <= 0)
            {
                //Check the second wind skill
                if (CheckSkill_SecondWind())
                {
                    //get 25% of total health back
                    healthCurrent = healthTotal * 50 / 100;
                    return;
                }

                //Stop combat between two chars
                if (damageSource != null)
                {
                    if (inCombatWith.Contains(damageSource))
                    {
                        StopCombat(damageSource);
                    }

                    if (damageSource.inCombatWith.Contains(this))
                    {
                        damageSource.StopCombat(this);
                    }
                }

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

    public virtual void GetCurrentWeaponStats(out int damage, out int bluntDamage, out float range, out float speed, out bool isRanged, out GameObject projectile, out List<Effect> enchantmentsEffects, out float weaponWeight)
    {
        if (equippedWeapon != null)
        {
            if (equippedWeapon is WeaponMeleeItem)
            {

                damage = (equippedWeapon as WeaponMeleeItem).weaponDamage;
                bluntDamage = (equippedWeapon as WeaponMeleeItem).weapontToHitBonus;
                range = (equippedWeapon as WeaponMeleeItem).weaponRange;
                speed = (equippedWeapon as WeaponMeleeItem).weaponSpeed;
                isRanged = false;
                projectile = null;
                enchantmentsEffects = (equippedWeapon as WeaponMeleeItem).enchantmentTargetEffects;
                weaponWeight = equippedWeapon.itemWeight;
                return;
            }
            else if (equippedWeapon is WeaponRangedItem)
            {
                damage = (equippedWeapon as WeaponRangedItem).weaponDamage;
                bluntDamage = 0;
                range = 15;
                speed = (equippedWeapon as WeaponRangedItem).weaponSpeed;
                isRanged = true;
                projectile = (equippedWeapon as WeaponRangedItem).projectilePrefab;
                enchantmentsEffects = (equippedWeapon as WeaponRangedItem).enchantmentTargetEffects;
                weaponWeight = equippedWeapon.itemWeight;
                return;
            }
        }

        damage = 0;
        bluntDamage = 0;
        range = 0;
        speed = 0;
        isRanged = false;
        projectile = null;
        enchantmentsEffects = null;
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
            bonusDefence -= oneManArmyDefenceBonus;

            oneManArmyDefenceBonus = StatFormulas.CalculateOneManArmyDefenceBonus(inCombatWith.Count);

            bonusDefence += oneManArmyDefenceBonus;
        }
    }


    public virtual void StopCombat(CharacterManager combatCharacter)
    {
        inCombatWith.Remove(combatCharacter);

        if (CheckSkill("One Man Army"))
        {
            bonusDefence -= oneManArmyDefenceBonus;

            oneManArmyDefenceBonus = StatFormulas.CalculateOneManArmyDefenceBonus(inCombatWith.Count);

            bonusDefence += oneManArmyDefenceBonus;
        }

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

    public virtual int GetWeaponDamage()
    {
        //Get current weapon damage
        int weaponDamage = 0;
        if (equippedWeapon is WeaponMeleeItem)
        {
            weaponDamage = (equippedWeapon as WeaponMeleeItem).weaponDamage;
        }
        else if (equippedWeapon is WeaponRangedItem)
        {
            weaponDamage = (equippedWeapon as WeaponRangedItem).weaponDamage;
        }

        return weaponDamage;
    }

    public virtual int GetEquipmentDefence()
    {
        int armourDefenceTotal = 0;

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

        return armourDefenceTotal;
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
        MessageBox.instance.Create("You use the item", true);

        foreach (Effect effect in potionItem.potionEffects)
        {
            AddEffect(effect);
        }

        RemoveItem(potionItem);
    }

    //For adding an effect for the first time
    public virtual void AddEffect(Effect effect)
    {
        //If the effect chance is not 100 or 0, the check if it passes
        if (effect.effectChance != 100 && effect.effectChance != 0)
        {
            float effectChance = Random.Range(0f, 100f);

            //If the roll is greater than the chance fail it
            if (effectChance >= effect.effectChance)
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

        //Set timer based on if effect is permanent or not
        if (effect.permanentEffect)
        {
            effect.effectSecondsPassed = 2;
        }
        else
        {
            effect.effectSecondsPassed = effect.effectSeconds;
        }

        //Add to current effects
        currentEffects.Add(effect);
    }

    public virtual void RemoveEffect(Effect effect)
    {
        //End the effect, run its end effect, and remove from all lists
        effect.EndEffect(this);

        //Clear all instances of the effect
        if (currentEffects.Contains(effect))
        {
            currentEffects.Remove(effect);
        }
    }

    public virtual void ClearEffects()
    {
        foreach (Effect effect in currentEffects)
        {
            //Add to ended effects
            endedEffects.Add(effect);
        }

        //Clear the current effects
        currentEffects.Clear();
    }

    //Runs every second, applies effects, removes ones which have run out
    public virtual void RunEffects()
    {
        foreach (Effect effect in currentEffects)
        {
            effect.ApplyEffect(this);

            //if the effect has ended then add to ended effects list
            if (effect.effectSecondsPassed <= 0)
            {
                endedEffects.Add(effect);
            }
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
                if (!CurrentInventoryCompareByID(item))
                {
                    AddItem(item);
                }
            }
        }

        if (newSkill.skillSpells.Count != 0)
        {
            foreach (Spell spell in newSkill.skillSpells)
            {
                if (!CurrentSpellsCompareByID(spell))
                {
                    currentSpells.Add(spell);
                }
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


    public virtual bool CheckSneakAttack()
    {
        //If the player is sneaking and is not in combat, used to deal crit damage
        if (!inCombat && CheckSkill_Sneak())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Skill Checks//
    //If player has the skill and the other conditions mets
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
        if (CheckSkill("Sharpshooter") && (equippedWeapon is WeaponRangedItem))
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

                    
                    if (character.TryGetComponent<CharacterAI>(out var aiController))
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
            targetCharacter.AddEffect(_disablingShotEffect);
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool CheckSkill_Hunter(CharacterManager targetCharacter)
    {
        if (targetCharacter is MonsterCharacterManager && CheckSkill("Hunter"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public virtual bool CheckSkill_SecondWind()
    {
        if (CheckSkill("Second Wind"))
        {
            int roll = Random.Range(1, 101);

            if (roll <= 25)
            {
                //Debug.Log("Second Wind Activated");
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public virtual void SetCanAttack(bool canAttack) { }

    public virtual void ResetSpellCooldown() { }

    public virtual void SetSpellCastingBonus(int percentage)
    {
        castingBonus += percentage;
    }


    public bool CurrentInventoryCompareByID(Item compareItem)
    {
        //Find if two items share a unique ID

        foreach (Item item in currentInventory)
        {
            if (item.GetUniqueID() == compareItem.GetUniqueID())
            {
                return true;
            }
        }

        return false;
    }

    public bool CurrentSpellsCompareByID(Spell compareSpell)
    {
        //Find if two spells share a unique ID

        foreach (Spell spell in currentSpells)
        {
            if (spell.GetUniqueID() == compareSpell.GetUniqueID())
            {
                return true;
            }
        }

        return false;
    }
}
