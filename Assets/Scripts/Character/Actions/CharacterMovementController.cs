using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementController : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private Transform _target;

    private CharacterAnimationController _characterAnimation;

    private Vector3 _startLocation;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _characterAnimation = GetComponentInChildren<CharacterAnimationController>();

        _startLocation = transform.position;
    }

    private void Update()
    {
        if(_target == null)
        {
            _navMeshAgent.SetDestination(_startLocation);
            return;
        }

        _navMeshAgent.SetDestination(_target.position);

        if (_navMeshAgent.velocity.sqrMagnitude > 0)
        {
            _characterAnimation.CharacterWalkingTrue();
        }
        else
        {
            _characterAnimation.CharacterWalkingFalse();
        }
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    public void SetTargetDistance(float i)
    {
        _navMeshAgent.stoppingDistance = i;
    }

    public void ReturnToStart()
    {
        _target = null;
    }
}
