using UnityEngine;
using UnityEngine.Rendering;

public class DebugFix : MonoBehaviour
{
    private void Awake()
    {
        DebugManager.instance.enableRuntimeUI = false;
    }
}
