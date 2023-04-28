using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActionManager : Singleton<StateActionManager>
{
    //Find a specific character in scene
    public CharacterManager GetCharacter(string charName)
    {
        foreach(CharacterManager character in DataManager.instance.activeCharacters)
        {
            if(character.characterSheet.characterName == charName)
            {
                return character;
            }
        }

        return null;
    }
}
