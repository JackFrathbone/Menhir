using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerJournalDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _journalEntryPrefab;

    [SerializeField] Transform _journalButtonParent;

    [SerializeField] Button _journalSwitchButton;
    private TextMeshProUGUI _journalSwitchButtonText;

    private PlayerCharacterManager _playerCharacterManager;

    private List<JournalEntry> _journalEntries = new();

    private bool _showActive;

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
        _journalSwitchButtonText = _journalSwitchButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    //Checks if the current tab is completed or active quests
    public void RefreshCurrentJournalEntries()
    {
        _journalSwitchButtonText.text = "Archive";
        _showActive = true;

        RefreshJournalButtonsActive();
    }

    public void RefreshJournalButtonsActive()
    {
        _journalSwitchButtonText.text = "Archive";
        _showActive = true;

        ClearButtons();

        _journalEntries.Clear();
        _journalEntries = new List<JournalEntry>(_playerCharacterManager.journalEntries);

        foreach (JournalEntry entry in _journalEntries)
        {
            if (!entry.isArchived)
            {
                GameObject questButton = Instantiate(_journalEntryPrefab, _journalButtonParent);
                questButton.GetComponentInChildren<TextMeshProUGUI>().text = entry.journalText;

                //Add the functions to the buttons
                Button[] buttons = questButton.GetComponentsInChildren<Button>();
                buttons[0].onClick.AddListener(delegate { ArchiveEntry(entry); });
                buttons[1].onClick.AddListener(delegate { DeleteEntry(entry); });
            }
        }
    }

    public void RefreshJournalButtonsArchive()
    {
        _journalSwitchButtonText.text = "Active";
        _showActive = false;

        ClearButtons();

        _journalEntries.Clear();
        _journalEntries = new List<JournalEntry>(_playerCharacterManager.journalEntries);

        foreach (JournalEntry entry in _journalEntries)
        {
            if (entry.isArchived)
            {
                GameObject questButton = Instantiate(_journalEntryPrefab, _journalButtonParent);
                questButton.GetComponentInChildren<TextMeshProUGUI>().text = entry.journalText;

                //Add the functions to the buttons
                Button[] buttons = questButton.GetComponentsInChildren<Button>();
                buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Mark Active";
                buttons[0].onClick.AddListener(delegate { ArchiveEntry(entry); });
                buttons[1].onClick.AddListener(delegate { DeleteEntry(entry); });
            }
        }
    }

    private void ArchiveEntry(JournalEntry entryToArchive)
    {
        foreach (JournalEntry entry in _playerCharacterManager.journalEntries)
        {
            if (entry == entryToArchive)
            {
                entry.isArchived = !entry.isArchived;
                break;
            }
        }

        RefreshEntries();
    }

    private void DeleteEntry(JournalEntry entryToDelete)
    {
        foreach (JournalEntry entry in _playerCharacterManager.journalEntries)
        {
            if (entry == entryToDelete)
            {
                _playerCharacterManager.journalEntries.Remove(entry);
                break;
            }
        }

        RefreshEntries();
    }

    private void RefreshEntries()
    {
        if (_showActive)
        {
            RefreshJournalButtonsActive();
        }
        else
        {
            RefreshJournalButtonsArchive();
        }
    }

    public void SwitchJournal()
    {
        _showActive = !_showActive;

        RefreshEntries();
    }

    public void CreateNewEntry(TextMeshProUGUI text)
    {
        string newText = text.text;
        if(newText == "" || newText == null)
        {
            return;
        }

        JournalEntry newEntry = new JournalEntry()
        {
            journalText = newText,
            isArchived = false
        };
        _playerCharacterManager.AddJournalEntry(newEntry);

        RefreshEntries();
    }

    private void ClearButtons()
    {
        foreach (Transform child in _journalButtonParent)
        {
            Destroy(child.gameObject);
        }
    }
}
