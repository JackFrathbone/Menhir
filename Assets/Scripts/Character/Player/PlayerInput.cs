using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject _characterMenu;
    [SerializeField] GameObject _pauseMenu;

    private PlayerInventory _playerInventory;
    private PlayerMagic _playerMagic;
    private PlayerActiveUI _playerActiveUI;
    private PlayerDialogueController _playerDialogueController;
    private PlayerJournalDisplay _playerJournalDisplay;

    private PlayerCombat _playerCombat;
    private PlayerCharacterManager _playerCharacterManager;

    private GameObject _target;
    private enum ActivateMode{disable, search, talk, door};
    private ActivateMode _activateMode;

    private void Awake()
    {
        GameManager.instance.PlayerUIObject = this.gameObject;
    }

    private void Start()
    {
        _playerInventory = GetComponent<PlayerInventory>();
        _playerMagic = GetComponent<PlayerMagic>();
        _playerActiveUI = GetComponent<PlayerActiveUI>();
        _playerDialogueController = GetComponent<PlayerDialogueController>();
        _playerJournalDisplay = GetComponent<PlayerJournalDisplay>();

        _playerCombat = GameManager.instance.playerObject.GetComponent<PlayerCombat>();
        _playerCharacterManager = GameManager.instance.playerObject.GetComponent<PlayerCharacterManager>();
    }

    private void Update()
    {
        CheckCursor();
        CheckButtons();
    }

    private void CheckCursor()
    {
        if (GameManager.instance.isPaused)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, 2f))
        {
            _target = hit.collider.gameObject;

            if (hit.collider.CompareTag("Character"))
            {
                CharacterManager targetChar = hit.collider.GetComponentInParent<CharacterManager>();

                //If alive and friendly// Compares character faction to target faction
                if ((targetChar.characterState == CharacterState.alive && !Factions.FactionHostilityCheck(_playerCharacterManager.characterSheet.characterFaction, targetChar.characterSheet.characterFaction, targetChar.characterSheet.characterAggression)) || targetChar.characterState == CharacterState.wounded)
                {
                    //Check here if there is dialogue
                    _playerActiveUI.EnableCrosshairText("Talk to");
                    _activateMode = ActivateMode.talk;
                }
                else if (targetChar.characterState == CharacterState.dead)
                {
                    _playerActiveUI.EnableCrosshairText("Search");
                    _activateMode = ActivateMode.search;
                }
            }
            else if (hit.collider.CompareTag("ItemContainer"))
            {
                _playerActiveUI.EnableCrosshairText("Search");
                _activateMode = ActivateMode.search;
            }
            else if (hit.collider.CompareTag("Door"))
            {
                _playerActiveUI.EnableCrosshairText("Use Door");
                _activateMode = ActivateMode.door;
            }
            else
            {
                _playerActiveUI.DisableCrosshairText();
            }
        }
        else
        {
            _target = null;
            _activateMode = ActivateMode.disable;
            _playerActiveUI.DisableCrosshairText();
        }
    }

    private void CheckButtons()
    {
        //For opening pause menu
        if (Input.GetButtonDown("CharacterMenu"))
        {
            if (GameManager.instance.CheckCanPause("characterMenu"))
            {
                if (!GameManager.instance.isPaused)
                {
                    GameManager.instance.PauseGame(true, "characterMenu");
                    _playerInventory.RefreshInventory();
                    _playerMagic.RefreshSpells();
                    _playerJournalDisplay.RefreshCurrentQuests();
                    _characterMenu.SetActive(true);
                }
                else
                {
                    GameManager.instance.UnPauseGame(true);
                    _playerInventory.GetComponent<PlayerInventoryDescription>().CloseDescription();
                    _playerInventory.CloseSearchInventory();
                    _characterMenu.SetActive(false);
                }
            }
        }

        if (Input.GetButtonDown("PauseMenu"))
        {
            if (GameManager.instance.CheckCanPause("pauseMenu"))
            {
                if (!GameManager.instance.isPaused)
                {
                    GameManager.instance.PauseGame(true, "pauseMenu");
                    _pauseMenu.SetActive(true);
                }
                else
                {
                    GameManager.instance.UnPauseGame(true);
                    _pauseMenu.SetActive(false);
                }
            }
        }

        if (Input.GetButtonDown("ReadyWeapon"))
        {
            _playerCharacterManager.ToggleWeapon();
        }

        //Check if the attack is pressed
        if (Input.GetButtonDown("ClickLeft"))
        {
            if (_playerCharacterManager.weaponOut && !GameManager.instance.isPaused)
            {
                _playerCombat.TriggerAttack();
            }
        }
        //Check if it goes up
        if (Input.GetButtonUp("ClickLeft"))
        {
            if (_playerCharacterManager.weaponOut && !GameManager.instance.isPaused)
            {
                _playerCombat.TriggerAttackEnd();
            }
        }

        if (Input.GetButtonDown("Spell1"))
        {
            _playerMagic.SetSelectedSpell(1);
        }

        if (Input.GetButtonDown("Spell2"))
        {
            _playerMagic.SetSelectedSpell(2);
        }

        if(Input.GetButtonDown("CastSpell"))
        {
            _playerMagic.CastSpell(_playerMagic.GetSelectedSpell());
        }

        //Activate objects that are currently looked at
        if (Input.GetButtonDown("Activate"))
        {
            if(_activateMode != ActivateMode.disable && _target != null && !GameManager.instance.isPaused)
            {
                switch (_activateMode)
                {
                    case ActivateMode.search:
                        //If container is a dead character

                            if (_target.CompareTag("Character") && GameManager.instance.CheckCanPause("characterMenu"))
                        {
                            GameManager.instance.PauseGame(true, "characterMenu");
                            _playerInventory.RefreshInventory();
                            _playerMagic.RefreshSpells();
                            _characterMenu.SetActive(true);
                            _playerInventory.OpenSearchInventory(_target.GetComponentInParent<ItemContainer>());
                        }
                        else if(_target.CompareTag("ItemContainer") && GameManager.instance.CheckCanPause("characterMenu"))
                        {
                            GameManager.instance.PauseGame(true, "characterMenu");
                            _playerInventory.RefreshInventory();
                            _playerMagic.RefreshSpells();
                            _characterMenu.SetActive(true);
                            _playerInventory.OpenSearchInventory(_target.GetComponent<ItemContainer>());
                        }
                        break;
                    case ActivateMode.talk:
                        if (_target.CompareTag("Character") && GameManager.instance.CheckCanPause("dialogueMenu"))
                        {
                            GameManager.instance.PauseGame(true, "dialogueMenu");
                            _playerDialogueController.StartDialogue(_target.GetComponentInParent<CharacterManager>());
                        }
                        else
                        {
                            Debug.Log("not valid character for dialogue or paused");
                            return;
                        }
                        break;
                    case ActivateMode.door:
                        _target.GetComponent<LoadingDoor>().ActivateLoadingDoor();
                        break;
                }
            }
        }
    }
}
