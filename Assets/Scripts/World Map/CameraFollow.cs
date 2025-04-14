using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] Transform _target;
    [SerializeField] float _smoothSpeed = 0.125f;
    private Vector3 _velocity = Vector3.zero;

    private void Update()
    {
        if (_target == null)
            return;

        Vector3 desiredPosition = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothSpeed);

    }
}