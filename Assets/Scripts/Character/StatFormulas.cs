using UnityEngine;

public static class StatFormulas
{
    public static float TotalCharacterHealth(int body)
    {
        return 1 + (body * 5f);
    }

    public static float TotalCharacterStamina(int hands)
    {
        return (1 + (hands * 2f)) * 6f;
    }

    public static float StaminaRegenRate(int hands)
    {
        return (float)hands / 8f;
    }

    public static bool RollToHit(int ability, int hitBonus, int targetDefence, bool hasAdvantage, bool hasDisadvantage, bool checkSkillLuckyStrike, bool checkSkillLuckyStrikeTarget, bool checkSkillHonourFighterTarget, bool checkSkillSharpshooter, bool checkSkillHunter)
    {
        //Generate the chance to hit
        int chanceToHit = ((ability * 10) + hitBonus) - targetDefence;
        //Debug.Log("Chance to Hit: " + chanceToHit);
        //Debug.Log("Target Defence: " + targetDefence);

        //Set advantage from skills
        if (checkSkillLuckyStrike)
        {
            int critChance = Random.Range(1, 101);

            if (critChance <= 15)
            {
                hasAdvantage = true;
            }
        }

        if (checkSkillLuckyStrikeTarget)
        {
            int critChance = Random.Range(1, 101);

            if (critChance <= 15)
            {
                hasAdvantage = true;
            }
        }

        if (checkSkillHunter)
        {
            hasAdvantage = true;
        }

        if (checkSkillHonourFighterTarget)
        {
            hasDisadvantage = true;
        }

        if (checkSkillSharpshooter)
        {
            //Roll against the targets defence, give advantage if passes
            int sharpshooterRoll = Random.Range(1, 101);

            if (sharpshooterRoll <= targetDefence)
            {
                hasAdvantage = true;
            }
        }


        int roll;
        //Roll based on advantage or disadvantage, or single
        if (hasAdvantage && !hasDisadvantage)
        {
            int roll1 = Random.Range(1, 101);
            int roll2 = Random.Range(1, 101);

            if (roll1 > roll2)
            {
                roll = roll1;
            }
            else
            {
                roll = roll2;
            }
        }
        else if (hasDisadvantage && !hasAdvantage)
        {
            int roll1 = Random.Range(1, 101);
            int roll2 = Random.Range(1, 101);

            if (roll1 < roll2)
            {
                roll = roll1;
            }
            else
            {
                roll = roll2;
            }
        }
        else
        {
            roll = Random.Range(1, 101);
        }

        //Conclude
        if (roll <= chanceToHit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int Damage(int damageMax, bool criticalDamage)
    {
        if (criticalDamage)
        {
            return Random.Range(1, damageMax + 1) * 2;
        }
        else
        {
            return Random.Range(1, damageMax + 1);
        }
    }

    public static int GetTotalDefence(int weaponDefence, int equipmentDefence, int shieldDefence, int defenceBonus)
    {
        return weaponDefence + equipmentDefence + shieldDefence + defenceBonus;
    }

    public static int RollDice(int diceMax, int diceAmount)
    {
        int rollTotal = 0;

        if (diceAmount > 1)
        {
            for (int i = 0; i < diceAmount; i++)
            {
                rollTotal += Random.Range(1, diceMax + 1);
            }
        }
        else
        {
            rollTotal = Random.Range(1, diceMax + 1);
        }

        return rollTotal;
    }



    public static float AttackStaminaCost(float weaponWeight, float weaponRange)
    {
        return (1.5f * weaponWeight) * weaponRange;
    }

    //True is a wound and false is a death, used heart score to add chance
    public static bool ToWound(int heartAbility)
    {
        float output = Random.Range(0f, 100f);
        float woundingChance = ((heartAbility - 3) * 10) + 10f;

        if (output <= woundingChance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static int MagicResistBonus(int mindAbility)
    {
        return ((mindAbility - 3) * 5);
    }

    public static int RestRestoreStamina(int hoursPassed)
    {
        return 10 * hoursPassed;
    }

    public static int RestRestoreHealth(int hoursPassed)
    {
        return 5 * hoursPassed;
    }

    public static int CalculateOneManArmyDefenceBonus(int combatCount)
    {
        if(combatCount == 1)
        {
            return 0;
        }

        int percentageAdd = -5;

        //Add 5 percent for each enemy
        for (int i = 0; i < combatCount; i++)
        {
            percentageAdd += 5;
        }

        return percentageAdd;
    }

    public static int CalculateBerzerkerDamageBonus(float totalHealth, float currentHealth, int currentDamage)
    {
        float bonusPercent = ((totalHealth - currentHealth) / totalHealth) * 100;
        return (int)(currentDamage * bonusPercent / 100);
    }
}
