using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    //References//
    protected enum AnimState
    {
        alive,
        wounded,
        dead
    }
    protected enum EquipType
    {
        unarmed,
        oneHanded,
        twoHanded,
        ranged
    }

    [Header("References")]
    protected AnimState _animState;
    protected EquipType _equipType;
    protected bool _hasShield;
    protected Animator _characterAnimator;

    protected bool _isWalking;
    protected bool _inCombat;

    protected virtual void Awake()
    {
        _characterAnimator = GetComponent<Animator>();
    }

    public virtual void SetState(int i)
    {
        _animState = (AnimState)i;

        CheckState();
    }

    public virtual void SetEquipType(int i)
    {
        _equipType = (EquipType)i;

        CheckState();
    }

    public virtual void SetShield(bool hasShield)
    {
        _hasShield = hasShield;
    }

    public virtual void CharacterWalkingTrue()
    {
        if (!_isWalking)
        {
            _characterAnimator.SetBool("isWalking", true);
            _isWalking = true;
        }
    }

    public virtual void CharacterWalkingFalse()
    {
        if (_isWalking)
        {
            _characterAnimator.SetBool("isWalking", false);
            _isWalking = false;
        }
    }

    protected virtual void CheckState()
    {
        _characterAnimator.SetInteger("animState", (int)_animState);
        _characterAnimator.SetInteger("equipState", (int)_equipType);

        if (_inCombat)
        {
            _characterAnimator.SetBool("shieldEquip", _hasShield);
        }
        else
        {
            _characterAnimator.SetBool("isHolding", false);
            _characterAnimator.SetBool("shieldEquip", false);
        }
    }

    public virtual void StartHolding(float holdSpeed)
    {
        _characterAnimator.SetFloat("holdSpeed", holdSpeed);
        _characterAnimator.SetBool("isHolding", true);
    }

    public virtual void StopHolding()
    {
        _characterAnimator.SetBool("isHolding", false);
    }

    public virtual void HitReaction()
    {
        _characterAnimator.SetTrigger("hurtCharacter");
    }

    public virtual void TriggerAttack()
    {
        _characterAnimator.SetTrigger("attackAction");
    }

    public virtual void TriggerBlock()
    {
        _characterAnimator.SetTrigger("blockAction");
    }

    public virtual void UpdateCombatState(bool inCombat)
    {
        _inCombat = inCombat;

        _characterAnimator.SetBool("inCombat", _inCombat);

        CheckState();
    }
}
