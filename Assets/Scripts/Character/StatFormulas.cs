using UnityEngine;

public static class StatFormulas
{
    public static float TotalCharacterHealth(int physique)
    {
        return 1 + (physique * 2f);
    }

    public static float TotalCharacterStamina(int agility)
    {
        return (1 + (agility * 2f)) * 2f;
    }

    public static int StaminaRegenRate(int agility)
    {
        return agility / 2;
    }

    //Returns a number which if positive is damage done to target, 0 or negative results in a block
    public static int CalculateHit(int hitDamage, int ability, int targetDefence, bool advantage, bool targetDisadvantage, bool checkSkillAssassinate, bool checkSkillLuckyStrikeCharacter, bool checkSkillLuckyStrikeTarget, bool checkSkillHonourFighter, bool checkSkillSharpshooter)
    {
        if (checkSkillLuckyStrikeCharacter)
        {
            int critChance = Random.Range(0, 101);

            if (critChance <= 15)
            {
                advantage = true;
            }
        }

        if (checkSkillLuckyStrikeTarget)
        {
            int critChance = Random.Range(0, 101);

            if (critChance <= 15)
            {
                targetDisadvantage = true;
            }
        }

        if (checkSkillHonourFighter)
        {
            targetDisadvantage = true;
        }

        if (checkSkillSharpshooter)
        {
            targetDisadvantage = true;
        }

        if (checkSkillAssassinate)
        {
            advantage = true;
        }

        if (advantage && !targetDisadvantage)
        {
            int fullDamage = hitDamage + (ability - 3);

            if ((hitDamage + (ability - 3)) - (targetDefence) > 0)
            {
                return fullDamage;
            }
            else
            {
                return (hitDamage + (ability - 3)) - (targetDefence);
            }
        }
        else if (!advantage && targetDisadvantage)
        {
            return (hitDamage + (ability - 3)) - (targetDefence / 2);
        }
        else
        {
            return (hitDamage + (ability - 3)) - (targetDefence);
        }
    }

    public static int GetTotalDefence(int weaponDefence, int equipmentDefence, int shieldDefence, int ability, int defenceBonus, bool isRangedAttack)
    {
        if (isRangedAttack)
        {
            weaponDefence = 0;
            equipmentDefence *= 2;
        }

        return weaponDefence + equipmentDefence + shieldDefence + (ability - 3) + defenceBonus;
    }

    public static int RollDice(int diceMax, int diceAmount)
    {
        int diceTotal = 0;

        if (diceAmount > 1)
        {
            for (int i = 0; i < diceAmount; i++)
            {
                diceTotal += Random.Range(1, diceMax + 1);
            }
        }
        else
        {
            diceTotal = Random.Range(1, diceMax + 1);
        }

        return diceTotal;
    }

    public static int Damage(int damageMax)
    {
        return Random.Range(1, damageMax + 1);
    }

    public static float AttackStaminaCost(float weaponWeight, float weaponRange)
    {
        return (1.5f * weaponWeight) * weaponRange;
    }

    //True is a wound and false is a death
    public static bool ToWound(float damageOverZero, bool checkSkillWoundingBlows)
    {
        float output;

        if (checkSkillWoundingBlows)
        {
            output = Random.Range(0f, 100f) - damageOverZero;
        }
        else
        {
            output = Random.Range(0f, 100f) + damageOverZero;
        }



        if (output >= 50)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
