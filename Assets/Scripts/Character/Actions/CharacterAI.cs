using System.Collections.Generic;
using UnityEngine;

//Takes in info from the characters trigger area, then decides on action to take
public class CharacterAI : MonoBehaviour
{
    private enum Actions
    {
        idle,
        combat,
        flee
    }

    [ReadOnly] public List<CharacterManager> _targets = new();
    private readonly List<CharacterManager> _inDetectionRange = new();

    //References
    private CharacterMovementController _movementController;
    private CharacterCombatController _combatController;
    private NonPlayerCharacterManager _NPCCharacterManager;
    private CharacterAnimationController _animationController;

    private void Awake()
    {
        _movementController = GetComponent<CharacterMovementController>();
        _combatController = GetComponent<CharacterCombatController>();
        _NPCCharacterManager = GetComponent<NonPlayerCharacterManager>();
        _animationController = GetComponentInChildren<CharacterAnimationController>();

        InvokeRepeating(nameof(CheckArea), 1f, 1f);
    }

    private void CheckArea()
    {
        //Get all characters in a set sphere cast area
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10f, transform.forward, 10f);

        List<CharacterManager> detectedCharacters = new();
        foreach (RaycastHit hitObject in hits)
        {
            CharacterManager targetCharacter;
            if (targetCharacter = hitObject.transform.gameObject.GetComponent<CharacterManager>())
            {
                detectedCharacters.Add(targetCharacter);

                if (!targetCharacter.CheckSkill_Sneak())
                {
                    CheckTarget(targetCharacter);
                }

                if (!_inDetectionRange.Contains(targetCharacter))
                {
                    _inDetectionRange.Add(targetCharacter);
                }

                if (!targetCharacter.inDetectionRange.Contains(_NPCCharacterManager))
                {
                    targetCharacter.inDetectionRange.Add(_NPCCharacterManager);
                }
            }
        }

        //Check which ones from the previous detection list are not present
        List<CharacterManager> charsToRemove = new();
        foreach (CharacterManager character in _inDetectionRange)
        {
            if (!detectedCharacters.Contains(character))
            {
                charsToRemove.Add(character);
            }
        }

        //Remove these non-detected ones
        foreach (CharacterManager charToRemove in charsToRemove)
        {
            charToRemove.inDetectionRange.Remove(_NPCCharacterManager);
            _inDetectionRange.Remove(charToRemove);
        }
    }

    //Used to interrupt sneaking players
    public void DetectTarget(CharacterManager characterManager)
    {
        CheckTarget(characterManager);
    }

    private void CheckTarget(CharacterManager characterManager)
    {
        //Checks if hostile and isnt sneaking
        if (Factions.FactionHostilityCheck(_NPCCharacterManager.characterSheet.characterFaction, characterManager.characterSheet.characterFaction, _NPCCharacterManager.characterSheet.characterAggression))
        {
            if (!_targets.Contains(characterManager) && characterManager.characterState == CharacterState.alive)
            {
                _targets.Add(characterManager);
                UpdateTargetlist();
            }
            else if (_targets.Contains(characterManager) && characterManager.characterState == CharacterState.alive)
            {
                UpdateTargetlist();
            }
        }
    }

    public void UpdateTargetlist()
    {
        //If there are targets, always attack the first in the list, otherwise go back to start
        if (_targets.Count > 0 && _targets[0] != null)
        {
            ChangeCurrenAction(Actions.combat, _targets[0]);
        }
        else
        {
            ChangeCurrenAction(Actions.idle, null);
        }
    }

    public void RemoveTarget(CharacterManager target)
    {
        if (target == null)
        {
            print("null target");
            return;
        }

        if (_targets.Contains(target))
        {
            _targets.Remove(target);
            UpdateTargetlist();
        }
        else
        {
            return;
        }
    }

    private void ChangeCurrenAction(Actions action, CharacterManager target)
    {
        switch (action)
        {
            case Actions.idle:
                _animationController.StopHolding();
                print("dd");
                _combatController.StopCombat();
                _movementController.ReturnToStart();
                break;
            case Actions.combat:
                _movementController.SetTarget(target.transform);
                _combatController.StartCombat(target);
                break;
        }
    }

    private void OnDisable()
    {
        foreach(CharacterManager character in _inDetectionRange)
        {
            character.inDetectionRange.Remove(character);
        }
    }

    private void OnDestroy()
    {
        foreach (CharacterManager character in _inDetectionRange)
        {
            character.inDetectionRange.Remove(character);
        }
    }
}
