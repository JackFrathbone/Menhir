using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("If the player can start an attack")]
    private bool _canAttack = true;

    [Header("Animator References")]
    [SerializeField] Animator _weaponMeleeAnimator;
    [SerializeField] Animator _weaponRangedAnimator;
    [SerializeField] Animator _shieldAnimator;

    [Header("Visual Effects")]
    [SerializeField] GameObject _bloodSplatterPrefab;

    [Header("Projectil References")]
    [SerializeField] Transform _playerProjectileSpawnPoint;

    private PlayerCharacterManager _playerCharacterManager;
    private PlayerActiveUI _playerActiveUI;

    [Header("Equipped Weapon Stats")]
    private int _weaponDamage;
    private int _weaponHitBonus;
    private float _weaponRange;
    private float _weaponSpeed;
    private float _itemWeight;
    private bool _isRanged;

    private GameObject _projectilePrefab;
    private List<Effect> _enchantmentEffects;

    private void Start()
    {
        _playerCharacterManager = GetComponent<PlayerCharacterManager>();

        _playerActiveUI = GameManager.instance.PlayerUIObject.GetComponent<PlayerActiveUI>();
    }
    private void SetWeaponStats()
    {
        _playerCharacterManager.GetCurrentWeaponStats(out _weaponDamage, out _weaponHitBonus, out _weaponRange, out _weaponSpeed, out _isRanged, out _projectilePrefab, out _enchantmentEffects, out _itemWeight);
    }

    public void SetCanAttack(bool canAttack)
    {
        _canAttack = canAttack;
    }

    public void TriggerAttack()
    {
        //If the player doesnt have enough stamina or can't attack
        if ((_playerCharacterManager.staminaCurrent <= _playerCharacterManager.staminaTotal * 0.25) || !_canAttack)
        {
            return;
        }

        //Set the weapon variables to be the current equipped weapon
        SetWeaponStats();

        //For melee atacks
        if (_playerCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            _weaponMeleeAnimator.SetInteger("attackRandom", Random.Range(1,3));
            _weaponMeleeAnimator.SetTrigger("attackAction");
            _weaponMeleeAnimator.SetBool("isHolding", true);

            //Speed of hold to attack based on weapon speed, in real seconds
            SetHoldSpeed(_weaponSpeed);
        }
        //If its ranged
        else if (_playerCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            _playerCharacterManager.SetRangedSprite((_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponModelDrawing);

            _weaponRangedAnimator.SetTrigger("attackAction");
            _weaponRangedAnimator.SetBool("isHolding", true);

            SetHoldSpeed(_weaponSpeed);

            //Play draw audio
            AudioManager.instance.PlayOneShot("event:/CombatDrawRanged", transform.position);
        }
    }

    //Used differentiate between a quick or held attack
    public void TriggerAttackEnd()
    {
        if (_playerCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            _weaponMeleeAnimator.SetBool("isHolding", false);
            SetHoldSpeed(_weaponSpeed);
        }
        else if (_playerCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            _weaponRangedAnimator.SetBool("isHolding", false);
        }
    }

    public void TriggerBlock()
    {
        _weaponMeleeAnimator.SetTrigger("blockAction");
        _shieldAnimator.SetTrigger("blockAction");
    }

    public void TriggerHold()
    {
        _weaponMeleeAnimator.SetBool("isHolding", true);
    }

    public void TriggerHoldEnd()
    {
        _weaponMeleeAnimator.SetBool("isHolding", false);
    }

    private void SetHoldSpeed(float f)
    {
        _weaponMeleeAnimator.SetFloat("holdSpeed", 1 / f);
        _weaponRangedAnimator.SetFloat("holdSpeed", 1 / f);
    }

    public void MeleeAttack()
    {
        TriggerHoldEnd();

        //If weapon is melee check the attack here, otherwise the ranged projectile deal with damage on its own
        if (_playerCharacterManager.equippedWeapon is WeaponMeleeItem)
        {
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, _weaponRange))
            {
                if (hit.collider.CompareTag("Character"))
                {
                    CalculateAttack(hit.transform.gameObject, hit.point);
                }
            }

            //Decrease stamina
            _playerCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_itemWeight, _weaponSpeed));

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSwingMelee", transform.position);
        }
    }

    public void RangedAttack()
    {
        if (_playerCharacterManager.equippedWeapon is WeaponRangedItem)
        {
            _playerCharacterManager.SetRangedSprite((_playerCharacterManager.equippedWeapon as WeaponRangedItem).weaponModelFired);

            ProjectileController projectileController = Instantiate(_projectilePrefab, _playerProjectileSpawnPoint.position, _playerProjectileSpawnPoint.rotation).GetComponent<ProjectileController>();

            projectileController.hitEvent.AddListener(delegate { GetProjectileHitData(projectileController); });

            //Decrease stamina
            _playerCharacterManager.DamageStamina(StatFormulas.AttackStaminaCost(_playerCharacterManager.equippedWeapon.itemWeight, _weaponSpeed));

            //Play audio
            AudioManager.instance.PlayOneShot("event:/CombatSwingRanged", transform.position);
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
        //Get target and get the relevant stats
        CharacterManager targetCharacterManager = target.GetComponentInParent<CharacterManager>();

        int targetDefence = targetCharacterManager.GetTotalDefence();

        int characterAbility = _playerCharacterManager.abilities.body;
        if (_isRanged)
        {
            characterAbility = _playerCharacterManager.abilities.hands;
        }

        //Check if it hits
        if (StatFormulas.RollToHit(characterAbility, _weaponHitBonus, targetDefence, _playerCharacterManager.hasAdvantage, _playerCharacterManager.hasDisadvantage, _playerCharacterManager.CheckSkill("Lucky Strike"), targetCharacterManager.CheckSkill("Lucky Strike"), targetCharacterManager.CheckSkill_HonourFighter(), _playerCharacterManager.CheckSkill_Sharpshooter(), _playerCharacterManager.CheckSkill_Hunter(targetCharacterManager)) || targetCharacterManager.characterState == CharacterState.wounded || targetCharacterManager.characterState == CharacterState.dead)
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

            //Do damage to target
            targetCharacterManager.DamageHealth(StatFormulas.Damage(_weaponDamage, 0f, _playerCharacterManager.CheckSneakAttack()), _playerCharacterManager);

            //Juice time
            Instantiate(_bloodSplatterPrefab, hitPoint, Quaternion.Euler(0f, Camera.main.transform.rotation.eulerAngles.y, 0f));
            AudioManager.instance.PlayOneShot("event:/CombatHit", hitPoint);

            _playerCharacterManager.CheckSkill_DisablingShot(targetCharacterManager);

            //Update the UI
            _playerActiveUI.UpdateTargetStatusUI(targetCharacterManager);
        }
        else
        {
            //Target blocks attack
            targetCharacterManager.TriggerBlock();
            AudioManager.instance.PlayOneShot("event:/CombatBlock", hitPoint);
        }
    }
}
