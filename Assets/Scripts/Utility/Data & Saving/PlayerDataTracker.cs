using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataTracker
{
    [Header("Location")]
    public int currentScene;
    public Vector3 characterPosition;
    public Vector3 characterRotation;

    [Header("Status")]
    public float healthCurrent;
    public float staminaCurrent;

    public Abilities abilities;

    [Header("Inventory/Magic/Skills")]
    public List<Item> currentInventory = new();
    public List<Spell> currentSpells = new();
    public List<Skill> currentSkills = new();

    [Header("Equipped")]
    public Item equippedWeapon;
    public ShieldItem equippedShield;

    public EquipmentItem equippedArmour;
    public EquipmentItem equippedCape;
    public EquipmentItem equippedFeet;
    public EquipmentItem equippedGreaves;
    public EquipmentItem equippedHands;
    public EquipmentItem equippedHelmet;
    public EquipmentItem equippedPants;
    public EquipmentItem equippedShirt;

    public Spell equippedSpell1;
    public Spell equippedSpell2;

    public Spell learnedSpell1;
    public Spell learnedSpell2;

    [Header("Active Effects")]
    public List<Effect> currentEffects = new();

    [Header("Quests & States")]
    public List<Quest> quests = new();
    public List<StateCheck> stateChecks = new();
    public List<DialogueTopicsNode.Topic> alreadyRunDialogueTopics = new();

    [Header("World Data")]
    public int currentDay;
    public int currentHour;
    public int currentMinute;
}
