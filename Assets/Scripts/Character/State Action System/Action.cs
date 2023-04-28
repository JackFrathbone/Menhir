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
    public string targetName;

    [Header("Set Data")]
    public bool setHidden;
    public Aggression setAggression;
    public Faction setFaction;
    public CharacterState setCharacterState;
    public Vector3 setPosition;
    public Vector3 setMoveTarget;
    public Item addItem;


    private StateActionManager _actionManager;

    public virtual void StartAction()
    {
        _actionManager = StateActionManager.instance;
        RunAction();
    }

    protected virtual void RunAction()
    {
        NonPlayerCharacterManager targetCharacter = StateActionManager.instance.GetCharacter(targetName).GetComponent<NonPlayerCharacterManager>();

        switch (actionType)
        {
            case ActionType.Set_Char_Hidden:
                targetCharacter.isHidden = setHidden;
                break;
            case ActionType.Set_Char_Aggresion:
                targetCharacter.characterSheet.characterAggression = setAggression;
                break;
            case ActionType.Set_Char_Faction:
                targetCharacter.characterSheet.characterFaction = setFaction;
                break;
            case ActionType.Set_Char_State:
                targetCharacter.characterState = setCharacterState;
                targetCharacter.SetCharacterState();
                break;
            case ActionType.Set_Char_Position:
                targetCharacter.transform.position = setPosition;
                break;
            case ActionType.Set_Char_MoveTarget:
                CharacterMovementController charMover = targetCharacter.GetComponent<CharacterMovementController>();
                if(charMover != null)
                {
                    charMover.MoveToPosition(setMoveTarget);
                }
                break;
            case ActionType.Add_Char_Item:
                targetCharacter.AddItem(addItem);
                break;
        }
    }
}
