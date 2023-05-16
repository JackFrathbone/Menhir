using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementController : MonoBehaviour
{
    [Header("References")]
    //For pointing the projectile target at targets
    [SerializeField] Transform _projectileSpawn;
    //For use when character avoid the player
    [SerializeField] Transform _backwardsTarget;

    private NavMeshAgent _navMeshAgent;
    [SerializeField, ReadOnly] Transform _target;

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

        //Set the char to face the target, for shooting projectiles and spells, moves both the transform and the projectile spawn
        Quaternion targetRotation = Quaternion.LookRotation(_target.transform.position - _projectileSpawn.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 1f * Time.deltaTime);
        _projectileSpawn.rotation = Quaternion.Slerp(_projectileSpawn.rotation, targetRotation, 1f * Time.deltaTime);
    }

    public void MoveToPosition(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
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
        if (_navMeshAgent == null || !_navMeshAgent.isOnNavMesh)
        {
            return;
        }

        _navMeshAgent.isStopped = true;
    }

    public void StartMovement()
    {
        if (_navMeshAgent == null || !_navMeshAgent.isOnNavMesh)
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
