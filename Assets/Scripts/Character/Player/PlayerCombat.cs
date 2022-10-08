using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Animator References")]
    [SerializeField] Animator _weaponMeleeAnimator;
    [SerializeField] Animator _weaponRangedAnimator;
    [SerializeField] Animator _shieldAnimator;

    [Header("Visual Effects")]
    [SerializeField] GameObject _bloodSplatterPrefab;

    [Header("Projectil References")]
    [SerializeField] Transform _playerProjectileSpawnPoint;

    private bool _isattacking;

    private PlayerCharacterManager _playerCharacterManager;
    private PlayerActiveUI _playerActiveUI;

    private float _weaponSpeed;

    private void Start()
    {
        _playerCharacterManager = GetComponent<PlayerCharacterManager>();

        _playerActiveUI = GameManager.instance.PlayerUIObject.GetComponent<PlayerActiveUI>();
    }

    public void TriggerAttack()
    {
        if (_playerCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            _weaponSpeed = (_playerCharacterManager.equippedWeapon as WeaponMeleeItem).weaponSpeed;
        }
        else if (_playerCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            _weaponSpeed = (_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponSpeed;
        }
        else if (_playerCharacterManager.equippedWeapon is WeaponFocusItem)
        {
            _weaponSpeed = (_playerCharacterManager.equippedWeapon as WeaponFocusItem).castingSpeed;
        }


        //For melee atacks
        if (!_isattacking && (_playerCharacterManager.equippedWeapon is WeaponMeleeItem))
        {
            _weaponMeleeAnimator.SetTrigger("attackAction");
            _weaponMeleeAnimator.SetBool("isHolding", true);

            _shieldAnimator.SetTrigger("attackAction");
            _shieldAnimator.SetBool("isHolding", true);

            //Speed of hold to attack based on weapon speed, in real seconds
            SetHoldSpeed(_weaponSpeed);

            _isattacking = true;
        }
        //If its ranged
        else if (!_isattacking && (_playerCharacterManager.equippedWeapon is WeaponRangedItem))
        {
            _playerCharacterManager.SetRangedSprite((_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponModelDrawing);

            _weaponRangedAnimator.SetTrigger("attackAction");
            _weaponRangedAnimator.SetBool("isHolding", true);

            SetHoldSpeed(_weaponSpeed);

            _isattacking = true;
        }
        //If its magic focus weapon
        else if (!_isattacking && (_playerCharacterManager.equippedWeapon is WeaponFocusItem))
        {
            _weaponMeleeAnimator.SetTrigger("attackAction");
            _weaponMeleeAnimator.SetBool("isHolding", true);

            _shieldAnimator.SetTrigger("attackAction");
            _shieldAnimator.SetBool("isHolding", true);

            //Speed of hold to attack based on weapon speed, in real seconds
            SetHoldSpeed(_weaponSpeed);

            _isattacking = true;
        }
    }

    //Used differentiate between a quick or held attack
    public void TriggerAttackEnd()
    {
        if (_isattacking && (_playerCharacterManager.equippedWeapon is WeaponMeleeItem))
        {
            _weaponMeleeAnimator.SetBool("isHolding", false);
            _shieldAnimator.SetBool("isHolding", false);
        }
        else if (_isattacking && (_playerCharacterManager.equippedWeapon is WeaponRangedItem))
        {
            _weaponRangedAnimator.SetBool("isHolding", false);
        }
        else if (_isattacking && (_playerCharacterManager.equippedWeapon is WeaponFocusItem))
        {
            _weaponMeleeAnimator.SetBool("isHolding", false);
            _shieldAnimator.SetBool("isHolding", false);
        }
    }

    public void TriggerBlock()
    {
        _weaponMeleeAnimator.SetTrigger("blockAction");
        _shieldAnimator.SetTrigger("blockAction");

        //If attacking and then forced to block, reset attack
        _isattacking = false;
    }

    private void SetHoldSpeed(float f)
    {
        _weaponMeleeAnimator.SetFloat("holdSpeed", 1 / f);
        _weaponRangedAnimator.SetFloat("holdSpeed", 1 / f);
        _shieldAnimator.SetFloat("holdSpeed", 1 / f);
    }

    public void MeleeAttack()
    {
        //If weapon is melee check the attack here, otherwise the ranged projectile deal with damage on its own
        if (_isattacking && (_playerCharacterManager.equippedWeapon is WeaponMeleeItem))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, _weaponSpeed))
            {
                CalculateAttack(hit.transform.gameObject, hit.point);
            }

            //Decrease stamina
            _playerCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_playerCharacterManager.equippedWeapon.itemWeight, _weaponSpeed));
            _isattacking = false;
        }
    }

    public void RangedAttack()
    {
        if (_isattacking && (_playerCharacterManager.equippedWeapon is WeaponRangedItem))
        {
            _playerCharacterManager.SetRangedSprite((_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponModelFired);

            ProjectileController projectileController = Instantiate((_playerCharacterManager.equippedWeapon as WeaponRangedItem).projectilePrefab, _playerProjectileSpawnPoint.position, _playerProjectileSpawnPoint.rotation).GetComponent<ProjectileController>();

            projectileController.hitEvent.AddListener(delegate { GetProjectileHitData(projectileController); });

            //Decrease stamina
            _playerCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_playerCharacterManager.equippedWeapon.itemWeight, _weaponSpeed));
            _isattacking = false;
        }
    }

    public void FocusCastAttack()
    {
        if (_isattacking && (_playerCharacterManager.equippedWeapon is WeaponFocusItem))
        {
            ProjectileController projectileController = Instantiate((_playerCharacterManager.equippedWeapon as WeaponFocusItem).projectilePrefab, _playerProjectileSpawnPoint.position, _playerProjectileSpawnPoint.rotation).GetComponent<ProjectileController>();

            foreach (Effect effect in (_playerCharacterManager.equippedWeapon as WeaponFocusItem).focusEffects)
            {
                projectileController.effects.Add(effect);
            }

            _isattacking = false;
        }

    }

    public void GetProjectileHitData(ProjectileController projectileController)
    {
        if (projectileController.hitCollision.collider.CompareTag("Character"))
        {
            CalculateAttack(projectileController.hitCollision.gameObject, projectileController.hitCollision.transform.position);
        }
    }

    //Check the target, calculate attack and damage if valid, and if blood is needed uses the vector3 hitpoint to spawn blood
    public void CalculateAttack(GameObject target, Vector3 hitPoint)
    {
        if (target.CompareTag("Character"))
        {
            //Get the various checks
            int weaponDamage = 0;
            int weaponRolls = 0;
            int weaponAbility = 0;
            int targetDefence = 0;

            if (_playerCharacterManager.equippedWeapon is WeaponMeleeItem)
            {
                weaponAbility = _playerCharacterManager.characterSheet.abilities.body;
                weaponDamage = (_playerCharacterManager.equippedWeapon as WeaponMeleeItem).weaponDamage + _playerCharacterManager.bonusDamage;
                weaponRolls = (_playerCharacterManager.equippedWeapon as WeaponMeleeItem).weaponRollAmount;
            }
            else if (_playerCharacterManager.equippedWeapon is WeaponRangedItem)
            {
                weaponAbility = _playerCharacterManager.characterSheet.abilities.hands;
                weaponDamage = (_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponDamage + _playerCharacterManager.bonusDamage;
                weaponRolls = (_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponRollAmount;
            }

            //Randomises the damage
            weaponDamage = StatFormulas.RollDice(weaponDamage, weaponRolls);

            //Get targets stats
            CharacterManager targetCharacterManager = target.gameObject.GetComponentInParent<CharacterManager>();
            _playerActiveUI.UpdateTargetStatusUI(targetCharacterManager);
            targetDefence = targetCharacterManager.GetTotalDefence();

            int hitDamage = StatFormulas.CalculateHit(weaponDamage, weaponAbility, targetDefence, _playerCharacterManager.hasAdvantage, targetCharacterManager.hasDisadvantage, _playerCharacterManager.CheckSkill_Assassinate(), _playerCharacterManager.CheckSkill("Lucky Strike"), targetCharacterManager.CheckSkill("Lucky Strike"), _playerCharacterManager.CheckSkill_HonourFighter(), _playerCharacterManager.CheckSkill_Sharpshooter());

            //Check if the character is already wounded and if yes make all attacks hit
            if (targetCharacterManager.characterState == CharacterState.wounded)
            {
                hitDamage = 1;
            }

            //If stamina is less than the amount the attack requires, make it always miss
            if (_playerCharacterManager.staminaCurrent < StatFormulas.AttackStaminaCost(_playerCharacterManager.equippedWeapon.itemWeight, _weaponSpeed))
            {
                hitDamage = 0;
            }

            if (hitDamage > 0)
            {
                //Do damage to target
                Instantiate(_bloodSplatterPrefab, hitPoint, Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f));
                targetCharacterManager.DamageHealth(StatFormulas.Damage(hitDamage));
                _playerCharacterManager.CheckSkill_DisablingShot(targetCharacterManager);
                _playerActiveUI.UpdateTargetStatusUI(targetCharacterManager);
            }
            else if (hitDamage <= 0)
            {
                //Target blocks attack
                targetCharacterManager.TriggerBlock();
            }
        }
    }
}
