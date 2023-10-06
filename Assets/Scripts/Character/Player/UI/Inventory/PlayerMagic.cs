using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour
{
    [Header("References")]
    //Text headings that the spells spawn under
    [SerializeField] GameObject _ritualsParent;
    [SerializeField] GameObject _recipeParent;

    //The button prefab that spells are shown by
    [SerializeField] GameObject _spellButtonPrefab;

    //Prefab for the area effect
    [SerializeField] GameObject _spellAreaPrefab;

    //The activeUI spellslots Icons
    [SerializeField] Image _spellSlotActiveUI;
    [SerializeField] Sprite _emptySlotActiveUISprite;

    private readonly List<GameObject> _buttonsToDelete = new();

    private PlayerCharacterManager _playerCharacterManager;

    //For casting target spells
    [SerializeField] Transform _playerProjectileSpawnPoint;

    [Header("Data")]
    private Spell _currentSpell;

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
    }

    public void RefreshSpells()
    {
        //Delete all the buttons
        for (int i = _buttonsToDelete.Count - 1; i >= 0; i--)
        {
            Destroy(_buttonsToDelete[i]);
        }

        _buttonsToDelete.Clear();

        List<Spell> tempSpellList = new(_playerCharacterManager.currentSpells);

        foreach (Spell spell in tempSpellList)
        {
            PlayerMagicButton button = Instantiate(_spellButtonPrefab, _ritualsParent.transform.parent).GetComponent<PlayerMagicButton>();

            //If not a recipe put under the rituals tab, other put in recipes tab
            if (!spell.isRecipe)
            {
                button.transform.SetSiblingIndex(_ritualsParent.transform.GetSiblingIndex() + 1);
            }
            else
            {
                button.transform.SetSiblingIndex(_recipeParent.transform.GetSiblingIndex() + 1);
            }

            button.SetSpell(spell, this);

            _buttonsToDelete.Add(button.gameObject);
        }
    }

    public Spell GetCurrentSpell()
    {
        return _currentSpell;
    }

    public void SetCurrentSpell(Spell spell)
    {
        _currentSpell = spell;

        if (_currentSpell != null)
        {
            _spellSlotActiveUI.sprite = _currentSpell.spellIcon;
        }
        else
        {
            _spellSlotActiveUI.sprite = _emptySlotActiveUISprite;
        }
    }

    private bool CheckCastSpell(Spell spell)
    {
        //Check if the spell exists
        if (spell == null)
        {
            return false;
        }

        //Go through all the required items and check if the player has them
        bool costPass = true;
        foreach (Item item in spell.castingCostItems)
        {
            if (!_playerCharacterManager.currentInventory.Contains(item))
            {
                costPass = false;
            }
        }

        //if not all item requirements are met the return
        if (!costPass)
        {
            MessageBox.instance.Create("You don't have the right items to prepare this spell!", true);
            return false;
        }

        //Remove all the casting itmems
        foreach (Item item in spell.castingCostItems)
        {
            _playerCharacterManager.RemoveItem(item);
        }

        //Remove players health
        if(spell.castingHealthCost > 0)
        {
            _playerCharacterManager.DamageHealth(spell.castingHealthCost, _playerCharacterManager);
        }

        //Remove player stamina
        if(spell.castingStaminaCost > 0)
        {
            _playerCharacterManager.DamageStamina(spell.castingHealthCost);
        }

        return true;
    }

    public void CastSpell()
    {
        Spell spell = _currentSpell;

        if (spell == null || !CheckCastSpell(spell))
        {
            return;
        }

        StartCoroutine(WaitToCast(spell));
    }

    private void FinishCastingSpell(Spell spell)
    {
        if (spell.castTarget)
        {
            ProjectileController projectileController = Instantiate(spell.projectilePrefab, _playerProjectileSpawnPoint.position, _playerProjectileSpawnPoint.rotation).GetComponent<ProjectileController>();

            projectileController.spellAreaScale = spell.spellArea;

            foreach (Effect effect in spell.spellEffects)
            {
                projectileController.effects.Add(effect);
            }
        }
        else
        {
            //If the self cast spell has an area spawn the visuals and do a sphere cast
            if (spell.spellArea != 0)
            {
                GameObject areaEffect = Instantiate(_spellAreaPrefab, _playerCharacterManager.transform.position, Quaternion.identity);
                areaEffect.transform.localScale = new Vector3(spell.spellArea, spell.spellArea, spell.spellArea);
                Destroy(areaEffect, 1f);
                RaycastHit[] hits = Physics.SphereCastAll(_playerCharacterManager.transform.position, spell.spellArea, transform.forward);

                foreach (RaycastHit hit in hits)
                {
                    CharacterManager targetCharacter = hit.collider.gameObject.GetComponent<CharacterManager>();

                    //If the spell ignores the caster check if target is the player and break
                    if (!spell.effectSelf)
                    {
                        if (targetCharacter == _playerCharacterManager)
                        {
                            break;
                        }
                    }

                    if (targetCharacter != null)
                    {
                        foreach (Effect effect in spell.spellEffects)
                        {
                            targetCharacter.AddEffect(effect);
                        }
                    }
                }
            }
            //Else just apply to self
            else
            {
                foreach (Effect effect in spell.spellEffects)
                {
                    _playerCharacterManager.AddEffect(effect);
                }
            }
        }

        //Audio for spell cast
        AudioManager.instance.PlayOneShot("event:/CombatSpellCast", transform.position);
    }

    public void CraftRecipe(Spell spell)
    {
        //If the spell is a recipe just run the effects

        if (!spell.isRecipe)
        {
            return;
        }

        //Check if the recipe meets the requirements
        if (!CheckCastSpell(spell))
        {
            return;
        }

        //Add the effects to the player
        foreach (Effect effect in spell.spellEffects)
        {
            _playerCharacterManager.AddEffect(effect);
        }
    }

    IEnumerator WaitToCast(Spell spell)
    {
        _spellSlotActiveUI.color = new Color(_spellSlotActiveUI.color.r, _spellSlotActiveUI.color.g, _spellSlotActiveUI.color.b, 0.1f);
        if (!_playerCharacterManager.CheckSkill("Warlock"))
        {
            _playerCharacterManager.SetSlowState(true);
            _playerCharacterManager.SetCanAttack(false);
        }
        yield return new WaitForSeconds(spell.castingTime - (spell.castingTime * _playerCharacterManager.castingBonus / 100));
        _spellSlotActiveUI.color = new Color(_spellSlotActiveUI.color.r, _spellSlotActiveUI.color.g, _spellSlotActiveUI.color.b, 1f);
        if (!_playerCharacterManager.CheckSkill("Warlock"))
        {
            _playerCharacterManager.SetSlowState(false);
            _playerCharacterManager.SetCanAttack(true);
        }
        FinishCastingSpell(spell);
    }
}
