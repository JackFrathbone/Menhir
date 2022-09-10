using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    //Determines if the container will delete when its empty, and also if it will combine with other containers// Basically if its unique or temporary
    public bool deleteEmpty = true;
    public List<Item> inventory = new List<Item>();

    public void SetInventory(List<Item> inv)
    {
        inventory = new List<Item>(inv);
    }

    public void RemoveItem(Item i)
    {
        inventory.Remove(i);

        CheckIfEmpty();
    }

    private void CombineContainers()
    {
        if (deleteEmpty)
        {
    
        }
    }

    private void CheckIfEmpty()
    {
        if (deleteEmpty)
        {
            if(inventory.Count == 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            return;
        }
    }
}
