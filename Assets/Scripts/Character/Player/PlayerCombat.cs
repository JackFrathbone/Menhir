using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Animator References")]
    [SerializeField] Animator _weaponAnimator;
    [SerializeField] Animator _shieldAnimator;

    [Header("Visual Effects")]
    [SerializeField] GameObject _bloodSplatterPrefab;

    private bool _isattacking;

    private PlayerCharacterManager _playerCharacterManager;
    private PlayerActiveUI _playerActiveUI;

    private void Start()
    {
        _playerCharacterManager = GetComponent<PlayerCharacterManager>();

        _playerActiveUI = GameManager.instance.PlayerUIObject.GetComponent<PlayerActiveUI>();
    }

    public void TriggerAttack()
    {
        //For melee atacks
        if (!_isattacking && (_playerCharacterManager.equippedWeapon is WeaponMeleeItem))
        {
            _weaponAnimator.SetTrigger("attackAction");
            _weaponAnimator.SetBool("isHolding", true);

            _shieldAnimator.SetTrigger("attackAction");
            _shieldAnimator.SetBool("isHolding", true);

            //Speed of hold to attack based on weapon speed, in real seconds
            SetHoldSpeed(_playerCharacterManager.weaponSpeed);

            _isattacking = true;
        }
        //If its ranged
        else if (!_isattacking && (_playerCharacterManager.equippedWeapon is WeaponRangedItem))
        {

        }
    }

    //Used differentiate between a quick or held attack
    public void TriggerAttackEnd()
    {
        if (_isattacking)
        {
            _weaponAnimator.SetBool("isHolding", false);
            _shieldAnimator.SetBool("isHolding", false);
        }
    }

    public void TriggerBlock()
    {
        _weaponAnimator.SetTrigger("blockAction");
        _shieldAnimator.SetTrigger("blockAction");

        //If attacking and then forced to block, reset attack
        _isattacking = false;
    }

    private void SetHoldSpeed(float f)
    {
        _weaponAnimator.SetFloat("holdSpeed", 1 / f);
        _shieldAnimator.SetFloat("holdSpeed", 1 / f);
    }

    public void Attack()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, _playerCharacterManager.weaponRange))
        {
            if (hit.collider.gameObject.tag == "Character")
            {
                //Get the various checks
                int weaponDamage = 0;
                int weaponRolls = 0;
                int weaponAbility = 0;
                int targetDefence = 0;
                if (_playerCharacterManager.equippedWeapon is WeaponMeleeItem)
                {
                    weaponAbility = _playerCharacterManager.characterSheet.abilities.body;
                    weaponDamage = (_playerCharacterManager.equippedWeapon as WeaponMeleeItem).weaponDamage;
                    weaponRolls = (_playerCharacterManager.equippedWeapon as WeaponMeleeItem).weaponRollAmount;
                }
                else if (_playerCharacterManager.equippedWeapon is WeaponRangedItem)
                {
                    weaponAbility = _playerCharacterManager.characterSheet.abilities.hands;
                    weaponDamage = (_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponDamage;
                    weaponRolls = (_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponRollAmount;
                }

                //Randomises the damage
                weaponDamage = StatFormulas.RollDice(weaponDamage, weaponRolls);

                //Get targets stats
                CharacterManager targetCharacterManager = hit.collider.gameObject.GetComponentInParent<CharacterManager>();
                _playerActiveUI.UpdateTargetStatusUI(targetCharacterManager);
                targetDefence = targetCharacterManager.GetTotalDefence();

                int hitDamage = StatFormulas.CalculateHit(weaponDamage, weaponAbility, targetDefence);

                //Check if the character is already wounded and if yes make all attacks hit
                if (targetCharacterManager.characterState == CharacterState.wounded)
                {
                    hitDamage = 1;
                }

                //If stamina is less than the amount the attack requires, make it always miss
                if(_playerCharacterManager.staminaCurrent < StatFormulas.AttackStaminaCost(_playerCharacterManager.equippedWeapon.itemWeight, _playerCharacterManager.weaponRange))
                {
                    hitDamage = 0;
                }

                if (hitDamage > 0)
                {
                    //Do damage to target
                    Instantiate(_bloodSplatterPrefab, hit.point, Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f));
                    targetCharacterManager.DamageHealth(StatFormulas.Damage(hitDamage));
                    _playerActiveUI.UpdateTargetStatusUI(targetCharacterManager);
                }
                else if (hitDamage <= 0)
                {
                    //Target blocks attack
                    targetCharacterManager.TriggerBlock();
                }
            }
        }

        //Decrease stamina
        _playerCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_playerCharacterManager.equippedWeapon.itemWeight, _playerCharacterManager.weaponRange));
        _isattacking = false;
    }
}
