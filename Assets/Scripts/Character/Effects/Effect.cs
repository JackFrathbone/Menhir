using UnityEngine;

public enum EffectType
{
    Restore_Health,
    Damage_Health,
    Fortify_Health,
    Drain_Health,
    Restore_Stamina,
    Damage_Stamina,
    Fortify_Stamina,
    Drain_Stamina,
    Fortify_Ability_Body,
    Fortify_Ability_Hands,
    Fortify_Ability_Mind,
    Fortify_Ability_Heart,
    Drain_Ability_Body,
    Drain_Ability_Hands,
    Drain_Ability_Mind,
    Drain_Ability_Heart,
    Slow,
    Paralyse,
    Shield,
    Spell_Shield,
    Set_Teleport,
    Teleport,
    Create_Item,
    Spawn_Character,
    Fortify_Attack,
    Drain_Attack,
    Remove_Effects,
    Gain_Advantage,
    Gain_Disadvantage,
    Invisibility,
    Spell_Casting_Speed
}

[System.Serializable]
public class Effect
{

    [Header("Effect Data")]
    public EffectType effectType;

    [Tooltip("Effect does x amount")]
    public int effectStrength;
    [Tooltip("for x seconds")]
    [Range(1, 60)] public int effectSeconds = 1;

    [Tooltip("chance of effect applying. 0 or 100 mean it always applies")]
    [Range(1, 100)] public float effectChance = 100;

    //This is only used for specific effects
    [Header("Extra Data")]
    public Vector3 effectTeleportPos;
    public Item effectCreateItem;
    public CharacterManager effectSpawnCharacter;

    [Header("Checks")]
    //The hidden checks
    [ReadOnly] public bool permanentEffect;
    [ReadOnly] public int effectSecondsPassed;

    //used to determine if effects should apply once at the start, or every second
    private bool _runOnce;

    public void ApplyEffect(CharacterManager targetCharacter)
    {
        //Debug.Log(effectType.ToString() + " on " + targetCharacter.characterSheet.characterName);

        //If the effect is not permanent then check if time has run out and remove time
        if (!permanentEffect)
        {
            //If time has run out then remove the effect
            if (effectSecondsPassed <= 0)
            {
                return;
            }

            //Remove a second each time
            effectSecondsPassed--;
        }

        //if runonce is true then dont apply the effect again
        if (_runOnce)
        {
            return;
        }

        switch (effectType)
        {
            case EffectType.Restore_Health:
                targetCharacter.AddHealth(effectStrength);
                break;
            case EffectType.Damage_Health:
                targetCharacter.DamageHealth(effectStrength, null);
                break;
            case EffectType.Fortify_Health:
                _runOnce = true;
                targetCharacter.AddHealth(effectStrength);
                break;
            case EffectType.Drain_Health:
                _runOnce = true;
                targetCharacter.DamageHealth(effectStrength, null);
                break;
            case EffectType.Restore_Stamina:
                targetCharacter.AddStamina(effectStrength);
                break;
            case EffectType.Damage_Stamina:
                targetCharacter.DamageStamina(effectStrength);
                break;
            case EffectType.Fortify_Stamina:
                _runOnce = true;
                targetCharacter.AddStamina(effectStrength);
                break;
            case EffectType.Drain_Stamina:
                _runOnce = true;
                targetCharacter.DamageStamina(effectStrength);
                break;
            case EffectType.Fortify_Ability_Body:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(effectStrength, "body");
                break;
            case EffectType.Fortify_Ability_Hands:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(effectStrength, "hands");
                break;
            case EffectType.Fortify_Ability_Mind:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(effectStrength, "mind");
                break;
            case EffectType.Fortify_Ability_Heart:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(effectStrength, "heart");
                break;
            case EffectType.Drain_Ability_Body:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(-effectStrength, "body");
                break;
            case EffectType.Drain_Ability_Hands:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(-effectStrength, "hands");
                break;
            case EffectType.Drain_Ability_Mind:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(-effectStrength, "mind");
                break;
            case EffectType.Drain_Ability_Heart:
                _runOnce = true;
                targetCharacter.ChangeAbilityTotal(-effectStrength, "body");
                break;
            case EffectType.Slow:
                _runOnce = true;
                targetCharacter.SetSlowState(true);
                break;
            case EffectType.Paralyse:
                _runOnce = true;
                targetCharacter.SetParalyseState(true);
                break;
            case EffectType.Shield:
                _runOnce = true;
                targetCharacter.SetBonusDefence(effectStrength);
                break;
            case EffectType.Spell_Shield:
                _runOnce = true;
                targetCharacter.SetEffectResist(effectStrength);
                break;
            case EffectType.Set_Teleport:
                _runOnce = true;
                break;
            case EffectType.Teleport:
                _runOnce = true;
                break;
            case EffectType.Create_Item:
                _runOnce = true;
                if (effectCreateItem != null)
                {
                    targetCharacter.AddItem(effectCreateItem);
                }
                else
                {
                    Debug.Log("Invalid effect item set");
                }
                break;
            case EffectType.Spawn_Character:
                _runOnce = true;
                break;
            case EffectType.Fortify_Attack:
                _runOnce = true;
                targetCharacter.SetBonusDamage(effectStrength);
                break;
            case EffectType.Drain_Attack:
                _runOnce = true;
                targetCharacter.SetBonusDamage(-effectStrength);
                break;
            case EffectType.Remove_Effects:
                targetCharacter.ClearEffects();
                break;
            case EffectType.Gain_Advantage:
                _runOnce = true;
                targetCharacter.SetAdvantage(true);
                break;
            case EffectType.Gain_Disadvantage:
                _runOnce = true;
                targetCharacter.SetDisadvantage(true);
                break;
            case EffectType.Invisibility:
                _runOnce = true;
                targetCharacter.isInvisible = false;
                break;
            case EffectType.Spell_Casting_Speed:
                _runOnce = true;
                targetCharacter.SetSpellCastingBonus(effectStrength);
                break;
        }
    }

    public void EndEffect(CharacterManager targetCharacter)
    {
        //Only to run on the effect ending ie giving back health after drain
        switch (effectType)
        {
            case EffectType.Restore_Health:
                //na
                break;
            case EffectType.Damage_Health:
                //na
                break;
            case EffectType.Fortify_Health:
                targetCharacter.DamageHealth(effectStrength, null);
                break;
            case EffectType.Drain_Health:
                targetCharacter.AddHealth(effectStrength);
                break;
            case EffectType.Restore_Stamina:
                //na
                break;
            case EffectType.Damage_Stamina:
                //na
                break;
            case EffectType.Fortify_Stamina:
                targetCharacter.DamageStamina(effectStrength);
                break;
            case EffectType.Drain_Stamina:
                targetCharacter.AddStamina(effectStrength);
                break;
            case EffectType.Fortify_Ability_Body:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "body");
                break;
            case EffectType.Fortify_Ability_Hands:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "hands");
                break;
            case EffectType.Fortify_Ability_Mind:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "mind");
                break;
            case EffectType.Fortify_Ability_Heart:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "heart");
                break;
            case EffectType.Drain_Ability_Body:
                targetCharacter.ChangeAbilityTotal(effectStrength, "body");
                break;
            case EffectType.Drain_Ability_Hands:
                targetCharacter.ChangeAbilityTotal(effectStrength, "hands");
                break;
            case EffectType.Drain_Ability_Mind:
                targetCharacter.ChangeAbilityTotal(effectStrength, "mind");
                break;
            case EffectType.Drain_Ability_Heart:
                targetCharacter.ChangeAbilityTotal(effectStrength, "body");
                break;
            case EffectType.Slow:
                targetCharacter.SetSlowState(false);
                break;
            case EffectType.Paralyse:
                targetCharacter.SetParalyseState(false);
                break;
            case EffectType.Shield:
                targetCharacter.SetBonusDefence(-effectStrength);
                break;
            case EffectType.Spell_Shield:
                targetCharacter.SetEffectResist(-effectStrength);
                break;
            case EffectType.Set_Teleport:
                break;
            case EffectType.Teleport:
                break;
            case EffectType.Create_Item:
                break;
            case EffectType.Spawn_Character:
                break;
            case EffectType.Fortify_Attack:
                targetCharacter.SetBonusDamage(-effectStrength);
                break;
            case EffectType.Drain_Attack:
                targetCharacter.SetBonusDamage(effectStrength);
                break;
            case EffectType.Remove_Effects:
                //No end effect
                break;
            case EffectType.Gain_Advantage:
                targetCharacter.SetAdvantage(false);
                break;
            case EffectType.Gain_Disadvantage:
                targetCharacter.SetDisadvantage(false);
                break;
            case EffectType.Invisibility:
                targetCharacter.isInvisible = false;
                break;
            case EffectType.Spell_Casting_Speed:
                _runOnce = true;
                targetCharacter.SetSpellCastingBonus(-effectStrength);
                break;
        }
        //Debug.Log(GetDescription() + " on" + targetCharacter.characterSheet.characterName + " ended");
    }

    public string GetDescription()
    {
        string effectname = effectType.ToString();
        effectname = effectname.Replace("_", " ");

        string strengthString = "";
        if (effectStrength != 0)
        {
            strengthString = " " + effectStrength + " points";
        }

        string timeString = "";
        if (effectSeconds != 0)
        {
            timeString = " for " + effectSeconds + " seconds";
        }


        return effectname + strengthString + timeString;
    }
}
