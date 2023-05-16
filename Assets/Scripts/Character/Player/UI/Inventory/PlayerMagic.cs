using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour
{
    [Header("Settings")]
    //For spell slot 1 or 2
    private int _selectedSpell;

    [Header("References")]
    //Text headings that the spells spawn under
    [SerializeField] GameObject _ritualsParent;
    [SerializeField] GameObject _singleUseParent;

    //The button prefab that spells are shown by
    [SerializeField] GameObject _spellButtonPrefab;

    //Prefab for the area effect
    [SerializeField] GameObject _spellAreaPrefab;

    //The buttons that show prepared and free spells
    [SerializeField] Button _spellSlot1;
    [SerializeField] Button _spellSlot2;

    //Used to disable the warlock buttons if the player doesnt have the ability
    [SerializeField] GameObject _warlockParent;

    [SerializeField] Button _freeSpellSlot1;
    [SerializeField] Button _freeSpellSlot2;

    //The activeUI spellslots Icons
    [SerializeField] Image _spellSlot1ActiveUI;
    [SerializeField] Image _spellSlot2ActiveUI;
    [SerializeField] Sprite _emptySlotActiveUISprite;

    private readonly List<GameObject> _buttonsToDelete = new();

    private PlayerMagicDescription _playerMagicDescription;
    private PlayerCharacterManager _playerCharacterManager;

    //For casting target spells
    [SerializeField] Transform _playerProjectileSpawnPoint;

    [Header("Data")]
    private Spell _preparedSpell1;
    private Spell _preparedSpell2;
    private Spell _freeSpell1;
    private Spell _freeSpell2;

    private bool _canCastSlot1 = true;
    private bool _canCastSlot2 = true;

    //The time it takes to be able to cast a spell again
    [SerializeField] float _castTimeDefault;

    private void Start()
    {
        _playerMagicDescription = GetComponent<PlayerMagicDescription>();

        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();

        SetSelectedSpell(1);

        _warlockParent.SetActive(false);
    }

    public void RefreshSpells()
    {
        if (_playerCharacterManager.CheckSkill("Warlock"))
        {
            _warlockParent.SetActive(true);
        }

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
            button.transform.SetSiblingIndex(_ritualsParent.transform.GetSiblingIndex() + 1);
            button.SetSpell(spell, this);

            _buttonsToDelete.Add(button.gameObject);
        }
    }

    public void RefreshPreparedSpells()
    {
        RefreshSpells();

        if (_preparedSpell1 != null)
        {
            _spellSlot1.gameObject.SetActive(true);

            _spellSlot1.onClick.RemoveAllListeners();

            _spellSlot1.image.sprite = _preparedSpell1.spellIcon;
            _spellSlot1.onClick.AddListener(delegate { _playerMagicDescription.SetDescription(_preparedSpell1); });

            _spellSlot1ActiveUI.sprite = _preparedSpell1.spellIcon;
        }
        else
        {
            _spellSlot1.gameObject.SetActive(false);
            _spellSlot1ActiveUI.sprite = _emptySlotActiveUISprite;
        }

        if (_preparedSpell2 != null)
        {
            _spellSlot2.gameObject.SetActive(true);

            _spellSlot2.onClick.RemoveAllListeners();

            _spellSlot2.image.sprite = _preparedSpell2.spellIcon;
            _spellSlot2.onClick.AddListener(delegate { _playerMagicDescription.SetDescription(_preparedSpell2); });

            _spellSlot2ActiveUI.sprite = _preparedSpell2.spellIcon;
        }
        else
        {
            _spellSlot2.gameObject.SetActive(false);
            _spellSlot2ActiveUI.sprite = _emptySlotActiveUISprite;
        }
    }

    public void RefreshLearnedSpells()
    {
        RefreshSpells();

        if (_freeSpell1 != null)
        {
            _freeSpellSlot1.gameObject.SetActive(true);

            _freeSpellSlot1.onClick.RemoveAllListeners();

            _freeSpellSlot1.image.sprite = _freeSpell1.spellIcon;
            _freeSpellSlot1.onClick.AddListener(delegate { _playerMagicDescription.SetDescription(_freeSpell1); });
        }
        else
        {
            _freeSpellSlot1.gameObject.SetActive(false);
        }

        if (_freeSpell2 != null)
        {
            _freeSpellSlot2.gameObject.SetActive(true);

            _freeSpellSlot2.onClick.RemoveAllListeners();

            _freeSpellSlot2.image.sprite = _freeSpell2.spellIcon;
            _freeSpellSlot2.onClick.AddListener(delegate { _playerMagicDescription.SetDescription(_freeSpell2); });
        }
        else
        {
            _freeSpellSlot2.gameObject.SetActive(false);
        }
    }

    public bool CheckSpellPrepared(Spell spell)
    {
        if (spell == _preparedSpell1 || spell == _preparedSpell2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckSpellLearned(Spell spell)
    {
        if (spell == _freeSpell1 || spell == _freeSpell2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PrepareSpell(Spell spell)
    {
        if (spell == null)
        {
            return;
        }

        //Sort out the casting cost if the spell hasnt been learned
        if (!CheckSpellLearned(spell))
        {
            bool costPass = true;

            foreach (Item item in spell.castingCostItems)
            {
                if (!_playerCharacterManager.currentInventory.Contains(item))
                {
                    costPass = false;
                }
            }

            if (!costPass)
            {
                return;
            }

            foreach (Item item in spell.castingCostItems)
            {
                _playerCharacterManager.RemoveItem(item);
            }
        }

        if (_preparedSpell1 == null)
        {
            _preparedSpell1 = spell;
        }
        else if (_preparedSpell2 == null)
        {
            _preparedSpell2 = spell;
        }
        else
        {
            _preparedSpell1 = spell;
        }

        RefreshPreparedSpells();
    }

    public void UnPrepareSpell(Spell spell)
    {
        if (_preparedSpell1 == spell)
        {
            _preparedSpell1 = null;
        }
        else if (_preparedSpell2 == spell)
        {
            _preparedSpell2 = null;
        }

        RefreshPreparedSpells();
    }

    public void LearnSpell(Spell spell)
    {
        if (spell == null)
        {
            return;
        }

        bool costPass = true;

        //Check if player has the items needed
        foreach (Item item in spell.castingCostItems)
        {
            if (!_playerCharacterManager.currentInventory.Contains(item))
            {
                costPass = false;
            }
        }

        if (!costPass)
        {
            return;
        }

        //Remove the items for the spell
        foreach (Item item in spell.castingCostItems)
        {
            _playerCharacterManager.RemoveItem(item);
        }

        //Assign the spell to a learned spell slot
        if (_freeSpell1 == null)
        {
            _freeSpell1 = spell;
        }
        else if (_freeSpell2 == null)
        {
            _freeSpell2 = spell;
        }

        RefreshLearnedSpells();
    }

    public void UnLearnSpell(Spell spell)
    {
        if (_freeSpell1 == spell)
        {
            _freeSpell1 = null;
        }
        else if (_freeSpell2 == spell)
        {
            _freeSpell2 = null;
        }

        RefreshLearnedSpells();
    }

    //Spell slot 1 or 2
    public void CastSpell(int spellSlot)
    {
        if ((spellSlot == 1 && !_canCastSlot1) || (spellSlot == 2 && !_canCastSlot2))
        {
            return;
        }

        Spell spell = null;

        if (spellSlot == 1)
        {
            spell = _preparedSpell1;
        }
        else if (spellSlot == 2)
        {
            spell = _preparedSpell2;
        }

        if (spell == null)
        {
            return;
        }

        if (CheckSpellPrepared(spell))
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
                            if(targetCharacter == _playerCharacterManager)
                            {
                                break;
                            }
                        }

                        if (targetCharacter != null)
                        {
                            foreach (Effect effect in spell.spellEffects)
                            {
                                effect.AddEffect(targetCharacter);
                            }
                        }
                    }
                }
                //Else just apply to self
                else
                {
                    foreach (Effect effect in spell.spellEffects)
                    {
                        effect.AddEffect(_playerCharacterManager);
                    }
                }
            }

            if (_preparedSpell1 == spell && _preparedSpell1 != _freeSpell1 && _preparedSpell1 != _freeSpell2)
            {
                _preparedSpell1 = null;
            }
            else if (_preparedSpell2 == spell && _preparedSpell2 != _freeSpell1 && _preparedSpell1 != _freeSpell2)
            {
                _preparedSpell2 = null;
            }
        }
        else
        {
            return;
        }

        RefreshPreparedSpells();

        //Audio for spell cast
        AudioManager.instance.PlayOneShot("event:/CombatSpellCast", transform.position);

        //Set the spell slot to be disabled
        if (spellSlot == 1)
        {
            _canCastSlot1 = false;
        }
        else
        {
            _canCastSlot2 = false;
        }

        StartCoroutine(WaitToCastAgain(spell, spellSlot));

    }

    public bool CheckWarlockSkill()
    {
        if (_playerCharacterManager.CheckSkill("Warlock"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetSelectedSpell(int i)
    {
        _selectedSpell = i;

        if (i == 1 && _preparedSpell1 != null)
        {
            _spellSlot1ActiveUI.color = Color.green;
            _spellSlot2ActiveUI.color = Color.white;
        }
        else if (i == 2 && _preparedSpell2 != null)
        {
            _spellSlot1ActiveUI.color = Color.white;
            _spellSlot2ActiveUI.color = Color.green;
        }
        else
        {
            _spellSlot1ActiveUI.color = Color.white;
            _spellSlot2ActiveUI.color = Color.white;
        }
    }

    public Spell GetEquippedSpell(int slot)
    {
        if (slot == 1)
        {
            return _preparedSpell1;
        }
        else if (slot == 2)
        {
            return _preparedSpell2;
        }
        else
        {
            Debug.Log("Invalid spell slot");
            return null;
        }
    }

    public Spell GetLearnedSpell(int slot)
    {
        if (slot == 1)
        {
            return _freeSpell1;
        }
        else if (slot == 2)
        {
            return _freeSpell2;
        }
        else
        {
            Debug.Log("Invalid spell slot");
            return null;
        }
    }

    public int GetSelectedSpell()
    {
        return _selectedSpell;
    }

    IEnumerator WaitToCastAgain(Spell spell, int spellSlot)
    {
        if (spellSlot == 1)
        {
            _spellSlot1ActiveUI.color = new Color(_spellSlot1ActiveUI.color.r, _spellSlot1ActiveUI.color.g, _spellSlot1ActiveUI.color.b, 0.1f);
        }
        else
        {
            _spellSlot2ActiveUI.color = new Color(_spellSlot2ActiveUI.color.r, _spellSlot2ActiveUI.color.g, _spellSlot2ActiveUI.color.b, 0.1f);
        }
        yield return new WaitForSeconds(spell.castingTime);
        if (spellSlot == 1)
        {
            _canCastSlot1 = true;
            _spellSlot1ActiveUI.color = new Color(_spellSlot1ActiveUI.color.r, _spellSlot1ActiveUI.color.g, _spellSlot1ActiveUI.color.b, 1f);
        }
        else
        {
            _canCastSlot2 = true;
            _spellSlot2ActiveUI.color = new Color(_spellSlot2ActiveUI.color.r, _spellSlot2ActiveUI.color.g, _spellSlot2ActiveUI.color.b, 1f);
        }
    }
}
