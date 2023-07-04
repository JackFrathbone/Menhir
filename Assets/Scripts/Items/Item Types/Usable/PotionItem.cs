using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Potion")]
public class PotionItem : Item
{
    [Header("Potion Data")]
    public List<Effect> potionEffects = new();

    public string GetEffectsDescription()
    {
        string description = "Casts: ";

        foreach (Effect effect in potionEffects)
        {
            description += effect.GetDescription() + "<br>";
        }

        return description;
    }
}
