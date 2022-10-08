using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    //References//
    private enum AnimState
    {
        alive,
        wounded,
        dead
    }
    private enum EquipType
    {
        unarmed,
        oneHanded,
        twoHanded,
        ranged
    }

    private AnimState _animState;
    private EquipType _equipType;
    private bool _hasShield;
    private Animator _characterAnimator;

    private void Awake()
    {
        _characterAnimator = GetComponent<Animator>();
    }

    public void SetState(int i)
    {
        _animState = (AnimState)i;

        CheckState();
    }

    public void SetEquipType(int i)
    {
        _equipType = (EquipType)i;
    }

    public void SetShield(bool hasShield)
    {
        _hasShield = hasShield;
    }

    public void CharacterWalkingTrue()
    {
        _characterAnimator.SetBool("isWalking", true);
    }

    public void CharacterWalkingFalse()
    {
        _characterAnimator.SetBool("isWalking", false);
    }

    private void CheckState()
    {
        _characterAnimator.SetInteger("animState", (int)_animState);
        _characterAnimator.SetInteger("equipState", (int)_equipType);
    }

    public void StartHolding()
    {
        _characterAnimator.SetBool("isHolding", true);
    }

    public void StopHolding()
    {
        _characterAnimator.SetBool("isHolding", false);
    }

    public void HitReaction()
    {
        _characterAnimator.SetTrigger("hurtCharacter");
    }

    public void TriggerAttack()
    {
        _characterAnimator.SetTrigger("attackAction");
    }

    public void TriggerBlock()
    {
        _characterAnimator.SetTrigger("blockAction");
    }

    public void TriggerDodge()
    {
        //_characterAnimator.SetTrigger("dodgeAction");
    }
}
