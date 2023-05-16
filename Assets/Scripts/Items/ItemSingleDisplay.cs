using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSingleDisplay : MonoBehaviour
{
    //Used for putting a single item out in the world
    [Header("Data")]
    public Item item;

    [Header("References")]
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = item.itemIcon;
    }

    private void OnValidate()
    {
        if (item != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = item.itemIcon;

            name = "WorldItem_" + item.itemName;
        }
    }
}
