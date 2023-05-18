using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Set_Char_Hidden,
    Set_Char_Aggresion,
    Set_Char_Faction,
    Set_Char_State,
    Set_Char_Position,
    Set_Char_MoveTarget,
    Add_Char_Item
}

[System.Serializable]
public class Action
{
    [Header("Target Settings")]
    public ActionType actionType;
    public CharacterSheet targetChar;

    [Header("Set Data")]
    public bool setHidden;
    public Aggression setAggression;
    public Faction setFaction;
    public CharacterState setCharacterState;
    public Vector3 setPosition;
    public Vector3 setMoveTarget;
    public Item addItem;

    public virtual void StartAction()
    {
        RunAction();
    }

    protected virtual void RunAction()
    {
        List<NonPlayerCharacterManager> targetCharacters = StateActionManager.instance.GetCharactersFromSheet(targetChar);

        if (targetCharacters == null || targetCharacters.Count == 0)
        {
            Debug.Log("No valid character");
            return;
        }

        foreach (NonPlayerCharacterManager targetChar in targetCharacters)
        {
            switch (actionType)
            {
                case ActionType.Set_Char_Hidden:
                    targetChar.isHidden = setHidden;
                    break;
                case ActionType.Set_Char_Aggresion:
                    targetChar.characterAggression = setAggression;
                    break;
                case ActionType.Set_Char_Faction:
                    targetChar.characterFaction = setFaction;
                    break;
                case ActionType.Set_Char_State:
                    targetChar.characterState = setCharacterState;
                    targetChar.SetCharacterState();
                    break;
                case ActionType.Set_Char_Position:
                    targetChar.transform.position = setPosition;
                    break;
                case ActionType.Set_Char_MoveTarget:
                    CharacterMovementController charMover = targetChar.GetComponent<CharacterMovementController>();
                    if (charMover != null)
                    {
                        charMover.MoveToPosition(setMoveTarget);
                    }
                    break;
                case ActionType.Add_Char_Item:
                    targetChar.AddItem(addItem);
                    break;
            }
        }
    }
}
