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

    [ReadOnly] public CharacterManager _currentTarget;
    [ReadOnly] public List<CharacterManager> _combatTargets = new();

    [Header("Data")]
    public bool _inCombat;

    [Header("Equipped Weapon Stats")]
    private int _weaponDamage;
    private int _weaponHitBonus;
    private float _weaponRange;
    private float _weaponSpeed;
    private float _itemWeight;
    private bool _isRanged;

    private GameObject _projectilePrefab;
    private List<Effect> _projectileEffects = new();
    private List<Effect> _enchantmentEffects = new();

    private void Awake()
    {
        _animationController = GetComponentInChildren<CharacterAnimationController>();
        _movementController = GetComponent<CharacterMovementController>();
        _characterManager = GetComponent<CharacterManager>();
    }

    private void Update()
    {
        //Dont do anything if not in combat
        if (!_inCombat || _combatTargets.Count == 0)
        {
            return;
        }

        //Check if current target is invalid, get new target, if no valid then end combat
        if (!CheckTargetValid(_combatTargets[0]))
        {
            //Remove the previous target from list
            _combatTargets.Remove(_combatTargets[0]);

            GetNewTarget();
        }

    }

    public void AddCombatTarget(CharacterManager targetCharacter)
    {
        //if valid and not already in list, add character to target list
        if (targetCharacter != null && !_combatTargets.Contains(targetCharacter))
        {
            _combatTargets.Add(targetCharacter);
            CheckIfCombat();
        }
    }

    private void CheckIfCombat()
    {
        //Check if the character is already in combat, to turn combat mode on or off
        if (_combatTargets.Count != 0)
        {
            if (!_inCombat)
            {
                StartCombat();
            }
        }
        else
        {
            StopCombat();
        }
    }

    public void StartCombat()
    {
        SetWeaponStats();

        _inCombat = true;

        _movementController.SetTargetDistance(_weaponRange);

        //Update the visuals
        _animationController.UpdateCombatState(true);

        GetNewTarget();
    }

    public void StopCombat()
    {
        StopAllCoroutines();

        _inCombat = false;

        _combatTargets.Clear();
        _currentTarget = null;

        //Update the animation state
        _animationController.UpdateCombatState(false);

        _animationController.StopHolding();
        _movementController.ReturnToStart();
    }


    //Check if the current combat target is still valid
    private bool CheckTargetValid(CharacterManager targetCharacter)
    {
        //If the target is null or is in a wounded or dead state, or if the character itself is dead or wounded
        if (targetCharacter == null || targetCharacter.characterState != CharacterState.alive)
        {
            //Remove combat target from the characters list
            _characterManager.StopCombat(_currentTarget);
            return false;
        }

        return true;
    }

    private void GetNewTarget()
    {
        if (_currentTarget != null)
        {
            //Set character manager to stop combat for skills purpose
            _characterManager.StopCombat(_currentTarget);
            _currentTarget.StopCombat(_characterManager);
        }

        //Go through list and get next valid target
        CharacterManager newTarget = null;
        foreach (CharacterManager target in _combatTargets)
        {
            if (CheckTargetValid(target))
            {
                newTarget = target;
                break;
            }
        }

        if (newTarget == null)
        {
            StopCombat();
            return;
        }

        //Set current target and movement target
        _currentTarget = newTarget;
        _movementController.SetTarget(_currentTarget.transform);

        //Set character manager to combat for skills purpose
        _characterManager.StartCombat(_currentTarget);
        _currentTarget.StartCombat(_characterManager);

        //Decide next action to take
        DecideNextAction();
    }

    private void SetWeaponStats()
    {
        _characterManager.GetCurrentWeaponStats(out _weaponDamage, out _weaponHitBonus, out _weaponRange, out _weaponSpeed, out _isRanged, out _projectilePrefab, out _projectileEffects, out _enchantmentEffects, out _itemWeight);
    }

    public void DecideNextAction()
    {
        StopAllCoroutines();

        if (!CheckTargetValid(_currentTarget))
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

        if (randomAction < 65)
        {
            StartCoroutine(WindupAttack());
        }
        else
        {
            StartCoroutine(BackOff());
        }
    }

    private IEnumerator WindupAttack()
    {
        _animationController.StartHolding(_weaponSpeed);

        yield return new WaitForSeconds(_weaponSpeed);

        //Update visual and wait for hit
        _animationController.TriggerAttack();

        yield return new WaitForSeconds(0.15f);

        //Start attack calc
        Attack();
    }

    private IEnumerator BackOff()
    {
        if (!CheckTargetValid(_currentTarget))
        {
            yield break;
        }

        _movementController.MoveBackwards();
        yield return new WaitForSeconds(0.5f);
        _movementController.SetTargetDistance(_weaponRange);
        if (!CheckTargetValid(_currentTarget))
        {
            DecideNextAction();
            yield break;
        }
        _movementController.SetTarget(_currentTarget.transform);
        DecideNextAction();
    }

    private bool CheckTargetInRange()
    {
        if (!CheckTargetValid(_currentTarget))
        {
            return false;
        }

        //Get the distance
        float distance = Vector3.Distance(transform.position, _currentTarget.transform.position);

        //Check if in weapon range or not
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
        /*        //Skip if stamina is too low
                if (_characterManager.staminaCurrent <= _characterManager.staminaTotal * 0.25)
                {
                    print("out of stamina");
                    DecideNextAction();
                    return;
                }*/

        if (!CheckTargetValid(_currentTarget))
        {
            return;
        }

        //If melee
        if (!_isRanged)
        {
            //Start the to hit process
            CalculateAttack(_currentTarget);

            //Remove stamina
            _characterManager.DamageStamina(StatFormulas.AttackStaminaCost(_itemWeight, _weaponSpeed));

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSwingMelee", transform.position);
        }
        else
        {
            if (_projectilePrefab != null)
            {
                //Create the projectile
                ProjectileController projectileController = Instantiate(_projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation).GetComponent<ProjectileController>();

                //If the weapon does physical damage add a listener to calculate attack
                if (_weaponDamage > 0)
                {
                    projectileController.hitEvent.AddListener(delegate { GetProjectileHitData(projectileController); });
                }

                //Decrease stamina
                _characterManager.DamageStamina(StatFormulas.AttackStaminaCost(_itemWeight, _weaponSpeed));

                //Check if projectile has effects
                if (_projectileEffects != null)
                {
                    //Get the effects and attack to projectile
                    foreach (Effect effect in _projectileEffects)
                    {
                        projectileController.effects.Add(effect);
                    }
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
        }

        DecideNextAction();
    }



    public void GetProjectileHitData(ProjectileController projectileController)
    {
        if (!CheckTargetValid(_currentTarget))
        {
            return;
        }

        if (projectileController.hitCollision.collider.CompareTag("Character") || projectileController.hitCollision.collider.CompareTag("Player"))
        {
            CharacterManager target = projectileController.hitCollision.collider.GetComponent<CharacterManager>();
            CalculateAttack(target);
        }
    }

    public void CalculateAttack(CharacterManager target)
    {
        //Get target and get the relevant stats
        CharacterManager targetCharacterManager = target.GetComponentInParent<CharacterManager>();
        int targetDefence = targetCharacterManager.GetTotalDefence();

        int characterAbility = _characterManager.abilities.body;
        if (_isRanged)
        {
            characterAbility = _characterManager.abilities.hands;
        }

        //Check if it hits
        if (StatFormulas.RollToHit(characterAbility, _weaponHitBonus, targetDefence, _characterManager.hasAdvantage, _characterManager.hasDisadvantage, _characterManager.CheckSkill("Lucky Strike"), targetCharacterManager.CheckSkill("Lucky Strike"), _currentTarget.CheckSkill_HonourFighter(), _characterManager.CheckSkill_Sharpshooter(), _characterManager.CheckSkill_Hunter(targetCharacterManager)))
        {
            if (_enchantmentEffects != null)
            {
                //Add weapon enchantment effects to the target
                foreach (Effect effect in _enchantmentEffects)
                {
                    //if the effect isnt already applied
                    if (!targetCharacterManager.currentEffects.Contains(effect))
                    {
                        targetCharacterManager.AddEffect(effect);
                    }
                }
            }

            //Do damage to target
            targetCharacterManager.DamageHealth(StatFormulas.Damage(_weaponDamage, _characterManager.CheckSneakAttack()), _characterManager);

            //Juice time
            Instantiate(_bloodSplatterPrefab, targetCharacterManager.transform.position, Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f));
            AudioManager.instance.PlayOneShot("event:/CombatHit", targetCharacterManager.transform.position);

            _characterManager.CheckSkill_DisablingShot(targetCharacterManager);
        }
        else
        {
            //Target blocks attack
            targetCharacterManager.TriggerBlock();
            AudioManager.instance.PlayOneShot("event:/CombatBlock", transform.position);
        }
    }

    private void OnDisable()
    {
        StopCombat();
    }
}
