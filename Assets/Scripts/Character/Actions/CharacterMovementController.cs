using UnityEngine;
using UnityEngine.AI;

public class CharacterMovementController : MonoBehaviour
{
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
    
    public void SlowMovement()
    {
        if(_navMeshAgent.speed == _originalSpeed)
        {
            _navMeshAgent.speed = _navMeshAgent.speed / 2;
        }
    }

    public void NormalMovment()
    {
        _navMeshAgent.speed = _originalSpeed;
    }

    public void StopMovement()
    {
        _navMeshAgent.isStopped = true;
    }

    public void StartMovement()
    {
        _navMeshAgent.isStopped = false;
    }

    public void ReturnToStart()
    {
        _target = null;
    }
}
