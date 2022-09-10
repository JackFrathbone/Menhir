using System.Collections.Generic;
using UnityEngine;

//Takes in info from the characters trigger area, then decides on action to take
public class CharacterAI : MonoBehaviour
{
    private enum _actions
    {
        idle,
        combat,
        flee
    }

    [SerializeField] List<CharacterManager> _targets = new List<CharacterManager>();

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
    }

    private void OnTriggerEnter(Collider other)
    {
        //If character enters area
        if (other.gameObject.GetComponentInParent<CharacterManager>())
        {
            CharacterManager targetChar = other.gameObject.GetComponentInParent<CharacterManager>();
            //Checks if hostile
            if (Factions.FactionHostilityCheck(_NPCCharacterManager.characterSheet.characterFaction, targetChar.characterSheet.characterFaction, _NPCCharacterManager.characterSheet.characterAggression))
            {
                if (!_targets.Contains(targetChar) && targetChar.characterState == CharacterState.alive)
                {
                    _targets.Add(targetChar);
                    UpdateTargetlist();
                }
            }
        }
    }

    private void UpdateTargetlist()
    {
        //If there are targets, always attack the first in the list, otherwise go back to start
        if(_targets.Count > 0 && _targets[0] != null)
        {
            ChangeCurrenAction(_actions.combat, _targets[0]);
        }
        else
        {
            ChangeCurrenAction(_actions.idle, null);
        }
    }

    public void RemoveTarget(CharacterManager target)
    {
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

    private void ChangeCurrenAction(_actions action, CharacterManager target)
    {
        switch (action)
        {
            case _actions.idle:
                _animationController.StopHolding();
                _combatController.StopCombat();
                _movementController.ReturnToStart();
                break;
            case _actions.combat:
                _movementController.SetTarget(target.transform);
                _combatController.StartCombat(target);
                break;
        }
    }
}
