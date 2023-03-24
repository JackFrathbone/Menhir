using System.Collections;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour
{
    //References
    [SerializeField] Transform _projectileSpawn;
    private CharacterAnimationController _animationController;
    private CharacterMovementController _movementController;
    private NonPlayerCharacterManager _NPCCharacterManager;
    private CharacterAI _characterAI;

    [SerializeField] CharacterManager _combatTarget;
    private float _weaponRange;
    private float _weaponSpeed;
    private int _weaponDamage;
    private int _holdDefence;

    private void Awake()
    {
        _animationController = GetComponentInChildren<CharacterAnimationController>();
        _movementController = GetComponent<CharacterMovementController>();
        _NPCCharacterManager = GetComponent<NonPlayerCharacterManager>();
        _characterAI = GetComponent<CharacterAI>();
    }

    public void StartCombat(CharacterManager target)
    {
        _combatTarget = target;
        _combatTarget.StartCombat(_NPCCharacterManager);

        _animationController.UpdateCombatState(true);

        DecideNextAction();
    }

    public void StopCombat()
    {
        _animationController.UpdateCombatState(false);

        _characterAI.RemoveTarget(_combatTarget);
        _combatTarget.StopCombat(_NPCCharacterManager);

        _combatTarget = null;
    }

    public void DecideNextAction()
    {
        //Stop hold bonus
        _NPCCharacterManager.SetBonusDefence(-_holdDefence);
        _holdDefence = 0;

        StopAllCoroutines();

   /*     if (_combatTarget != null && (_NPCCharacterManager.characterState != CharacterState.alive || _combatTarget.characterState != CharacterState.alive))
        {
            print("ee");
            StopCombat();
            return;
        }*/

        if(_combatTarget == null)
        {
            return;
        }

        //Set the distance from the target
        if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem meleeItem)
        {
            _movementController.SetTargetDistance(meleeItem.weaponRange);
            _weaponRange = meleeItem.weaponRange;
            _weaponSpeed = meleeItem.weaponSpeed;
            _weaponDamage = meleeItem.weaponDamage;
        }
        else if (_NPCCharacterManager.equippedWeapon is WeaponRangedItem rangedItem)
        {
            _movementController.SetTargetDistance(15f);
            _weaponRange = 15f;
            _weaponSpeed = rangedItem.weaponSpeed;
            _weaponDamage = rangedItem.weaponDamage;
        }
        else if (_NPCCharacterManager.equippedWeapon is WeaponFocusItem focusItem)
        {
            _movementController.SetTargetDistance(15f);
            _weaponRange = 15f;
            _weaponSpeed = focusItem.castingSpeed;
            _weaponDamage = 0;
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
        _animationController.StartHolding(_weaponSpeed / 2);
        yield return new WaitForSeconds(_weaponSpeed / 2);
        Attack();
    }

    private IEnumerator HoldAttack()
    {
        _animationController.StartHolding(_weaponSpeed);

        yield return new WaitForSeconds(_weaponSpeed);

        if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            _holdDefence = (_NPCCharacterManager.equippedWeapon as WeaponMeleeItem).weaponDefence;

            _NPCCharacterManager.SetBonusDefence(_holdDefence);
        }

        float randomWaitTime = Random.Range(0.5f, 3f);
        yield return new WaitForSeconds(randomWaitTime);
        Attack();
    }

    private bool CheckTargetInRange()
    {
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

        //If melee
        if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            CalculateAttack(_combatTarget.gameObject);

            _NPCCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_NPCCharacterManager.equippedWeapon.itemWeight, _weaponSpeed));

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSwingMelee", transform.position);
        }

        //If ranged
        else if (_NPCCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            ProjectileController projectileController = Instantiate((_NPCCharacterManager.equippedWeapon as WeaponRangedItem).projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation).GetComponent<ProjectileController>();

            projectileController.hitEvent.AddListener(delegate { GetProjectileHitData(projectileController); });

            //Decrease stamina
            _NPCCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_NPCCharacterManager.equippedWeapon.itemWeight, _weaponSpeed));

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSwingRanged", transform.position);
        }
        //If focus
        else if (_NPCCharacterManager.equippedWeapon is WeaponFocusItem)
        {
            ProjectileController projectileController = Instantiate((_NPCCharacterManager.equippedWeapon as WeaponFocusItem).projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation).GetComponent<ProjectileController>();

            foreach (Effect effect in (_NPCCharacterManager.equippedWeapon as WeaponFocusItem).focusEffects)
            {
                projectileController.effects.Add(effect);
            }

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSpellCast", transform.position);
        }

        DecideNextAction();
    }

    private IEnumerator BackOff()
    {
        _movementController.MoveBackwards();
        yield return new WaitForSeconds(0.5f);
        _movementController.SetTargetDistance(_weaponRange);
        _movementController.SetTarget(_combatTarget.transform);
        DecideNextAction();
    }

    public void GetProjectileHitData(ProjectileController projectileController)
    {
        if (projectileController.hitCollision.collider.CompareTag("Character") || projectileController.hitCollision.collider.CompareTag("Player"))
        {
            CalculateAttack(projectileController.hitCollision.gameObject);
        }
    }

    public void CalculateAttack(GameObject target)
    {
        if (target.CompareTag("Character") || target.CompareTag("Player"))
        {
            //Get the various checks
            int weaponDamage = 0;
            int weaponRolls = 0;
            int weaponAbility = 0;
            bool isRangedAttack = false;

            if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem)
            {
                weaponAbility = _NPCCharacterManager.characterSheet.abilities.body;
                weaponDamage = _weaponDamage + _NPCCharacterManager.bonusDamage;
                weaponRolls = (_NPCCharacterManager.equippedWeapon as WeaponMeleeItem).weaponRollAmount;
            }
            else if (_NPCCharacterManager.equippedWeapon is WeaponRangedItem)
            {
                weaponAbility = _NPCCharacterManager.characterSheet.abilities.hands;
                weaponDamage = (_NPCCharacterManager.equippedWeapon as WeaponRangedItem).weaponDamage + _NPCCharacterManager.bonusDamage;
                weaponRolls = (_NPCCharacterManager.equippedWeapon as WeaponRangedItem).weaponRollAmount;
                isRangedAttack = true;
            }

            //Randomises the damage
            weaponDamage = StatFormulas.RollDice(weaponDamage, weaponRolls);

            //Get targets stats
            CharacterManager targetCharacterManager = target.GetComponentInParent<CharacterManager>();
            int targetDefence = targetCharacterManager.GetTotalDefence(isRangedAttack);
            int hitDamage = StatFormulas.CalculateHit(weaponDamage, weaponAbility, targetDefence, _NPCCharacterManager.hasAdvantage, targetCharacterManager.hasDisadvantage, _NPCCharacterManager.CheckSkill_Assassinate(), _NPCCharacterManager.CheckSkill("Lucky Strike"), targetCharacterManager.CheckSkill("Lucky Strike"), _NPCCharacterManager.CheckSkill_HonourFighter(), _NPCCharacterManager.CheckSkill_Sharpshooter());

            //Check if the character is already wounded and if yes make all attacks hit
            if (targetCharacterManager.characterState == CharacterState.wounded)
            {
                hitDamage = 1;
            }

            //If stamina is less than the amount the attack requires, make it always miss
            if (_NPCCharacterManager.staminaCurrent < StatFormulas.AttackStaminaCost(_NPCCharacterManager.equippedWeapon.itemWeight, _weaponSpeed))
            {
                hitDamage = 0;
            }

            if (hitDamage > 0)
            {
                //Do damage to target
                targetCharacterManager.DamageHealth(StatFormulas.Damage(hitDamage));
                AudioManager.instance.PlayOneShot("event:/CombatHit", transform.position);

                _NPCCharacterManager.CheckSkill_DisablingShot(targetCharacterManager);
            }
            else if (hitDamage <= 0)
            {
                //Target blocks attack
                targetCharacterManager.TriggerBlock();
                AudioManager.instance.PlayOneShot("event:/CombatBlock", transform.position);
            }
        }
    }


}
