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

    public void SetVisuals(NonPlayerCharacterManager charManager)
    {
        SetBaseVisuals(charManager);
        SetEquipmentVisuals(charManager);
    }

    private void SetBaseVisuals(NonPlayerCharacterManager charManager)
    {
        //For the skintone
        _baseRenderer.color = charManager.characterSkintone;
        _armRenderer.color = charManager.characterSkintone;
        _legRenderer.color = charManager.characterSkintone;
        _handsRenderer.color = charManager.characterSkintone;
        _feetRenderer.color = charManager.characterSkintone;

        //For the hair
        _hairRenderer.color = charManager.characterHairColor;
        _beardRenderer.color = charManager.characterHairColor;

        //Currently only randomise the hair and bear options
        if (charManager.randomiseVisuals)
        {
            _hairRenderer.sprite = _spriteDatabase.GetRandomHair();

            _beardRenderer.sprite = _spriteDatabase.GetRandomBeard();
        }
        else
        {
            _hairRenderer.sprite = charManager.characterHair;
            _beardRenderer.sprite = charManager.characterBeard;
        }


    }

    private void SetEquipmentVisuals(NonPlayerCharacterManager charManager)
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

            //Hide the hair if helmet equipped
            _hairRenderer.sprite = null;
        }
        else
        {
            _helmetRenderer.sprite = null;
            _helmetRenderer.color = Color.clear;
        }

        //Below are the items which dont have visuals but instead use built in animated and recolorable skins//
        if (charManager.equippedShirt != null)
        {
            _shirtRenderer.sprite = charManager.equippedShirt.equipmentModel;
            _shirtRenderer.color = charManager.equippedShirt.equipmentColor;

            ///Set the arm color to match shirt
            _armRenderer.color = charManager.equippedShirt.equipmentColor;
        }
        else
        {
            _shirtRenderer.sprite = null;
            _shirtRenderer.color = Color.clear;
        }

        if (charManager.equippedPants != null)
        {
            _legRenderer.color = charManager.equippedPants.equipmentColor;
        }
        else
        {
            _legRenderer.color = charManager.characterSkintone;
        }

        if (charManager.equippedHands != null)
        {
            _handsRenderer.color = charManager.equippedHands.equipmentColor;
        }
        else
        {
            _handsRenderer.color = charManager.characterSkintone;
        }

        if (charManager.equippedFeet != null)
        {
            _feetRenderer.color = charManager.equippedFeet.equipmentColor;
        }
        else
        {
            _feetRenderer.color = charManager.characterSkintone;
        }
    }
}
