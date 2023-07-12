using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    [Header("Things To Run")]
    [SerializeField, TextArea(1, 6)] string _messageBoxText;
    [SerializeField] List<StateCheck> _stateChecksToAdd = new();
    [SerializeField] JournalEntry _journalEntryToAdd;

    [Header("Tracking")]
    [ReadOnly] public string uniqueID;
    [ReadOnly] public bool triggered = false;

    public string GetUniqueID()
    {
        return uniqueID;
    }

#if UNITY_EDITOR
    [InspectorButton("GenerateID")]
    public bool generateID;

    public void GenerateID()
    {
        if(uniqueID == "" || uniqueID == null)
        {
            uniqueID = name + GetInstanceID().ToString();
            UnityEditor.EditorUtility.SetDirty(this);
        }
        else
        {
            Debug.Log("Clear ID before generating a new one");
        }
    }

    [InspectorButton("ClearID")]
    public bool clearID;

    public void ClearID()
    {
        uniqueID = "";
    }
#endif

    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            if (_messageBoxText != null && _messageBoxText != "")
            {
                MessageBox.instance.Create(_messageBoxText, true);
            }

            if (_stateChecksToAdd != null && _stateChecksToAdd.Count != 0)
            {
                PlayerCharacterManager player = other.GetComponent<PlayerCharacterManager>();

                foreach (StateCheck stateCheck in _stateChecksToAdd)
                {
                    player.stateChecks.Add(stateCheck);
                }
            }

            if(_journalEntryToAdd.journalText != "")
            {
                PlayerCharacterManager player = other.GetComponent<PlayerCharacterManager>();

                player.AddJournalEntry(_journalEntryToAdd);
            }

            triggered = true;
        }
    }
}
