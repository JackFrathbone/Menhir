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

    [Header("Settings")]
    [SerializeField] bool debugMode;

    [Header("References")]
    [ReadOnly] public List<CharacterManager> _targets = new();
    //Used for raycasting eyesight
    private Collider characterCollider;
    private CharacterMovementController _movementController;
    private CharacterCombatController _combatController;
    private CharacterManager _characterManager;
    private CharacterAnimationController _animationController;

    [Header("Data")]
    [ReadOnly] [SerializeField] Actions currentAction = Actions.idle;

    private void Awake()
    {
        _movementController = GetComponent<CharacterMovementController>();
        _combatController = GetComponent<CharacterCombatController>();
        _characterManager = GetComponent<CharacterManager>();
        _animationController = GetComponentInChildren<CharacterAnimationController>();

        characterCollider = GetComponent<Collider>();

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

            //If it as character
            if (targetCharacter = hitObject.transform.gameObject.GetComponent<CharacterManager>())
            {
                //Ignore if the parent is this object or if its already being tracked
                if (targetCharacter.transform == this.transform || _targets.Contains(targetCharacter))
                {
                    continue;
                }

                //Check if they are in direct line of sight otherwise skip this character
                if (Physics.Raycast(characterCollider.bounds.center, hitObject.transform.position - characterCollider.bounds.center, out RaycastHit hit, 10f))
                {
                    if (hit.transform != hitObject.transform)
                    {
                        //Char is blocked
                        continue;
                    }
                }


                if (targetCharacter != _characterManager)
                {
                    detectedCharacters.Add(targetCharacter);

                    if (!targetCharacter.CheckSkill_Sneak())
                    {
                        CheckTarget(targetCharacter);
                    }

                    if (!_characterManager.inDetectionRange.Contains(targetCharacter))
                    {
                        _characterManager.inDetectionRange.Add(targetCharacter);
                    }

                    if (!targetCharacter.inDetectionRange.Contains(_characterManager))
                    {
                        targetCharacter.inDetectionRange.Add(_characterManager);
                    }
                }
            }
        }

        //Check which ones from the previous detection list are not present
        List<CharacterManager> charsToRemove = new();
        foreach (CharacterManager character in _characterManager.inDetectionRange)
        {
            if (!detectedCharacters.Contains(character))
            {
                charsToRemove.Add(character);
            }
        }

        //Remove these non-detected ones
        foreach (CharacterManager charToRemove in charsToRemove)
        {
            charToRemove.inDetectionRange.Remove(_characterManager);
            _characterManager.inDetectionRange.Remove(charToRemove);
        }
    }

    //Used to interrupt sneaking players
    public void DetectTarget(CharacterManager characterManager)
    {
        if (debugMode)
        {
            Debug.Log("Detected sneaking character");
        }

        CheckTarget(characterManager);
    }

    private void CheckTarget(CharacterManager targetCharacter)
    {
        if (debugMode)
        {
            Debug.Log("Checking target");
        }

        //If the character is dead or wounded
        if ((targetCharacter.characterState == CharacterState.dead || targetCharacter.characterState == CharacterState.wounded) && _targets.Contains(targetCharacter))
        {
            _targets.Remove(targetCharacter);
        }

        //Checks if hostile and isnt sneaking
        if (Factions.FactionHostilityCheck(_characterManager.characterFaction, targetCharacter.characterFaction, _characterManager.characterAggression))
        {
            if (!_targets.Contains(targetCharacter) && targetCharacter.characterState == CharacterState.alive)
            {
                _targets.Add(targetCharacter);
                UpdateTargetlist();
            }
            else if (_targets.Contains(targetCharacter) && targetCharacter.characterState == CharacterState.alive)
            {
                UpdateTargetlist();
            }
        }
    }

    public void UpdateTargetlist()
    {
        if (debugMode)
        {
            Debug.Log("Update target list");
        }

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
        if (debugMode)
        {
            Debug.Log("Remove target");
        }

        if (target == null)
        {
            return;
        }

        if (_targets.Contains(target))
        {
            _targets.Remove(target);

            //Choose next target in list
            UpdateTargetlist();
        }
        else
        {
            return;
        }
    }

    private void ChangeCurrenAction(Actions action, CharacterManager target)
    {
        if (debugMode)
        {
            Debug.Log("Change current action");
        }

        currentAction = action;

        switch (action)
        {
            case Actions.idle:
                _animationController.StopHolding();
                _animationController.SetEquipType(0);
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
        foreach (CharacterManager character in _characterManager.inDetectionRange)
        {
            character.inDetectionRange.Remove(_characterManager);
        }
    }

    private void OnDestroy()
    {
        foreach (CharacterManager character in _characterManager.inDetectionRange)
        {
            character.inDetectionRange.Remove(_characterManager);
        }
    }
}
