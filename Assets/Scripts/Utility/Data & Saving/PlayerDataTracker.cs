using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataTracker
{
    [Header("Character Sheet")]
    [ReadOnly] public string playerName;
    [ReadOnly] public int pronounInt;

    [ReadOnly] public string colorHair; 

    [ReadOnly] public string hairSprite;
    [ReadOnly] public string beardSprite;

    [Header("Location")]
    [ReadOnly] public int currentScene;
    [ReadOnly] public Vector3 characterPosition;
    [ReadOnly] public Vector3 characterRotation;

    [Header("Status")]
    [ReadOnly] public float healthCurrent;
    [ReadOnly] public float staminaCurrent;

    //Abilities
    [ReadOnly] public int bodyLevel;
    [ReadOnly] public int handsLevel;
    [ReadOnly] public int mindLevel;
    [ReadOnly] public int heartLevel;

    [Header("Inventory/Magic/Skills")]
    [ReadOnly] public List<string> currentInventory = new();
    [ReadOnly] public List<string> currentSpells = new();
    [ReadOnly] public List<string> currentSkills = new();

    [Header("Equipped")]
    [ReadOnly] public string equippedWeapon;
    [ReadOnly] public string equippedShield;

    [ReadOnly] public string equippedArmour;
    [ReadOnly] public string equippedCape;
    [ReadOnly] public string equippedFeet;
    [ReadOnly] public string equippedGreaves;
    [ReadOnly] public string equippedHands;
    [ReadOnly] public string equippedHelmet;
    [ReadOnly] public string equippedPants;
    [ReadOnly] public string equippedShirt;

    [ReadOnly] public string equippedSpell1;
    [ReadOnly] public string equippedSpell2;

    [ReadOnly] public string learnedSpell1;
    [ReadOnly] public string learnedSpell2;

    [Header("Active Effects")]
    [ReadOnly] public List<Effect> currentEffects = new();

    [Header("Quests & States")]
    [ReadOnly] public List<string> quests = new();
    [ReadOnly] public List<string> stateChecks = new();
    [ReadOnly] public List<string> alreadyRunDialogueTopics = new();

    [Header("World Data")]
    [ReadOnly] public int currentDay;
    [ReadOnly] public int currentHour;
    [ReadOnly] public int currentMinute;
}
