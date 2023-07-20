using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that holds a list of items, one of which is randomly choosen that should be added to the character the first time they are loaded
/// </summary>
[CreateAssetMenu(menuName = "Items/New random item")]
public class RandomItem: ScriptableObject
{
    [Header("Settings")]
    [Tooltip("List of items to randomly choose one from")]
    public List<Item> items;

    public Item GetItem()
    {
        Item i = null;

        i = items[Random.Range(0, items.Count)];

        return i;
    }
}
