using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MessageBox : Singleton<MessageBox>
{
    [Header("References")]
    private TextMeshProUGUI _text;
    private Button _button;

    public void Create(string s, bool playerPresent)
    {
        GameManager.instance.PauseGame(playerPresent, "messageBox");

        Instantiate(Resources.Load("Prefabs/MessageBoxCanvas", typeof(GameObject)) as GameObject, this.transform);

        _text = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponentInChildren<Button>();

        _button.onClick.AddListener(delegate { CloseButton(playerPresent); });

        _text.text = s;
    }

    private void CloseButton(bool playerPresent)
    {
        if (GameManager.instance.CheckCanUnpause("messageBox"))
        {
            GameManager.instance.UnPauseGame(playerPresent);
        }
        Destroy(this.gameObject);
    }
}
