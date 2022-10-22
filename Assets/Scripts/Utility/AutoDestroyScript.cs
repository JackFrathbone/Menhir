using UnityEngine;

public class AutoDestroyScript : MonoBehaviour
{
    [Tooltip("How many seconds before this object is destroyed")]
    [SerializeField] float _destroyTimer;

    private void Start()
    {
        Destroy(gameObject, _destroyTimer);
    }
}
