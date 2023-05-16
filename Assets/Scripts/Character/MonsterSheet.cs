using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/New Monster Sheet")]
public class MonsterSheet : ScriptableObject
{
    [Header("Monster Bio")]
    public string monsterName;

    [Header("Inventory")]
    public List<Item> monsterInventory = new();

    [Header("Combat")]
    public int health;

    public int damage;
    public int bluntDamage;
    public int range;
    public int attackSpeed;

    public bool isRanged;
    public GameObject projectilePrefab;
    public List<Effect> projectileEffects = new();

    public int defence;
    public int moveSpeed;

    [Header("World States")]
    [Tooltip("Will make the character start disabled in the world")]
    public bool startHidden = false;
    public CharacterState startState = CharacterState.alive;

    [Header("Mosnter Visuals")]
    public Sprite idleSprite;
    public Sprite walk1Sprite;
    public Sprite walk2Sprite;
    public Sprite holdSprite;
    public Sprite hitSprite;
    public Sprite blockSprite;
    public Sprite deadSprite;
}