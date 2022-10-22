using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour
{
    [Header("References")]
    //Text headings that the spells spawn under
    [SerializeField] GameObject _ritualsParent;
    [SerializeField] GameObject _singleUseParent;

    //The button prefab that spells are shown by
    [SerializeField] GameObject _spellButtonPrefab;

    //The buttons that show prepared and free spells
    [SerializeField] Button _spellSlot1;
    [SerializeField] Button _spellSlot2;
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

    private void Start()
    {
        _playerMagicDescription = GetComponent<PlayerMagicDescription>();

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
        //Sort out the casting cost if the spell hasnt been learned
        if (!CheckSpellLearned(spell))
        {
            bool costPass = true;

            foreach(Item item in spell.castingCostItems)
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

    public void LearnSpell(Spell spell)
    {
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
            _freeSpell2 = null;
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
        Spell spell = null;

        if(spellSlot == 1)
        {
            spell = _preparedSpell1;
        }
        else if(spellSlot == 2)
        {
            spell = _preparedSpell2;
        }

        if(spell == null)
        {
            return;
        }

        if (CheckSpellPrepared(spell))
        {
            if (spell.castTarget)
            {
                ProjectileController projectileController = Instantiate(spell.projectilePrefab, _playerProjectileSpawnPoint.position, _playerProjectileSpawnPoint.rotation).GetComponent<ProjectileController>();

                foreach (Effect effect in spell.spellEffects)
                {
                    projectileController.effects.Add(effect);
                }
            }
            else
            {
                foreach(Effect effect in spell.spellEffects)
                {
                    effect.AddEffect(_playerCharacterManager);
                }
            }

            if (_preparedSpell1 == spell)
            {
                _preparedSpell1 = null;
            }
            else if (_preparedSpell2 == spell)
            {
                _preparedSpell2 = null;
            }
        }
        else
        {
            return;
        }
        RefreshPreparedSpells();
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
}
