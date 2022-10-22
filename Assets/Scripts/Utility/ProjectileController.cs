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

    private void Start()
    {
        _projectileRigidbody = GetComponent<Rigidbody>();
        _projectileRigidbody.useGravity = _affectedByGravity;
        _projectileRigidbody.AddForce(transform.forward * (_projectileSpeed * 100), ForceMode.Force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        hitCollision = collision;

        if (collision.collider.CompareTag("Character") || collision.collider.CompareTag("Player"))
        {
            hitEvent.Invoke();

            if(effects.Count != 0)
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
}
