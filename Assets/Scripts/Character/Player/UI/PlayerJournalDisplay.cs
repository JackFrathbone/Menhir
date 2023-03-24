using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerJournalDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _questButtonPrefab;
    [SerializeField] GameObject _questEntryTextPrefab;

    [SerializeField] Transform _questButtonParent;

    private PlayerCharacterManager _playerCharacterManager;

    private List<Quest> _questsToDisplay = new();

    private bool _onActive = true;

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
    }

    //Checks if the current tab is completed or active quests
    public void RefreshCurrentQuests()
    {
        if (_onActive)
        {
            RefreshQuestButtonsActive();
        }
        else
        {
            RefreshQuestButtonsCompleted();
        }
    }

    public void RefreshQuestButtonsActive()
    {
        ClearButtons();

        _questsToDisplay.Clear();
        _questsToDisplay = _playerCharacterManager.GetQuestsActive();

        foreach(Quest quest in _questsToDisplay)
        {
            GameObject questButton = Instantiate(_questButtonPrefab, _questButtonParent);
            questButton.GetComponentInChildren<TextMeshProUGUI>().text = quest.questName;

            Transform previousEntryPosition = null;

            foreach(QuestEntry questEntry in quest.questEntries)
            {
                if (questEntry.questEntryActive)
                {
                    GameObject questEntryDisplay = Instantiate(_questEntryTextPrefab, _questButtonParent);

                    if(previousEntryPosition == null)
                    {
                        previousEntryPosition = questButton.transform;
                    }

                    questEntryDisplay.transform.SetSiblingIndex(previousEntryPosition.GetSiblingIndex() + 1);
                    questEntryDisplay.GetComponentInChildren<TextMeshProUGUI>().text = questEntry.questEntryStage + ". " + questEntry.questEntryJournalText;

                    previousEntryPosition = questEntryDisplay.transform;
                }
            }
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
            questButton.GetComponentInChildren<TextMeshProUGUI>().text = quest.questName;
        }
    }

    public void ToggleActiveQuests(bool b)
    {
        _onActive = b;
    }

    private void ClearButtons()
    {
        foreach (Transform child in _questButtonParent)
        {
            Destroy(child.gameObject);
        }
    }
}
