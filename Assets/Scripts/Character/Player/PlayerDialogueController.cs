using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerDialogueController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject dialogueUI;
    [SerializeField] TextMeshProUGUI characterNameText;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] GameObject dialogueNextButton;
    [SerializeField] Transform dialogueLeaveButton;
    [SerializeField] GameObject descriptionBox;

    [Header("Prefab")]
    [SerializeField] GameObject dialogueTopicButton;

    private TextMeshProUGUI descriptionText;

    private CharacterManager currentCharacterManager;
    private Dialogue currentDialogue;
    private int currentDialogueLine;
    //List of all current topic buttons to be deleted
    private List<GameObject> topicButtonsToDelete = new List<GameObject>();

    //Links
    private PlayerCharacterManager _playerCharacterManager;

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
        descriptionText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void StartDialogue(CharacterManager characterManager)
    {
        currentCharacterManager = characterManager;

        descriptionBox.SetActive(false);

        dialogueUI.SetActive(true);
        dialogueNextButton.SetActive(false);
        dialogueText.text = currentCharacterManager.characterSheet.characterGreeting;
        characterNameText.text = currentCharacterManager.characterSheet.characterName;

        SpawnTopicButtons(currentCharacterManager.characterSheet.characterDialogueTopics);
    }

    public void LoadNextLine()
    {
        if(currentDialogue != null)
        {
            if(currentDialogueLine >= currentDialogue.dialogueLines.Count -1)
            {
                //Check if dialogue can run once, and then add it to the check list if it is
                if (currentDialogue.dialogueRunOnce)
                {
                    currentCharacterManager.alreadyRunDialogue.Add(currentDialogue);
                }

                //Add state checks to global list
                foreach(StateCheck stateCheck in currentDialogue.dialogueAddStateCheck)
                {
                    _playerCharacterManager.stateChecks.Add(stateCheck);
                }

                //If there is more sub dialogue to follow
                if(currentDialogue.dialogueSubTopics.Count > 0)
                {
                    SpawnTopicButtons(currentDialogue.dialogueSubTopics);
                }
                //If not go back to root
                else
                {
                    SpawnTopicButtons(currentCharacterManager.characterSheet.characterDialogueTopics);
                    dialogueLeaveButton.gameObject.SetActive(true);
                }
  
            }
            else
            {
                currentDialogueLine++;
                dialogueText.text = currentDialogue.dialogueLines[currentDialogueLine];
            }
        }
    }

    public void LoadTopic(Dialogue dialogue)
    {
        if (!AbilityCheck(dialogue))
        {
            return;
        }

        ClearAllTopicButtons();

        currentDialogue = dialogue;
        currentDialogueLine = 0;

        dialogueNextButton.SetActive(true);
        dialogueLeaveButton.gameObject.SetActive(false);
        dialogueText.text = currentDialogue.dialogueLines[currentDialogueLine];
    }

    private void SpawnTopicButtons(List<Dialogue> dialogues)
    {
        dialogueText.text = "";
        dialogueNextButton.SetActive(false);
        ClearAllTopicButtons();

        foreach (Dialogue dialogue in dialogues)
        {
            if(!currentCharacterManager.alreadyRunDialogue.Contains(dialogue) && CompareStateChecks(dialogue) && ItemCheck(dialogue))
            {
                GameObject topicButton = Instantiate(dialogueTopicButton, dialogueText.transform.parent);
                topicButton.GetComponentInChildren<TextMeshProUGUI>().text = SetTopicButtonText(dialogue);
                topicButton.GetComponent<Button>().onClick.AddListener(delegate { LoadTopic(dialogue); });

                topicButtonsToDelete.Add(topicButton);
            }
        }

        dialogueLeaveButton.transform.SetAsLastSibling();
    }

    //Compare dialogue state check requirements vs what is in the global list, returns true is they have it
    private bool CompareStateChecks(Dialogue dialogue)
    {
        //If no checks
        if(dialogue.dialogueStateChecks.Count == 0)
        {
            return true;
        }

        //If there checks
        foreach (StateCheck stateCheck in dialogue.dialogueStateChecks)
        {
            if (_playerCharacterManager.stateChecks.Contains(stateCheck))
            {
                return true;
            }
        }

        return false;
    }

    //Checks required items by items in players inventory
    private bool ItemCheck(Dialogue dialogue)
    {
        List<Item> requiredItems = dialogue.dialogueRequiredItems;

        //If list is empty or null simply pass the check
        if (requiredItems == null || requiredItems.Count == 0)
        {
            return true;
        }

        //Go through each item and if not present set to false
        bool passCheck = true;

        foreach(Item item in requiredItems)
        {
            if (!_playerCharacterManager.currentInventory.Contains(item))
            {
                passCheck = false;
            }
        }

        return passCheck;
    }

    private bool AbilityCheck(Dialogue dialogue)
    {
        //Passes true unless a player doesnt have enough points in any ability
        bool passCheck = true;

        if (dialogue.dialogueAbilityChecks.body != 0 && _playerCharacterManager.characterSheet.abilities.body < dialogue.dialogueAbilityChecks.body)
        {
            passCheck = false;
        }
        else if (dialogue.dialogueAbilityChecks.hands != 0 && _playerCharacterManager.characterSheet.abilities.hands < dialogue.dialogueAbilityChecks.hands)
        {
            passCheck = false;
        }
        else if (dialogue.dialogueAbilityChecks.mind != 0 && _playerCharacterManager.characterSheet.abilities.mind < dialogue.dialogueAbilityChecks.mind)
        {
            passCheck = false;
        }
        else if (dialogue.dialogueAbilityChecks.heart != 0 && _playerCharacterManager.characterSheet.abilities.heart < dialogue.dialogueAbilityChecks.heart)
        {
            passCheck = false;
        }

        return passCheck;
    }

    //Used for displaying Ability checks
    private string SetTopicButtonText(Dialogue dialogue)
    {
        string newTopicText = dialogue.dialogueTopic;

        if (dialogue.dialogueAbilityChecks.body != 0)
        {
            newTopicText += " [Physique " + dialogue.dialogueAbilityChecks.body.ToString() + "]";
        }
        else if (dialogue.dialogueAbilityChecks.hands != 0)
        {
            newTopicText += " [Agility " + dialogue.dialogueAbilityChecks.hands.ToString() + "]";
        }
        else if (dialogue.dialogueAbilityChecks.mind != 0)
        {
            newTopicText += " [Mental " + dialogue.dialogueAbilityChecks.mind.ToString() + "]";
        }
        else if (dialogue.dialogueAbilityChecks.heart != 0)
        {
            newTopicText += " [Social " + dialogue.dialogueAbilityChecks.heart.ToString() + "]";
        }

        return newTopicText;
    }

    private void ClearAllTopicButtons()
    {
        //Delete all the buttons
        for (int i = topicButtonsToDelete.Count - 1; i >= 0; i--)
        {
            Destroy(topicButtonsToDelete[i].gameObject);
        }

        topicButtonsToDelete.Clear();
    }

    public void EndDialogue()
    {
        ClearAllTopicButtons();
        currentCharacterManager = null;
        GameManager.instance.UnPauseGame(true);
        dialogueUI.SetActive(false);
    }

    public void ToggleDescription()
    {
        if (descriptionBox.activeInHierarchy)
        {
            CloseCharacterDescription();
        }
        else
        {
            ShowCharacterDescription();
        }
    }

    private void ShowCharacterDescription()
    {
        descriptionText.text = currentCharacterManager.characterSheet.characterDescription;
        descriptionBox.SetActive(true);
    }

    private void CloseCharacterDescription()
    {
        descriptionText.text = "";
        descriptionBox.SetActive(false);
    }
}
