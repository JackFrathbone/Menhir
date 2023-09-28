using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SceneDecorationRandomiser : MonoBehaviour
{
    //Used to randomise the scale and rotation of object when they are first placed in a scene
    [Header("Settings")]
    [SerializeField] bool _randomRotateY;

    [SerializeField] bool _randomScale;
    [SerializeField] float _scaleMin;
    [SerializeField] float _scaleMax;


    private void Start()
    {
        if (_randomRotateY)
        {
            // Generate a random rotation angle
            float randomYRotation = Random.Range(0, 360);

            // Apply the rotation to the transform on the Y-axis
            transform.Rotate(0f, randomYRotation, 0f);
        }

        if (_randomScale)
        {
            //Get a random scale from the min and max
            float randomScaleFloat = Random.Range(_scaleMin, _scaleMax);

            // Create a new Vector3 with the random scale values
            Vector3 randomScale = new(randomScaleFloat, randomScaleFloat, randomScaleFloat);

            // Apply the random scale to the object's transform
            transform.localScale = randomScale;
        }

        DestroyImmediate(this);
    }
}
