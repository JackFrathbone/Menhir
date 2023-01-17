using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJournalDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _questButtonPrefab;
    [SerializeField] GameObject _questEntryTextPrefab;

    [SerializeField] Transform _questButtonParent;

    private PlayerCharacterManager _playerCharacterManager;

    private List<Quest> _questsToDisplay = new();

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
    }

    public void RefreshQuestButtonsActive()
    {
        ClearButtons();

        _questsToDisplay.Clear();
        _questsToDisplay = _playerCharacterManager.GetQuestsActive();

        foreach(Quest quest in _questsToDisplay)
        {
            GameObject questButton = Instantiate(_questButtonPrefab, _questButtonParent);
        }
    }

    public void RefreshQuestButtonsCompleted()
    {
        ClearButtons();

        _questsToDisplay.Clear();
        _questsToDisplay = _playerCharacterManager.GetQuestsCompleted();

        foreach (Quest quest in _questsToDisplay)
        {
            GameObject questButton = Instantiate(_questButtonPrefab, _questButtonParent);
        }
    }

    private void ClearButtons()
    {
        foreach(GameObject child in _questButtonParent)
        {
            Destroy(child, 0.1f);
        }
    }
}
