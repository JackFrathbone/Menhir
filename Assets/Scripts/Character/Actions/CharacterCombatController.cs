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

        StartHold();
    }

    public void StopCombat()
    {
        _combatTarget.StopCombat(_NPCCharacterManager);
        _combatTarget = null;

    }

    private void StartHold()
    {
        _animationController.StartHolding();

        //Set the distance from the target
        if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            WeaponMeleeItem meleeItem = (WeaponMeleeItem)_NPCCharacterManager.equippedWeapon;
            _movementController.SetTargetDistance(meleeItem.weaponRange);
            _weaponRange = meleeItem.weaponRange;
            _weaponSpeed = meleeItem.weaponSpeed;
            _weaponDamage = meleeItem.weaponDamage;
        }
        else if (_NPCCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            WeaponRangedItem rangedItem = (WeaponRangedItem)_NPCCharacterManager.equippedWeapon;
            _movementController.SetTargetDistance(15f);
            _weaponRange = 15f;
            _weaponSpeed = rangedItem.weaponSpeed;
            _weaponDamage = rangedItem.weaponDamage;
        }
        else if (_NPCCharacterManager.equippedWeapon is WeaponFocusItem)
        {
            WeaponFocusItem focusItem = (WeaponFocusItem)_NPCCharacterManager.equippedWeapon;
            _movementController.SetTargetDistance(15f);
            _weaponRange = 15f;
            _weaponSpeed = focusItem.castingSpeed;
            _weaponDamage = 0;
        }

        DecideNextAction();
    }

    private void DecideNextAction()
    {
        StopAllCoroutines();

        if (_combatTarget == null)
        {
            return;
        }

        if (_NPCCharacterManager.characterState != CharacterState.alive)
        {
            StopCombat();
            return;
        }

        if (_combatTarget.characterState != CharacterState.alive)
        {
            _characterAI.RemoveTarget(_combatTarget);
            //_combatTarget = null;
            //StopHold();
            return;
        }

        //Check if in range
        if (!CheckTargetInRange())
        {
            StartCoroutine(WaitToGetInRange());
            return;
        }

        int randomAction = Random.Range(0, 2);
        float randomTime = Random.Range(_weaponSpeed, 3f);

        switch (randomAction)
        {
            case 0:
                StartCoroutine(BackOff());
                break;
            case 1:
                StartCoroutine(Attack(randomTime));
                break;
        }
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

    private IEnumerator Attack(float waitTime)
    {
        _animationController.TriggerAttack();

        if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            CalculateAttack(_combatTarget.gameObject);
        }

        //If ranged
        else if (_NPCCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            ProjectileController projectileController = Instantiate((_NPCCharacterManager.equippedWeapon as WeaponRangedItem).projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation).GetComponent<ProjectileController>();

            projectileController.hitEvent.AddListener(delegate { GetProjectileHitData(projectileController); });

            //Decrease stamina
            _NPCCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_NPCCharacterManager.equippedWeapon.itemWeight, _weaponSpeed));
        }
        //If focus
        else if (_NPCCharacterManager.equippedWeapon is WeaponFocusItem)
        {
            ProjectileController projectileController = Instantiate((_NPCCharacterManager.equippedWeapon as WeaponFocusItem).projectilePrefab, _projectileSpawn.position, _projectileSpawn.rotation).GetComponent<ProjectileController>();

            foreach (Effect effect in (_NPCCharacterManager.equippedWeapon as WeaponFocusItem).focusEffects)
            {
                projectileController.effects.Add(effect);
            }
        }

        yield return new WaitForSeconds(waitTime);
        DecideNextAction();
    }

    private IEnumerator BackOff()
    {
        _movementController.SetTargetDistance(5f);
        yield return new WaitForSeconds(1f);
        _movementController.SetTargetDistance(_weaponRange);
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
            int targetDefence = 0;

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
            }

            //Randomises the damage
            weaponDamage = StatFormulas.RollDice(weaponDamage, weaponRolls);

            //Get targets stats
            CharacterManager targetCharacterManager = target.gameObject.GetComponentInParent<CharacterManager>();
            targetDefence = targetCharacterManager.GetTotalDefence();

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

                _NPCCharacterManager.CheckSkill_DisablingShot(targetCharacterManager);
            }
            else if (hitDamage <= 0)
            {
                //Target blocks attack
                targetCharacterManager.TriggerBlock();
            }
        }
    }


}
