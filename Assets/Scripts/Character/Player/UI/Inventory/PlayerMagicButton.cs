using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMagicButton : MonoBehaviour
{
    [Header("References")]
    private Spell _spell;
    private TextMeshProUGUI _Label;
    private PlayerMagic _playerMagic;
    [SerializeField] Image _icon;
    private Button _button;

    public void SetSpell(Spell spell, PlayerMagic playerMagic)
    {
        _playerMagic = playerMagic;

        _spell = spell;

        _Label = GetComponentInChildren<TextMeshProUGUI>();
        _Label.text = _spell.spellName;

        _button = GetComponent<Button>();
        _button.onClick.AddListener(delegate { StartMagicDescription(); });

        _icon.sprite = _spell.spellIcon;
    }

    public void StartMagicDescription()
    {
        _playerMagic.GetComponent<PlayerMagicDescription>().SetDescription(_spell);
    }
}
