using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDataTracker
{
    [Header("Identification")]
    public string characterName;

    [Header("Location")]
    public Vector3 characterPosition;

    [Header("Status")]
    public float healthCurrent;
    public float staminaCurrent;

    [Header("Inventory")]
    public List<string> currentInventory = new();

    [Header("Active Effects")]
    public List<Effect> currentEffects = new();

    [Header("Character States")]
    public CharacterState characterState;
}
