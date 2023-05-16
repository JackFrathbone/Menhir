using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/New Faction")]
public class Faction : ScriptableObject
{
    [Header("Faction Data")]
    public string factionName;
    [TextArea(1,3)]
    public string factionDescription;

    [Header("Faction Relations")]
    //All factions not in these two lists are set to be neutral
    public List<Faction> alliedFactions;
    public List<Faction> hostileFactions;
}
