using System.Collections;
using UnityEngine;

public class CharacterCombatController : MonoBehaviour
{
    //References
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

        StartHold();
    }

    public void StopCombat()
    {
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
        else
        {
            _movementController.SetTargetDistance(10f);
            _weaponRange = 10f;
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
        //Get the weapon skill type
        int weaponDamage = 0;
        int weaponRolls = 0;
        int weaponAbility = 0;
        int targetDefence = 0;

        if (_NPCCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            weaponDamage = (_NPCCharacterManager.equippedWeapon as WeaponMeleeItem).weaponDamage;
            weaponRolls = (_NPCCharacterManager.equippedWeapon as WeaponMeleeItem).weaponRollAmount;
            weaponAbility = _NPCCharacterManager.characterSheet.abilities.body;

            //Randomises the damage
            weaponDamage = StatFormulas.RollDice(weaponDamage, weaponRolls);

            //Gets target defence
            targetDefence = _combatTarget.GetTotalDefence();

            int hitDamage = StatFormulas.CalculateHit(weaponDamage, weaponAbility, targetDefence);

            //Check if the character is already wounded and if yes make all attacks hit
            if (_combatTarget.characterState == CharacterState.wounded)
            {
                hitDamage = 1;
            }

            if (hitDamage > 0)
            {
                //Do damage to target
                _combatTarget.DamageHealth(StatFormulas.Damage(hitDamage));

            }
            else if (hitDamage <= 0)
            {
                //Target blocks attack
                _combatTarget.TriggerBlock();

            }
        }
        //If ranged
        else
        {

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


}
