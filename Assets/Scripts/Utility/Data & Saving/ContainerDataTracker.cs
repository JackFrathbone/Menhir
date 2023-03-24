using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContainerDataTracker
{
    [Header("Data")]
    public string containerName;

    public bool deleteEmpty;

    public List<Item> inventory = new();

    public Vector3 containerPosition;
}
