using UnityEngine;

//Spells items are usabled items that hold spells, when activated they are removed and the attached spell added to the player
public class SpellItem : Item
{
    [Header("Spell Data")]
    public Spell spell;
}
