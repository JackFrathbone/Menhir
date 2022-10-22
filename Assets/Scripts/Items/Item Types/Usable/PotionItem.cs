using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/New Potion")]
public class PotionItem : Item
{
    [Header("Potion Data")]
    public List<Effect> potionEffects = new();
}
