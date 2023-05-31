using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterManager : MonoBehaviour
{
    [Header("Settings")]
    public string characterName;

    [Header("Dialogue")]
    [TextArea(1,6)]
    public string greeting;

    private void OnValidate()
    {
        if(characterName != null || characterName != "")
        {
            name = characterName.Replace(" ", "_");
        }
    }
}
