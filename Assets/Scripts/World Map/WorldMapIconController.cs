using Udar.SceneField;
using UnityEngine;
using TMPro;

public class WorldMapIconController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The name that will appear under the icon")]
    [SerializeField] string _iconLabel;

    [Tooltip("The scene attached to this icon that should be loaded on interaction")]
    [SerializeField] SceneField _attachedScene;

    private bool _isHiddenByDefault;

    [Header("Data")]
    private TextMeshPro _locationTextMesh;
    private bool _isHidden;

    private void Start()
    {
        _locationTextMesh = GetComponentInChildren<TextMeshPro>();
        _locationTextMesh.text = _iconLabel;

        if (_isHiddenByDefault)
        {
            _isHidden = true;
            gameObject.SetActive(false);
        }
    }

    public string GetIconLabel()
    {
        return _iconLabel;
    }

    public int GetSceneInt()
    {
        return _attachedScene.BuildIndex;
    }
}
