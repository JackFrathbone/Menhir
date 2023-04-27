using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDatabase : ScriptableObject
{
    [Header("Character Visuals")]
    public List<Sprite> hairSprites;
    public List<Sprite> beardSprites;

    public Sprite GetHairFromName(string name)
    {
        foreach (Sprite sprite in hairSprites)
        {
            if(sprite.name == name)
            {
                return sprite;
            }
        }

        Debug.Log("Sprite does not exist");
        return null;
    }

    public Sprite GetBeardFromName(string name)
    {
        foreach (Sprite sprite in beardSprites)
        {
            if (sprite.name == name)
            {
                return sprite;
            }
        }

        Debug.Log("Sprite does not exist");
        return null;
    }
}
