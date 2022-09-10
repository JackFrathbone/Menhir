using UnityEngine;

public class AutoDestroyScript : MonoBehaviour
{
    [SerializeField] float _destroyTimer;

    private void Start()
    {
        Destroy(gameObject, _destroyTimer);
    }
}
