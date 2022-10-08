using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileController : MonoBehaviour
{
    [Header("Projectile Setting")]
    [SerializeField] float _projectileSpeed;
    [SerializeField] bool _affectedByGravity;

    private Rigidbody _projectileRigidbody;

    //Events to attach to the target
    public List<Effect> effects = new List<Effect>();

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

            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject, 4f);
            _projectileRigidbody.isKinematic = true;
        }
    }
}
