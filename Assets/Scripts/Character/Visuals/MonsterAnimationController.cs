using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimationController : CharacterAnimationController
{
    [Header("References")]
    [SerializeField] SpriteRenderer _baseRenderer;
    private MonsterCharacterManager _monsterCharacterManager;

    public bool _walkCycle1;
    public bool _attacking;
    public bool _isDead;

    protected override void Awake()
    {
        _monsterCharacterManager = GetComponent<MonsterCharacterManager>();
    }

    private void Start()
    {
        _baseRenderer.sprite = _monsterCharacterManager.idleSprite;
        _baseRenderer.color = _monsterCharacterManager.spriteColorOverride;
    }

    public override void SetState(int i)
    {
        if (i == 0)
        {
            _isDead = false;
            _baseRenderer.sprite = _monsterCharacterManager.idleSprite;
        }
        else
        {
            CancelInvoke();
            _isDead = true;
            _baseRenderer.sprite = _monsterCharacterManager.deadSprite;
        }
    }

    public override void SetEquipType(int i)
    {

    }

    public override void SetShield(bool hasShield)
    {

    }

    public override void CharacterWalkingTrue()
    {
        if (!_isWalking)
        {
            InvokeRepeating(nameof(SwitchWalkFrame), 0f, 2f);
            _isWalking = true;
        }
    }

    public override void CharacterWalkingFalse()
    {
        if (_isWalking)
        {
            CancelInvoke();
            _isWalking = false;
        }
    }

    private void SwitchWalkFrame()
    {
        if (_isDead)
        {
            CancelInvoke();
        }

        if (_walkCycle1)
        {
            _baseRenderer.sprite = _monsterCharacterManager.walk1Sprite;
        }
        else
        {
            _baseRenderer.sprite = _monsterCharacterManager.walk2Sprite;
        }

        _walkCycle1 = !_walkCycle1;
    }

    protected override void CheckState()
    {

    }

    public override void StartHolding(float holdSpeed)
    {
        if (_isDead)
        {
            SetState(1);
            return;
        }

        if (!_attacking)
        {
            _baseRenderer.sprite = _monsterCharacterManager.holdSprite;
        }
        else
        {
            StartCoroutine(WaitForAttackAnim());
        }
    }

    public override void StopHolding()
    {
        _baseRenderer.sprite = _monsterCharacterManager.idleSprite;
    }

    public override void HitReaction()
    {
        _baseRenderer.sprite = _monsterCharacterManager.hurtSprite;
    }

    public override void TriggerAttack()
    {
        _attacking = true;
    }

    public override void TriggerBlock()
    {
        _baseRenderer.sprite = _monsterCharacterManager.blockSprite;
    }

    public override void UpdateCombatState(bool inCombat)
    {

    }

    IEnumerator WaitForAttackAnim()
    {
        if (_isDead)
        {
            yield return false;
        }

        _baseRenderer.sprite = _monsterCharacterManager.attackSprite;
        yield return new WaitForSeconds(1f);
        _attacking = false;
        StartHolding(0);
    }
}
