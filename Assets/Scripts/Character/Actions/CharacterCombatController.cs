using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _bloodSplatterPrefab;
    [SerializeField] Transform _projectileSpawn;

    private CharacterAnimationController _animationController;
    private CharacterMovementController _movementController;
    private CharacterManager _characterManager;
    private CharacterAI _characterAI;

    private CharacterManager _combatTarget;
    private int _holdDefence;

    private int _weaponDamage;
    private int _weaponBluntDamage;
    private float _weaponRange;
    private float _weaponSpeed;
    private int _weaponDefence;

    private float _itemWeight;

    private bool _isRanged;
    private GameObject _projectilePrefab;
    private List<Effect> _projectileEffects;

    private void Awake()
    {
        _animationController = GetComponentInChildren<CharacterAnimationController>();
        _movementController = GetComponent<CharacterMovementController>();
        _characterManager = GetComponent<CharacterManager>();
        _characterAI = GetComponent<CharacterAI>();
    }

    public void StartCombat(CharacterManager target)
    {
        _combatTarget = target;
        //Set character manager to combat for skills purpose
        _characterManager.StartCombat(_combatTarget);

        //Update the visuals
        _animationController.UpdateCombatState(true);

        DecideNextAction();

        SetWeaponStats();
        _movementController.SetTargetDistance(_weaponRange);
    }

    public void StopCombat()
    {
        StopAllCoroutines();

        //Update the animation state
        _animationController.UpdateCombatState(false);

        //Remove combat target from the characters list
        _characterManager.StopCombat(_combatTarget);

        //Tell the AI that target is out of combat and should be removed
        _characterAI.RemoveTarget(_combatTarget);

        _combatTarget = null;
    }

    private void SetWeaponStats()
    {
        _characterManager.GetCurrentWeaponStats(out _weaponDamage, out _weaponBluntDamage, out _weaponDefence, out _weaponRange, out _weaponSpeed, out _isRanged, out _projectilePrefab, out _projectileEffects, out _itemWeight);

    }

    public void DecideNextAction()
    {
        //Stop hold bonus
        _characterManager.SetBonusDefence(-_holdDefence);
        _holdDefence = 0;

        StopAllCoroutines();

        if (CheckTargetValid())
        {
            return;
        }

        //Check if in range
        if (!CheckTargetInRange())
        {
            StartCoroutine(WaitToGetInRange());
            return;
        }

        int randomAction = Random.Range(0, 101);

        if (randomAction < 75)
        {
            StartCoroutine(QuickAttack());
        }
        else if (randomAction >= 75 && randomAction < 90)
        {
            StartCoroutine(HoldAttack());
        }
        else if (randomAction >= 90)
        {
            StartCoroutine(BackOff());
        }
    }

    private IEnumerator QuickAttack()
    {
        _animationController.StartHolding(_weaponSpeed);
        yield return new WaitForSeconds(_weaponSpeed);
        Attack();
    }

    private IEnumerator HoldAttack()
    {
        _animationController.StartHolding(_weaponSpeed);

        yield return new WaitForSeconds(_weaponSpeed);

        if (!_isRanged)
        {
            _holdDefence = _weaponDefence;

            _characterManager.SetBonusDefence(_holdDefence);
        }

        float randomWaitTime = Random.Range(0.5f, 3f);
        yield return new WaitForSeconds(randomWaitTime);
        Attack();
    }

    private bool CheckTargetInRange()
    {
        //If not target just return false
        if (_combatTarget == null)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, _combatTarget.transform.position);
        if (distance <= _weaponRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator WaitToGetInRange()
    {
        yield return new WaitForSeconds(1f);
        DecideNextAction();
    }

    private void Attack()
    {
        _animationController.TriggerAttack();

        if (CheckTargetValid())
        {
            return;
        }

        if (!_isRanged)
        {
            CalculateAttack(_combatTarget.gameObject);

            _characterManager.DamageStamina(StatFormulas.AttackStaminaCost(_itemWeight, _weaponSpeed));

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSwingMelee", transform.position);
        }
        else
        {
            ProjectileController projectileController = Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation).GetComponent<ProjectileController>();

            //If the weapon does physical damage add a listener to calculate attack
            if (_weaponDamage > 0 || _weaponBluntDamage > 0)
            {
                projectileController.hitEvent.AddListener(delegate { GetProjectileHitData(projectileController); });
            }

            //Decrease stamina
            _characterManager.DamageStamina(StatFormulas.AttackStaminaCost(_itemWeight, _weaponSpeed));

  
            foreach (Effect effect in _projectileEffects)
            {
                projectileController.effects.Add(effect);
            }

            //Play audio based on if the ranged attack has effects or not
            if (_projectileEffects == null || _projectileEffects.Count == 0)
            {
                AudioManager.instance.PlayOneShot("event:/CombatSwingRanged", transform.position);
            }
            else
            {
                AudioManager.instance.PlayOneShot("event:/CombatSpellCast", transform.position);
            }
        }

        DecideNextAction();
    }

    private IEnumerator BackOff()
    {
        if (CheckTargetValid())
        {
            yield break;
        }

        _movementController.MoveBackwards();
        yield return new WaitForSeconds(0.5f);
        _movementController.SetTargetDistance(_weaponRange);
        _movementController.SetTarget(_combatTarget.transform);
        DecideNextAction();
    }

    public void GetProjectileHitData(ProjectileController projectileController)
    {
        if (CheckTargetValid())
        {
            return;
        }

        if (projectileController.hitCollision.collider.CompareTag("Character") || projectileController.hitCollision.collider.CompareTag("Player"))
        {
            CalculateAttack(projectileController.hitCollision.gameObject);
        }
    }

    public void CalculateAttack(GameObject target)
    {
        if (CheckTargetValid())
        {
            return;
        }

        if (target.CompareTag("Character") || target.CompareTag("Player"))
        {
            //Randomises the damage
            _weaponDamage = StatFormulas.RollDice(_weaponDamage, 1);

            //Get targets stats
            CharacterManager targetCharacterManager = target.GetComponentInParent<CharacterManager>();
            int targetDefence = targetCharacterManager.GetTotalDefence(_isRanged);

            int characterAbilityBonus = _characterManager.abilities.body;
            if (_isRanged)
            {
                characterAbilityBonus = _characterManager.abilities.hands;
            }

            int hitDamage = StatFormulas.CalculateHit(_weaponDamage, _weaponBluntDamage, characterAbilityBonus, targetDefence, _characterManager.hasAdvantage, targetCharacterManager.hasDisadvantage, _characterManager.CheckSkill_Assassinate(), _characterManager.CheckSkill("Lucky Strike"), targetCharacterManager.CheckSkill("Lucky Strike"), _characterManager.CheckSkill_HonourFighter(), _characterManager.CheckSkill_Sharpshooter());

            //Check if the character is already wounded and if yes make all attacks hit
            if (targetCharacterManager.characterState == CharacterState.wounded)
            {
                hitDamage = 1;
            }

            //If stamina is less than the amount the attack requires, make it always miss
            if (_characterManager.staminaCurrent < StatFormulas.AttackStaminaCost(_itemWeight, _weaponSpeed))
            {
                hitDamage = 0;
            }

            if (hitDamage > 0)
            {
                //Juice time
                Instantiate(_bloodSplatterPrefab, targetCharacterManager.transform.position, Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f));
                AudioManager.instance.PlayOneShot("event:/CombatHit", targetCharacterManager.transform.position);

                //Do damage to target
                targetCharacterManager.DamageHealth(StatFormulas.Damage(hitDamage), _characterManager);

                _characterManager.CheckSkill_DisablingShot(targetCharacterManager);
            }
            else if (hitDamage <= 0)
            {
                //Target blocks attack
                targetCharacterManager.TriggerBlock();
                AudioManager.instance.PlayOneShot("event:/CombatBlock", transform.position);
            }
        }
    }


    //Check if the current combat target is still valid otherwise stop current combat
    private bool CheckTargetValid()
    {
        //If the target is null or is in a wounded or dead state, or if the character itself is dead or wounded
        if (_combatTarget == null || _combatTarget.characterState != CharacterState.alive || _characterManager.characterState != CharacterState.alive)
        {
            StopCombat();
            return true;
        }

        return false;
    }

}
