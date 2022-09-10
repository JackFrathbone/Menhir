using UnityEngine;

[CreateAssetMenu(menuName = "Magic/Spells/Base Spell")]
public class Spell : ScriptableObject
{
    [Header("Spell Data")]
    public string spellName;
    public Sprite spellIcon;
}
