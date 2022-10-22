using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementController : MonoBehaviour
{
    [Header("References")]
    //For use when character avoid the player
    [SerializeField] Transform _backwardsTarget;

    private NavMeshAgent _navMeshAgent;
    private Transform _target;

    private CharacterAnimationController _characterAnimation;

    private Vector3 _startLocation;

    private float _originalSpeed;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _characterAnimation = GetComponentInChildren<CharacterAnimationController>();

        _startLocation = transform.position;

        _originalSpeed = _navMeshAgent.speed;
    }

    private void Update()
    {
        if (_target == null)
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
        _navMeshAgent.updateRotation = true;
    }

    public void SetTargetDistance(float i)
    {
        _navMeshAgent.stoppingDistance = i;
    }

    public void SlowMovement()
    {
        if (_navMeshAgent == null)
        {
            return;
        }

        if (_navMeshAgent.speed == _originalSpeed)
        {
            _navMeshAgent.speed /= 2;
        }
    }

    public void NormalMovment()
    {
        if (_navMeshAgent == null)
        {
            return;
        }

        _navMeshAgent.speed = _originalSpeed;
    }

    public void StopMovement()
    {
        if (_navMeshAgent == null)
        {
            return;
        }

        _navMeshAgent.isStopped = true;
    }

    public void StartMovement()
    {
        if (_navMeshAgent == null)
        {
            return;
        }

        _navMeshAgent.isStopped = false;
    }

    public void ReturnToStart()
    {
        _target = null;
    }

    public void MoveBackwards()
    {
        if (NavMesh.SamplePosition(_backwardsTarget.position, out _, 1f, NavMesh.AllAreas))
        {
            SetTarget(_backwardsTarget);
            SetTargetDistance(1f);
            _navMeshAgent.updateRotation = false;
        }
    }
}
