using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInventoryButton : MonoBehaviour
{
    private Item _item;
    private TextMeshProUGUI _Label;
    private PlayerInventory _playerInventory;

    private Button _button;

    public void SetItem(Item i, PlayerInventory p, bool isSearch, ItemContainer c)
    {
        _playerInventory = p;

        _item = i;

        _Label = GetComponentInChildren<TextMeshProUGUI>();
        _Label.text = _item.itemName;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate { StartInventoryDescription(isSearch, c); });
    }

    public void StartInventoryDescription(bool isSearch, ItemContainer c)
    {
        _playerInventory.GetComponent<PlayerInventoryDescription>().SetDescription(_item, isSearch, c);
    }
}
