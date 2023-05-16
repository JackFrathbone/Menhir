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
    Gain_Disadvantage
}

[System.Serializable]
public class Effect
{

    [Header("Effect Data")]
    public EffectType effectType;

    [Tooltip("Effect does x amount")]
    public int effectStrength;
    [Tooltip("for x seconds")]
    [Range(1,60)] public int effectSeconds = 1;

    [Tooltip("chance of effect applying. 0 or 100 mean it always applies")]
    public float effectChance = 100;

    //This is only used for specific effects
    [Header("Extra Data")]

    //The hidden checks
    private int effectSecondsPassed;

    public void AddEffect(CharacterManager targetCharacter)
    {
        //Debug.Log(GetDescription() + " on " + targetCharacter.characterSheet.characterName);

        effectSecondsPassed = effectSeconds;

        targetCharacter.AddEffect(this);
    }

    public void ApplyEffect(CharacterManager targetCharacter)
    {
        //Debug.Log(effectType.ToString() + " on " + targetCharacter.characterSheet.characterName);

        if (effectSecondsPassed <= 0)
        {
            EndEffect(targetCharacter);
            return;
        }

        effectSecondsPassed--;

        switch (effectType)
        {
            case EffectType.Restore_Health:
                targetCharacter.AddHealth(effectStrength);
                break;
            case EffectType.Damage_Health:
                targetCharacter.DamageHealth(effectStrength);
                break;
            case EffectType.Fortify_Health:
                targetCharacter.AddHealth(effectStrength);
                break;
            case EffectType.Drain_Health:
                targetCharacter.DamageHealth(effectStrength);
                break;
            case EffectType.Restore_Stamina:
                targetCharacter.AddStamina(effectStrength);
                break;
            case EffectType.Damage_Stamina:
                targetCharacter.DamageStamina(effectStrength);
                break;
            case EffectType.Fortify_Stamina:
                targetCharacter.AddStamina(effectStrength);
                break;
            case EffectType.Drain_Stamina:
                targetCharacter.DamageStamina(effectStrength);
                break;
            case EffectType.Fortify_Ability_Body:
                targetCharacter.ChangeAbilityTotal(effectStrength, "body");
                break;
            case EffectType.Fortify_Ability_Hands:
                targetCharacter.ChangeAbilityTotal(effectStrength, "hands");
                break;
            case EffectType.Fortify_Ability_Mind:
                targetCharacter.ChangeAbilityTotal(effectStrength, "mind");
                break;
            case EffectType.Fortify_Ability_Heart:
                targetCharacter.ChangeAbilityTotal(effectStrength, "heart");
                break;
            case EffectType.Drain_Ability_Body:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "body");
                break;
            case EffectType.Drain_Ability_Hands:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "hands");
                break;
            case EffectType.Drain_Ability_Mind:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "mind");
                break;
            case EffectType.Drain_Ability_Heart:
                targetCharacter.ChangeAbilityTotal(-effectStrength, "body");
                break;
            case EffectType.Slow:
                targetCharacter.SetSlowState(true);
                break;
            case EffectType.Paralyse:
                targetCharacter.SetParalyseState(true);
                break;
            case EffectType.Shield:
                targetCharacter.SetBonusDefence(effectStrength);
                break;
            case EffectType.Spell_Shield:
                targetCharacter.SetEffectResist(effectStrength);
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
                targetCharacter.SetBonusDamage(effectStrength);
                break;
            case EffectType.Drain_Attack:
                targetCharacter.SetBonusDamage(-effectStrength);
                break;
            case EffectType.Remove_Effects:
                targetCharacter.ClearEffects();
                break;
            case EffectType.Gain_Advantage:
                targetCharacter.SetAdvantage(true);
                break;
            case EffectType.Gain_Disadvantage:
                targetCharacter.SetDisadvantage(true);
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
                targetCharacter.DamageHealth(effectStrength);
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
        }

        targetCharacter.endedEffects.Add(this);
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
