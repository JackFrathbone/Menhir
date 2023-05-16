using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateActionManager : Singleton<StateActionManager>
{
    //Find all characters using the specific character sheet
    public List<NonPlayerCharacterManager> GetCharactersFromSheet(CharacterSheet characterSheet)
    {
        List<NonPlayerCharacterManager> characters = DataManager.instance.GetActiveCharacters();
        List<NonPlayerCharacterManager> actionCharacters = new();

        if (characters.Count == 0)
        {
            Debug.Log("No characters in scene");
            return null;
        }

        foreach(CharacterManager character in characters)
        {
            if(character.characterName == characterSheet.characterName && character is NonPlayerCharacterManager)
            {
                actionCharacters.Add(character as NonPlayerCharacterManager);
            }
        }

        return actionCharacters;
    }
}
