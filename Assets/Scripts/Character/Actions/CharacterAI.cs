using System.Collections.Generic;
using UnityEngine;

//Takes in info from the characters trigger area, then decides on action to take
public class CharacterAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] bool debugMode;

    [Header("References")]
    //Used for raycasting eyesight
    private Collider characterCollider;
    private CharacterMovementController _movementController;
    private CharacterCombatController _combatController;
    private CharacterManager _characterManager;
    private CharacterAnimationController _animationController;

    private void Awake()
    {
        _movementController = GetComponent<CharacterMovementController>();
        _combatController = GetComponent<CharacterCombatController>();
        _characterManager = GetComponent<CharacterManager>();
        _animationController = GetComponentInChildren<CharacterAnimationController>();

        characterCollider = GetComponent<Collider>();

        InvokeRepeating(nameof(AIUpdate), 1f, 1f);
    }

    private void Start()
    {
        //If the player is a monster set a custom move speed
        if (_characterManager is MonsterCharacterManager)
        {
            _movementController.SetSpeed((_characterManager as MonsterCharacterManager).moveSpeed);
        }
    }

    //Used to run all the checks to update actions and check for other characters
    private void AIUpdate()
    {
        //Look for other characters
        CheckArea();
    }

    private void CheckArea()
    {
        //Get all objects in a sphere cast
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, 10f, transform.forward, 10f);

        List<CharacterManager> detectedCharacters = new();

        //Go throygh all the detected objects
        foreach (RaycastHit hitObject in hits)
        {
            CharacterManager targetCharacter;

            //If it as character
            if (targetCharacter = hitObject.transform.gameObject.GetComponent<CharacterManager>())
            {
                //Ignore if the parent is this
                if (targetCharacter.transform == this.transform)
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

                //Check unless they are dead or the same character manager
                if (targetCharacter != _characterManager && (targetCharacter.characterState != CharacterState.dead || targetCharacter.characterState != CharacterState.wounded))
                {
                    //If they pass the sneak test dont add them to detected list
                    if (targetCharacter.CheckSkill_Sneak())
                    {
                        continue;
                    }

                    //If not already in list, add the characters detection list
                    if (!_characterManager.inDetectionRange.Contains(targetCharacter))
                    {
                        _characterManager.inDetectionRange.Add(targetCharacter);
                    }

                    //If not already in list, add to the targets detection list
                    if (!targetCharacter.inDetectionRange.Contains(_characterManager))
                    {
                        targetCharacter.inDetectionRange.Add(_characterManager);
                    }

                    //Check the target for combat
                    CheckTargetCombat(targetCharacter);
                }
            }
        }

        //Check which ones from the previous detection list are not present or dead
        List<CharacterManager> charsToRemove = new();
        foreach (CharacterManager character in _characterManager.inDetectionRange)
        {
            if (!detectedCharacters.Contains(character) || character.characterState == CharacterState.dead)
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

        CheckTargetCombat(characterManager);
    }

    private void CheckTargetCombat(CharacterManager targetCharacter)
    {
        if (debugMode)
        {
            Debug.Log("Checking target if combat valid");
        }

        //Checks if hostile and valid combat target
        if (Factions.FactionHostilityCheck(_characterManager.characterFaction, targetCharacter.characterFaction, _characterManager.characterAggression))
        {
            //Send target to combat controller to deal with
            _combatController.AddCombatTarget(targetCharacter);
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
