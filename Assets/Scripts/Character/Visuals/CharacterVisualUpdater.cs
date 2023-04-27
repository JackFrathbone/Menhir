using System.Collections.Generic;
using UnityEngine;

public class CharacterVisualUpdater : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SpriteDatabase _spriteDatabase;

    [Header("Body Renderers")]
    [SerializeField] SpriteRenderer _baseRenderer;
    [SerializeField] SpriteRenderer _armRenderer;
    [SerializeField] SpriteRenderer _handsRenderer;
    [SerializeField] SpriteRenderer _legRenderer;
    [SerializeField] SpriteRenderer _feetRenderer;
    [SerializeField] SpriteRenderer _hairRenderer;
    [SerializeField] SpriteRenderer _beardRenderer;

    [Header("Equipment Renderers")]
    [SerializeField] SpriteRenderer _armourRenderer;
    [SerializeField] SpriteRenderer _capeRenderer;
    [SerializeField] SpriteRenderer _capefrontRenderer;
    [SerializeField] SpriteRenderer _greavesRenderer;
    [SerializeField] SpriteRenderer _helmetRenderer;
    [SerializeField] SpriteRenderer _shirtRenderer;

    public void SetVisuals(CharacterManager charManager)
    {
        SetBaseVisuals(charManager.characterSheet);
        SetEquipmentVisuals(charManager);
    }

    private void SetBaseVisuals(CharacterSheet charSheet)
    {
        //For the skintone
        _baseRenderer.color = charSheet.characterSkintone;
        _armRenderer.color = charSheet.characterSkintone;
        _legRenderer.color = charSheet.characterSkintone;
        _handsRenderer.color = charSheet.characterSkintone;
        _feetRenderer.color = charSheet.characterSkintone;

        //For the hair
        _hairRenderer.color = charSheet.characterHairColor;
        _beardRenderer.color = charSheet.characterHairColor;

        //Currently only randomise the hair and bear options
        if (charSheet.randomiseVisuals)
        {
            int randomHair = _spriteDatabase.hairSprites.Count + 1;
            if(randomHair <= _spriteDatabase.hairSprites.Count)
            {
                _hairRenderer.sprite = _spriteDatabase.hairSprites[randomHair];
            }

            int randomBeard = _spriteDatabase.beardSprites.Count + 1;
            if (randomBeard <= _spriteDatabase.beardSprites.Count)
            {
                _beardRenderer.sprite = _spriteDatabase.hairSprites[randomHair];
            }
        }
        else
        {
            _hairRenderer.sprite = charSheet.characterHair;
            _beardRenderer.sprite = charSheet.characterBeard;
        }


    }

    private void SetEquipmentVisuals(CharacterManager charManager)
    {
        //Equipment with visual models
        if (charManager.equippedArmour != null)
        {
            _armourRenderer.sprite = charManager.equippedArmour.equipmentModel;
            _armourRenderer.color = charManager.equippedArmour.equipmentColor;
        }
        else
        {
            _armourRenderer.sprite = null;
            _armourRenderer.color = Color.clear;
        }

        //Cape has a front piece which needs to be enabled
        if (charManager.equippedCape != null)
        {
            _capeRenderer.sprite = charManager.equippedCape.equipmentModel;
            _capeRenderer.color = charManager.equippedCape.equipmentColor;

            _capefrontRenderer.color = charManager.equippedCape.equipmentColor;
        }
        else
        {
            _capeRenderer.sprite = null;
            _capeRenderer.color = Color.clear;
            _capefrontRenderer.color = Color.clear;
        }

        if (charManager.equippedGreaves != null)
        {
            _greavesRenderer.sprite = charManager.equippedGreaves.equipmentModel;
            //_greavesRenderer.color = charManager.equippedGreaves.equipmentColor;
        }
        else
        {
            _greavesRenderer.sprite = null;
            //_greavesRenderer.color = Color.clear;
        }

        if (charManager.equippedHelmet != null)
        {
            _helmetRenderer.sprite = charManager.equippedHelmet.equipmentModel;
            _helmetRenderer.color = charManager.equippedHelmet.equipmentColor;
        }
        else
        {
            _helmetRenderer.sprite = null;
            _helmetRenderer.color = Color.clear;
        }

        //Below are the items which dont have visuals but instead use built in animated and recolorable skins//
        if (charManager.equippedShirt != null)
        {
            _shirtRenderer.color = charManager.equippedShirt.equipmentColor;
        }
        else
        {
            _shirtRenderer.color = Color.clear;
        }

        if (charManager.equippedPants != null)
        {
            _legRenderer.color = charManager.equippedPants.equipmentColor;
        }
        else
        {
            _legRenderer.color = charManager.characterSheet.characterSkintone;
        }

        if (charManager.equippedHands != null)
        {
            _handsRenderer.color = charManager.equippedHands.equipmentColor;
        }
        else
        {
            _handsRenderer.color = charManager.characterSheet.characterSkintone;
        }

        if (charManager.equippedFeet != null)
        {
            _feetRenderer.color = charManager.equippedFeet.equipmentColor;
        }
        else
        {
            _feetRenderer.color = charManager.characterSheet.characterSkintone;
        }
    }
}
