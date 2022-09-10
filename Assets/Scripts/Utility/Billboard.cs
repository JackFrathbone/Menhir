using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    private void Awake()
    {
        GetCamera();
    }

    private void GetCamera()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam != null)
        {
            transform.rotation = Quaternion.Euler(0f, cam.transform.rotation.eulerAngles.y, 0f);
        }
        else
        {
            GetCamera();
        }

    }
}
