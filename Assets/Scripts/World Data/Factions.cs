using System.Collections.Generic;
using UnityEngine;

public enum Aggression
{
    //Will never attack
    Passive,
    //Will/won't attack based on faction relationship
    Neutral,
    //Will always attack
    Hostile
};


public static class Factions
{
    public static bool FactionHostilityCheck(Faction characterFaction, Faction targetFaction, Aggression characterAggresion)
    {
        if(characterAggresion == Aggression.Hostile)
        {
            return true;
        }
        else if(characterAggresion == Aggression.Passive)
        {
            return false;
        }

        if(targetFaction == null)
        {
            return false;
        }

        if (characterFaction.hostileFactions.Contains(targetFaction) || targetFaction.hostileFactions.Contains(characterFaction))
        {
            return true;
        }

        return false;
    }
}
