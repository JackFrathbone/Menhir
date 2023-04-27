using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileController : MonoBehaviour
{
    [Header("Projectile Setting")]
    [SerializeField] float _projectileSpeed;
    [SerializeField] bool _affectedByGravity;
    [SerializeField] bool _destroyOnImpact;

    private Rigidbody _projectileRigidbody;

    //Events to attach to the target
    public List<Effect> effects = new();

    //Used to run functions when hit
    [HideInInspector] public UnityEvent hitEvent;
    public Collision hitCollision;

    [Header("Spell Area Settings")]
    [SerializeField] GameObject _spellAreaPrefab;
    [Tooltip("How large of an area in meters the effect should be applied, 0 makes it affect only single targets")]
    public float spellAreaScale = 0;

    private void Start()
    {
        _projectileRigidbody = GetComponent<Rigidbody>();
        _projectileRigidbody.useGravity = _affectedByGravity;
        _projectileRigidbody.AddForce(transform.forward * (_projectileSpeed * 100), ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitCollision = collision;

        if (effects.Count != 0)
        {
            //Play spell hit audio if there are effects attached
            AudioManager.instance.PlayOneShot("event:/CombatSpellHit", transform.position);
        }
        else
        {
            //Play regular hit audio if there are no effects attached
            AudioManager.instance.PlayOneShot("event:/CombatRangedHit", transform.position);
        }

        //If no area effect
        if (spellAreaScale == 0)
        {
            if (collision.collider.CompareTag("Character") || collision.collider.CompareTag("Player"))
            {
                hitEvent.Invoke();

                if (effects.Count != 0)
                {
                    CharacterManager targetCharacter = collision.collider.GetComponent<CharacterManager>();

                    foreach (Effect effect in effects)
                    {
                        effect.AddEffect(targetCharacter);
                    }
                }

                if (_destroyOnImpact)
                {
                    Destroy(gameObject);
                }
                else
                {
                    Destroy(gameObject, 1f);
                }
            }
            else
            {
                if (_destroyOnImpact)
                {
                    Destroy(gameObject);
                }
                else
                {
                    _projectileRigidbody.isKinematic = true;
                    Destroy(gameObject, 4f);
                }
            }
        }
        //Spawn an area effect
        else
        {
            GameObject areaEffect = Instantiate(_spellAreaPrefab, transform.position, Quaternion.identity);
            areaEffect.transform.localScale = new Vector3(spellAreaScale, spellAreaScale, spellAreaScale);

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, spellAreaScale, transform.forward);

            foreach (RaycastHit hit in hits)
            {
                CharacterManager targetCharacter = hit.collider.gameObject.GetComponent<CharacterManager>();

                if (targetCharacter != null)
                {
                    foreach (Effect effect in effects)
                    {
                        effect.AddEffect(targetCharacter);
                    }
                }
            }

            Destroy(areaEffect, 1f);
            Destroy(gameObject);
        }
    }
}
