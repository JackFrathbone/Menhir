using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using XNode;

public class PlayerDialogueController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] GameObject _activeUi;
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

    //List of all current topic buttons to be deleted
    readonly private List<GameObject> topicButtonsToDelete = new();

    //Links
    private PlayerCharacterManager _playerCharacterManager;

    //New dialogue graph system stuff
    private DialogueGraph _currentDialogueGraph;
    private DialogueSentencesNode _currentSentencesNode;

    private int _currentSentence;

    private string _currentGreeting;

    private void Start()
    {
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
        descriptionText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();
    }

    //When a character does has a dialogue graph/component
    public void StartDialogue(DialogueComponent dialogueComponent)
    {
        //Disable the active UI
        _activeUi.SetActive(false);

        //Gets the target data needed
        _currentDialogueGraph = dialogueComponent.AttachedDialogueGraph;
        currentCharacterManager = dialogueComponent.DialogueCharacterManager;
        _currentGreeting = dialogueComponent.CharacterGreeting;

        //Set the UI states
        descriptionBox.SetActive(false);
        dialogueUI.SetActive(true);

        dialogueNextButton.SetActive(true);
        dialogueLeaveButton.gameObject.SetActive(false);

        characterNameText.text = dialogueComponent.CharacterName;
        dialogueText.text = ReplaceText(_currentGreeting);

        dialogueNextButton.GetComponent<Button>().onClick.AddListener(delegate { LoadBaseTopics(); });
    }

    private void LoadBaseTopics()
    {
        Button nextButton = dialogueNextButton.GetComponent<Button>();
        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(delegate { LoadNextLine(); });

        dialogueNextButton.SetActive(false);
        dialogueLeaveButton.gameObject.SetActive(true);

        ReturnToEntryNode();

        SpawnTopicButtons();
    }

    public void LoadNextLine()
    {
        if (_currentSentencesNode != null)
        {
            //If there is no next sentence to load
            if (_currentSentence >= _currentSentencesNode.sentences.Count - 1)
            {
                //Move to the next node
                LoadNextNode();
            }
            else
            {
                _currentSentence++;
                dialogueText.text = ReplaceText(_currentSentencesNode.sentences[_currentSentence]);
            }
        }
    }

    public void LoadTopicButton(DialogueTopicsNode.Topic topic)
    {
        //Go the next sentences node
        if (!AbilityCheck(topic))
        {
            MessageBox.instance.Create("You can't convince them", true);
            return;
        }

        //Check if dialogue can run once, and then set it true
        if (topic.topicRunOnce)
        {
            _playerCharacterManager.alreadyRunDialogueTopics.Add(topic.uniqueID);
        }

        ClearAllTopicButtons();
        LoadNextNodeViaTopic(topic);
    }

    private void SpawnTopicButtons()
    {
        dialogueText.text = "";
        dialogueNextButton.SetActive(false);
        ClearAllTopicButtons();

        foreach (DialogueTopicsNode.Topic topic in (_currentDialogueGraph.current as DialogueTopicsNode).topics)
        {
            if (!_playerCharacterManager.alreadyRunDialogueTopics.Contains(topic.uniqueID) && CompareStateChecks(topic) && ItemCheck(topic))
            {
                GameObject topicButton = Instantiate(dialogueTopicButton, dialogueText.transform.parent);
                topicButton.GetComponentInChildren<TextMeshProUGUI>().text = SetTopicButtonText(topic);
                topicButton.GetComponent<Button>().onClick.AddListener(delegate { LoadTopicButton(topic); });

                topicButtonsToDelete.Add(topicButton);
            }
        }

        dialogueLeaveButton.transform.SetAsLastSibling();
    }

    //Compare dialogue state check requirements vs what is in the global list, returns true is they have it
    private bool CompareStateChecks(DialogueTopicsNode.Topic topic)
    {
        //If no checks
        if (topic.topicStateChecks.Count == 0 && topic.topicStateChecksInactive.Count == 0)
        {
            return true;
        }

        bool passCheck = true;

        //If there checks
        foreach (StateCheck stateCheck in topic.topicStateChecks)
        {
            if (!_playerCharacterManager.stateChecks.Contains(stateCheck))
            {
                passCheck = false;
            }
        }

        //If there are inactive checks, check if the player doesn't have
        foreach (StateCheck stateCheck in topic.topicStateChecksInactive)
        {
            if (_playerCharacterManager.stateChecks.Contains(stateCheck))
            {
                passCheck = false; ;
            }
        }

        return passCheck;
    }

    //Checks required items by items in players inventory
    private bool ItemCheck(DialogueTopicsNode.Topic topic)
    {
        List<Item> requiredItems = topic.topicRequiredItems;

        //If list is empty or null simply pass the check
        if (requiredItems == null || requiredItems.Count == 0)
        {
            return true;
        }

        //Go through each item and if not present set to false
        bool passCheck = true;

        foreach (Item item in requiredItems)
        {
            if (!_playerCharacterManager.currentInventory.Contains(item))
            {
                passCheck = false;
            }
        }

        return passCheck;
    }

    private bool AbilityCheck(DialogueTopicsNode.Topic topic)
    {
        //Passes true unless a player doesnt have enough points in any ability
        bool passCheck = true;

        if (topic.topicAbilityChecks.body != 0 && _playerCharacterManager.abilities.body < topic.topicAbilityChecks.body)
        {
            passCheck = false;
        }
        else if (topic.topicAbilityChecks.hands != 0 && _playerCharacterManager.abilities.hands < topic.topicAbilityChecks.hands)
        {
            passCheck = false;
        }
        else if (topic.topicAbilityChecks.mind != 0 && _playerCharacterManager.abilities.mind < topic.topicAbilityChecks.mind)
        {
            passCheck = false;
        }
        else if (topic.topicAbilityChecks.heart != 0 && _playerCharacterManager.abilities.heart < topic.topicAbilityChecks.heart)
        {
            passCheck = false;
        }

        return passCheck;
    }

    //Used for displaying Ability checks
    private string SetTopicButtonText(DialogueTopicsNode.Topic topic)
    {
        string newTopicText = topic.topicTitle;

        if (topic.topicAbilityChecks.body != 0)
        {
            newTopicText += " [Body " + topic.topicAbilityChecks.body.ToString() + "]";
        }
        else if (topic.topicAbilityChecks.hands != 0)
        {
            newTopicText += " [Hands " + topic.topicAbilityChecks.hands.ToString() + "]";
        }
        else if (topic.topicAbilityChecks.mind != 0)
        {
            newTopicText += " [Mind " + topic.topicAbilityChecks.mind.ToString() + "]";
        }
        else if (topic.topicAbilityChecks.heart != 0)
        {
            newTopicText += " [Heart " + topic.topicAbilityChecks.heart.ToString() + "]";
        }

        return newTopicText;
    }

    private void ClearAllTopicButtons()
    {
        //Delete all the buttons
        for (int i = topicButtonsToDelete.Count - 1; i >= 0; i--)
        {
            Destroy(topicButtonsToDelete[i]);
        }

        topicButtonsToDelete.Clear();
    }

    public void EndDialogue()
    {
        ClearAllTopicButtons();
        currentCharacterManager = null;
        _currentDialogueGraph = null;
        _currentSentencesNode = null;
        _currentSentence = 0;
        GameManager.instance.UnPauseGame(true);
        dialogueUI.SetActive(false);

        //Enable the active UI
        _activeUi.SetActive(true);
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
        //If the character is a non player manager then show the description
        if(currentCharacterManager is NonPlayerCharacterManager)
        {
            descriptionText.text = (currentCharacterManager as NonPlayerCharacterManager).characterDescription;
            descriptionBox.SetActive(true);
        }
    }

    private void CloseCharacterDescription()
    {
        descriptionText.text = "";
        descriptionBox.SetActive(false);
    }

    private void ReturnToEntryNode()
    {
        //For getting the alive or wounded dialogue branches
        foreach (DialogueBaseNode node in _currentDialogueGraph.nodes)
        {
            if (node.GetNodeType() == "entry")
            {
                _currentDialogueGraph.current = node;
                break;
            }
        }

        //If there is a valid character manager then set the greeting by state
        if(currentCharacterManager != null)
        {
            //checks if alive or wounded, then moves to the appropriate topic node
            if (currentCharacterManager.characterState == CharacterState.alive)
            {
                foreach (NodePort port in _currentDialogueGraph.current.Ports)
                {
                    if (port.fieldName == "exitAlive")
                    {
                        _currentDialogueGraph.current = port.Connection.node as DialogueBaseNode;
                        break;
                    }
                }
            }
            else if (currentCharacterManager.characterState == CharacterState.wounded)
            {
                foreach (NodePort port in _currentDialogueGraph.current.Ports)
                {
                    if (port.fieldName == "exitWounded")
                    {
                        _currentDialogueGraph.current = port.Connection.node as DialogueBaseNode;
                        break;
                    }
                }
            }
        }
        //Otherwise just proceed to the alive port
        else
        {
            foreach (NodePort port in _currentDialogueGraph.current.Ports)
            {
                if (port.fieldName == "exitAlive")
                {
                    _currentDialogueGraph.current = port.Connection.node as DialogueBaseNode;
                    break;
                }
            }
        }

    }

    private void LoadNextNode()
    {
        //Goes to next node
        foreach (NodePort port in _currentDialogueGraph.current.Ports)
        {
            if (port.fieldName == "exit")
            {
                if (port.Connection != null)
                {
                    _currentDialogueGraph.current = port.Connection.node as DialogueBaseNode;
                }
                else
                {
                    _currentDialogueGraph.current = null;
                }
                break;
            }
        }

        RunNewNode();
    }

    private void LoadNextNodeViaTopic(DialogueTopicsNode.Topic topic)
    {
        int topicNum = 0;

        foreach (DialogueTopicsNode.Topic topicSearch in (_currentDialogueGraph.current as DialogueTopicsNode).topics)
        {
            if (topicSearch == topic)
            {
                break;
            }
            topicNum++;
        }

        //Goes to next node
        foreach (NodePort port in _currentDialogueGraph.current.Ports)
        {
            if (port.fieldName == "topics " + topicNum.ToString())
            {
                if(port.Connection != null)
                {
                    _currentDialogueGraph.current = port.Connection.node as DialogueBaseNode;
                }
                else
                {
                    _currentDialogueGraph.current = null;
                }
                break;
            }
        }

        RunNewNode();
    }

    private void RunNewNode()
    {
        //Check the loaded node//
        //Checks if the port is topic or not
        if (_currentDialogueGraph.current is DialogueTopicsNode)
        {
            SpawnTopicButtons();
        }
        //If the node is a sentences node
        else if(_currentDialogueGraph.current is DialogueSentencesNode)
        {
            _currentSentencesNode = _currentDialogueGraph.current as DialogueSentencesNode;
            _currentSentence = 0;

            dialogueNextButton.SetActive(true);
            dialogueLeaveButton.gameObject.SetActive(false);
            dialogueText.text = ReplaceText(_currentSentencesNode.sentences[_currentSentence]);
        }
        //If the node is a state set node
        else if (_currentDialogueGraph.current is DialogueStateSetNode)
        {
            //Go through each statecheck in the node and add to the player
            foreach (StateCheck stateCheck in (_currentDialogueGraph.current as DialogueStateSetNode).dialogueAddStateCheck)
            {
                _playerCharacterManager.stateChecks.Add(stateCheck);
            }

            LoadNextNode();
        }
        //If it is a jorunal entry node
        else if (_currentDialogueGraph.current is DialogueJournalEntryNode)
        {
            _playerCharacterManager.AddJournalEntry((_currentDialogueGraph.current as DialogueJournalEntryNode).entryToAdd);
            LoadNextNode();
        }
        //If it is a load level node
        else if (_currentDialogueGraph.current is DialogueLoadLevelNode)
        {
            SceneLoader.instance.LoadPlayerScene((_currentDialogueGraph.current as DialogueLoadLevelNode).targetScene.BuildIndex, (_currentDialogueGraph.current as DialogueLoadLevelNode).spawnPointName, Vector3.zero, Vector3.zero, true, true);
        }
        //If it is a give object node
        else if (_currentDialogueGraph.current is DialogueGiveItemNode)
        {
            foreach (Item item in (_currentDialogueGraph.current as DialogueGiveItemNode).itemsToGive)
            {
                _playerCharacterManager.AddItem(item);
            }
            
            LoadNextNode();
        }
        //If it is a action node
        else if (_currentDialogueGraph.current is DialogueStateActionNode)
        {
            foreach (Action action in (_currentDialogueGraph.current as DialogueStateActionNode).actionToRun)
            {
                action.StartAction();
            }
            LoadNextNode();
        }
        //If it is a quit node then exit dialogue completely
        else if (_currentDialogueGraph.current is DialogueQuitNode)
        {
            EndDialogue();
        }
        //If not the above return to the entry node
        else
        {
            ReturnToEntryNode();

            SpawnTopicButtons();
            dialogueLeaveButton.gameObject.SetActive(true);
        }
    }

    //This takes in a string and replaces all relevant cases with the players data such as name and pronouns
    private string ReplaceText(string s)
    {
        string newS = s.Replace("%pcName", _playerCharacterManager.characterName);

        return newS;
    }
}
