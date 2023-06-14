using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : Singleton<DataManager>
{
    [Header("References")]
    [SerializeField] GameObject _itemContainerPrefab;
    [SerializeField] ScriptableObjectDatabase scriptableObjectDatabase;
    [SerializeField] SpriteDatabase spriteDatabase;

    //This script is a container of all tracked scenes with their associated data
    [Header("Data")]
    [SerializeField] List<SceneData> _trackedScenes = new();

    private PlayerDataTracker _playerData;

    [ReadOnly] [SerializeField] List<NonPlayerCharacterManager> _activeNPCharacters = new();

    private void Start()
    {
        keepAlive = true;
    }

    public void AddActiveCharacter(NonPlayerCharacterManager character)
    {
        if (!_activeNPCharacters.Contains(character))
        {
            _activeNPCharacters.Add(character);
        }
        else
        {
            Debug.Log("Adding existing character");
            return;
        }
    }

    public void RemoveActiveCharacter(NonPlayerCharacterManager character)
    {
        if (_activeNPCharacters.Contains(character))
        {
            _activeNPCharacters.Remove(character);
        }
        else
        {
            Debug.Log("Tried removing non existent character");
            return;
        }
    }

    //Empty list of chars in scene when loading a new one
    public void ClearActiveCharacters()
    {
        _activeNPCharacters.Clear();
    }

    public List<NonPlayerCharacterManager> GetActiveCharacters()
    {
        return _activeNPCharacters;
    }

    public void SetPlayerTracker(PlayerDataTracker dataTracker)
    {
        _playerData = dataTracker;
    }

    public void SaveSaveSlot(int i)
    {
        if (_playerData == null)
        {
            _playerData = new();
        }

        //Save current data
        SaveSceneData(SceneLoader.instance.GetCurrentScene());

        //Clear the save directory if it already exists
        if (Directory.Exists(Application.dataPath + "/Saves/save" + i.ToString())) { Directory.Delete(Application.dataPath + "/Saves/save" + i.ToString(), true); }

        //Create the save directory
        Directory.CreateDirectory(Application.dataPath + "/Saves/save" + i.ToString());

        //Saves player data text file to a folder named savei where i is the saveslot
        string playerDataJson = JsonUtility.ToJson(_playerData);
        File.WriteAllText(Application.dataPath + "/Saves/save" + i.ToString() + "/" + "PlayerData" + ".txt", playerDataJson);

        //Saves each tracked scene to its own text files in the same folder
        foreach (SceneData sceneData in _trackedScenes)
        {
            string sceneDataJson = JsonUtility.ToJson(sceneData);
            File.WriteAllText(Application.dataPath + "/Saves/save" + i.ToString() + "/" + "SceneData_" + sceneData.trackedSceneName + ".txt", sceneDataJson);
        }

        MessageBox.instance.Create("Saved your game!", true);
    }

    public void LoadSaveSlot(int i)
    {
        if (!Directory.Exists(Application.dataPath + "/Saves/save" + i.ToString()))
        {
            MessageBox.instance.Create("This save slot is empty!", false);
            return;
        }

        //Get the player data
        string playerSaveData = File.ReadAllText(Application.dataPath + "/Saves/save" + i.ToString() + "/" + "PlayerData" + ".txt");
        _playerData = JsonUtility.FromJson<PlayerDataTracker>(playerSaveData);

        //Create a list of all files in the save directory
        List<string> sceneDataFileNames = new();
        string[] files = Directory.GetFiles(Application.dataPath + "/Saves/save" + i.ToString());

        //Clear tracked data
        _trackedScenes.Clear();

        //Only track those that are labelled as SceneData
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            if (fileName.Contains("SceneData_") && !fileName.Contains(".meta"))
            {
                sceneDataFileNames.Add(fileName);
            }
        }

        foreach (string sceneDataName in sceneDataFileNames)
        {
            string sceneDataString = File.ReadAllText(Application.dataPath + "/Saves/save" + i.ToString() + "/" + sceneDataName);
            SceneData sceneData = JsonUtility.FromJson<SceneData>(sceneDataString);
            _trackedScenes.Add(sceneData);
        }

        SceneLoader.instance.LoadPlayerScene(_playerData.currentScene, null, _playerData.characterPosition, _playerData.characterRotation, false, true);

        Debug.Log("Loaded Game");
    }

    public void CheckLoadedScene()
    {
        Scene currentScene = SceneManager.GetSceneByBuildIndex(SceneLoader.instance.GetCurrentScene());

        if (currentScene.buildIndex == -1)
        {
            Debug.Log("Checked scene not loaded");
            return;
        }

        //Check list of tracked scenes for existing scene tracker
        foreach (SceneData sceneData in _trackedScenes)
        {
            if (sceneData.trackedSceneName == currentScene.name)
            {
                LoadSceneData(sceneData.trackedSceneName);
                return;
            }
        }

        //If none are found create a new scene tracker
        CreateNewTrackedScene(currentScene);
    }

    //Collects all data types from a scene when it is closed
    public bool SaveSceneData(int buildIndex)
    {
        SavePlayerTracker();

        List<CharacterManager> characterManagers = new(GameObject.FindObjectsOfType<CharacterManager>());
        foreach (CharacterManager character in characterManagers)
        {
            if (character.gameObject.scene.buildIndex == buildIndex)
            {
                SaveCharacterTracker(character);
            }
        }

        List<ItemContainer> itemContainers = new(GameObject.FindObjectsOfType<ItemContainer>());
        foreach (ItemContainer itemContainer in itemContainers)
        {
            if (itemContainer.gameObject.scene.buildIndex == buildIndex)
            {
                SaveContainerTracker(itemContainer);
            }
        }

        List<TriggerController> triggers = new(GameObject.FindObjectsOfType<TriggerController>());
        foreach (TriggerController trigger in triggers)
        {
            if (trigger.triggered)
            {
                SaveTriggerTracker(trigger);
            }
        }

        //returns a bool at end to let sceneloader know the data has been saved
        return true;
    }

    private void SavePlayerTracker()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            //No player object in scene
            return;
        }

        PlayerCharacterManager playerCharacterManager = playerObject.GetComponent<PlayerCharacterManager>();

        if (playerCharacterManager == null)
        {
            Debug.Log("trying to save player data with no active player");
            return;
        }

        //The character sheet data
        _playerData.playerName = playerCharacterManager.characterName;
        _playerData.pronounInt = ((int)playerCharacterManager.characterPronouns);

        _playerData.colorHair = ColorUtility.ToHtmlStringRGBA(playerCharacterManager.characterHairColor);

        if (playerCharacterManager.characterHair != null)
        {
            _playerData.hairSprite = playerCharacterManager.characterHair.name;
        }
        else
        {
            _playerData.hairSprite = "";
        }

        if (playerCharacterManager.characterBeard != null)
        {
            _playerData.beardSprite = playerCharacterManager.characterBeard.name;
        }
        else
        {
            _playerData.beardSprite = "";
        }


        //The player character manager specific data
        _playerData.currentScene = SceneLoader.instance.GetCurrentScene();
        _playerData.characterPosition = playerCharacterManager.transform.position;
        _playerData.characterRotation = playerCharacterManager.transform.rotation.eulerAngles;

        _playerData.healthCurrent = playerCharacterManager.healthCurrent;
        _playerData.staminaCurrent = playerCharacterManager.staminaCurrent;

        _playerData.bodyLevel = playerCharacterManager.abilities.body;
        _playerData.handsLevel = playerCharacterManager.abilities.hands;
        _playerData.mindLevel = playerCharacterManager.abilities.mind;
        _playerData.heartLevel = playerCharacterManager.abilities.heart;

        _playerData.currentInventory.Clear();
        foreach (Item item in playerCharacterManager.currentInventory)
        {
            _playerData.currentInventory.Add(item.uniqueID);
        }

        _playerData.currentSpells.Clear();
        foreach (Spell spell in playerCharacterManager.currentSpells)
        {
            _playerData.currentSpells.Add(spell.uniqueID);
        }

        _playerData.currentSkills.Clear();
        foreach (Skill skill in playerCharacterManager.currentSkills)
        {
            _playerData.currentSkills.Add(skill.uniqueID);
        }

        //Equipped
        if (playerCharacterManager.equippedWeapon != null) { _playerData.equippedWeapon = playerCharacterManager.equippedWeapon.GetUniqueID(); } else { _playerData.equippedWeapon = ""; };
        if (playerCharacterManager.equippedShield != null) { _playerData.equippedShield = playerCharacterManager.equippedShield.GetUniqueID(); } else { _playerData.equippedShield = ""; };

        if (playerCharacterManager.equippedArmour != null) { _playerData.equippedArmour = playerCharacterManager.equippedArmour.GetUniqueID(); } else { _playerData.equippedArmour = ""; };
        if (playerCharacterManager.equippedCape != null) { _playerData.equippedCape = playerCharacterManager.equippedCape.GetUniqueID(); } else { _playerData.equippedCape = ""; };
        if (playerCharacterManager.equippedFeet != null) { _playerData.equippedFeet = playerCharacterManager.equippedFeet.GetUniqueID(); } else { _playerData.equippedFeet = ""; };
        if (playerCharacterManager.equippedGreaves != null) { _playerData.equippedGreaves = playerCharacterManager.equippedGreaves.GetUniqueID(); } else { _playerData.equippedGreaves = ""; };
        if (playerCharacterManager.equippedHands != null) { _playerData.equippedHands = playerCharacterManager.equippedHands.GetUniqueID(); } else { _playerData.equippedHands = ""; };
        if (playerCharacterManager.equippedHelmet != null) { _playerData.equippedHelmet = playerCharacterManager.equippedHelmet.GetUniqueID(); } else { _playerData.equippedHelmet = ""; };
        if (playerCharacterManager.equippedPants != null) { _playerData.equippedPants = playerCharacterManager.equippedPants.GetUniqueID(); } else { _playerData.equippedPants = ""; };
        if (playerCharacterManager.equippedShirt != null) { _playerData.equippedShirt = playerCharacterManager.equippedShirt.GetUniqueID(); } else { _playerData.equippedShirt = ""; };

        PlayerMagic playerMagic = playerCharacterManager.GetPlayerMagic();

        if (playerMagic.GetEquippedSpell(1) != null) { _playerData.equippedSpell1 = playerCharacterManager.GetPlayerMagic().GetEquippedSpell(1).uniqueID; } else { _playerData.equippedSpell1 = ""; };
        if (playerMagic.GetEquippedSpell(2) != null) { _playerData.equippedSpell2 = playerCharacterManager.GetPlayerMagic().GetEquippedSpell(2).uniqueID; } else { _playerData.equippedSpell2 = ""; };

        if (playerMagic.GetLearnedSpell(1) != null) { _playerData.learnedSpell1 = playerCharacterManager.GetPlayerMagic().GetLearnedSpell(1).uniqueID; } else { _playerData.learnedSpell1 = ""; };
        if (playerMagic.GetLearnedSpell(2) != null) { _playerData.learnedSpell2 = playerCharacterManager.GetPlayerMagic().GetLearnedSpell(2).uniqueID; } else { _playerData.learnedSpell2 = ""; };

        _playerData.currentEffects = playerCharacterManager.currentEffects;

        _playerData.journalEntries.Clear();
        foreach (JournalEntry entry in playerCharacterManager.journalEntries)
        {
            _playerData.journalEntries.Add(entry);
        }

        _playerData.stateChecks.Clear();
        foreach (StateCheck stateCheck in playerCharacterManager.stateChecks)
        {
            _playerData.stateChecks.Add(stateCheck.uniqueID);
        }

        _playerData.alreadyRunDialogueTopics = playerCharacterManager.alreadyRunDialogueTopics;

        _playerData.currentDay = TimeController.GetDays();
        _playerData.currentHour = TimeController.GetHours();
        _playerData.currentMinute = TimeController.GetMinutes();
    }

    private void SaveCharacterTracker(CharacterManager characterManager)
    {
        Scene targetScene = characterManager.gameObject.scene;
        CharacterDataTracker targetTracker = null;

        //Find the relevant tracked scene
        foreach (SceneData sceneData in _trackedScenes)
        {
            //Find the relevant tracked character
            if (sceneData.trackedSceneName == targetScene.name)
            {
                foreach (CharacterDataTracker characterData in sceneData.characterDataTrackers)
                {
                    //if there is already a trackedCharacter entry
                    if (characterData.characterName == characterManager.name)
                    {
                        targetTracker = characterData;
                        break;
                    }
                }

                if (targetTracker != null)
                {
                    break;
                }

                CharacterDataTracker newCharacterDataTracker = new();
                sceneData.characterDataTrackers.Add(newCharacterDataTracker);
                targetTracker = newCharacterDataTracker;
                break;
            }
        }

        if (targetTracker == null)
        {
            Debug.Log("Invalid tracked character");
            return;
        }

        //The different stats to save//
        targetTracker.characterName = characterManager.name;

        targetTracker.characterPosition = characterManager.transform.position;

        targetTracker.healthCurrent = characterManager.healthCurrent;
        targetTracker.staminaCurrent = characterManager.staminaCurrent;

        foreach (Item item in characterManager.currentInventory)
        {
            targetTracker.currentInventory.Add(item.uniqueID);
        }

        targetTracker.currentEffects = characterManager.currentEffects;

        targetTracker.characterState = characterManager.characterState;

    }

    private void SaveContainerTracker(ItemContainer container)
    {
        Scene targetScene = container.gameObject.scene;
        ContainerDataTracker targetTracker = null;

        //Find the relevant tracked scene
        foreach (SceneData sceneData in _trackedScenes)
        {
            //Find the relevant tracked container
            if (sceneData.trackedSceneName == targetScene.name)
            {
                foreach (ContainerDataTracker containerData in sceneData.containerDataTrackers)
                {
                    //if there is already a trackedContainer entry
                    if (containerData.containerName == container.name)
                    {
                        targetTracker = containerData;
                        break; ;
                    }
                }

                ContainerDataTracker newContainerDataTracker = new();
                sceneData.containerDataTrackers.Add(newContainerDataTracker);
                targetTracker = newContainerDataTracker;
                break;
            }
        }

        //The different stats to save//
        targetTracker.containerName = container.name;

        targetTracker.deleteEmpty = container.deleteEmpty;

        targetTracker.inventory = container.inventory;

        targetTracker.containerPosition = container.transform.position;
    }

    private void SaveTriggerTracker(TriggerController trigger)
    {
        Scene targetScene = trigger.gameObject.scene;

        //Find the relevant tracked scene
        foreach (SceneData sceneData in _trackedScenes)
        {
            if (sceneData.trackedSceneName == targetScene.name)
            {
                if (!sceneData.triggeredTriggerIDs.Contains(trigger.uniqueID))
                {
                    sceneData.triggeredTriggerIDs.Add(trigger.uniqueID);
                }
            }
        }
    }

    private void LoadSceneData(string trackedSceneName)
    {
        SceneData targetSceneData = null;

        foreach (SceneData sceneData in _trackedScenes)
        {
            if (sceneData.trackedSceneName == trackedSceneName)
            {
                targetSceneData = sceneData;
                break;
            }
        }

        LoadPlayerData();
        LoadSceneDataCharacters(trackedSceneName, targetSceneData);
        LoadSceneDataContainers(trackedSceneName, targetSceneData);
        LoadSceneDataTriggers(trackedSceneName, targetSceneData);
    }

    private void LoadPlayerData()
    {
        PlayerCharacterManager playerCharacterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterManager>();

        if (playerCharacterManager == null)
        {
            Debug.Log("trying to save player data with no active player");
            return;
        }

        //Character sheet
        playerCharacterManager.characterName = _playerData.playerName;
        playerCharacterManager.characterPronouns = (CharacterPronouns)_playerData.pronounInt;

        ColorUtility.TryParseHtmlString("#" + _playerData.colorHair, out Color hair);

        playerCharacterManager.characterHairColor = hair;

        ColorUtility.TryParseHtmlString("#" + _playerData.colorSkin, out Color skin);

        playerCharacterManager.characterSkintone = skin;

        if (_playerData.hairSprite != "") { playerCharacterManager.characterHair = spriteDatabase.GetHairFromName(_playerData.hairSprite); }
        else { playerCharacterManager.characterHair = null; }
        if (_playerData.beardSprite != "") { playerCharacterManager.characterBeard = spriteDatabase.GetBeardFromName(_playerData.beardSprite); }
        else { playerCharacterManager.characterBeard = null; }


        //Player manager
        playerCharacterManager.healthCurrent = _playerData.healthCurrent;
        playerCharacterManager.staminaCurrent = _playerData.staminaCurrent;

        playerCharacterManager.abilities.body = _playerData.bodyLevel;
        playerCharacterManager.abilities.hands = _playerData.handsLevel;
        playerCharacterManager.abilities.mind = _playerData.mindLevel;
        playerCharacterManager.abilities.heart = _playerData.heartLevel;

        playerCharacterManager.currentInventory.Clear();
        foreach (string uniqueID in _playerData.currentInventory)
        {
            Item item = Instantiate(scriptableObjectDatabase.GetItemFromID(uniqueID));

            playerCharacterManager.currentInventory.Add(item);

            if (item.uniqueID == _playerData.equippedWeapon) { playerCharacterManager.EquipItem("weapon", item); }
            if (item.uniqueID == _playerData.equippedShield) { playerCharacterManager.EquipItem("shield", item); }

            if (item.uniqueID == _playerData.equippedArmour) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedCape) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedFeet) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedGreaves) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedHands) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedHelmet) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedPants) { playerCharacterManager.EquipItem("equipment", item); }
            if (item.uniqueID == _playerData.equippedShirt) { playerCharacterManager.EquipItem("equipment", item); }
        }

        playerCharacterManager.currentSpells.Clear();
        foreach (string uniqueID in this._playerData.currentSpells)
        {
            Spell spell = Instantiate(scriptableObjectDatabase.GetSpellFromID(uniqueID));
            playerCharacterManager.currentSpells.Add(spell);

            if (spell.uniqueID == this._playerData.learnedSpell1) { playerCharacterManager.GetPlayerMagic().LearnSpell(spell); };
            if (spell.uniqueID == this._playerData.learnedSpell2) { playerCharacterManager.GetPlayerMagic().LearnSpell(spell); };

            if (spell.uniqueID == this._playerData.equippedSpell1) { playerCharacterManager.GetPlayerMagic().PrepareSpell(spell); };
            if (spell.uniqueID == this._playerData.equippedSpell2) { playerCharacterManager.GetPlayerMagic().PrepareSpell(spell); };
        }

        playerCharacterManager.currentSkills.Clear();
        foreach (string uniqueID in this._playerData.currentSkills)
        {
            Skill skill = Instantiate(scriptableObjectDatabase.GetSkillFromID(uniqueID));
            playerCharacterManager.AddSkill(skill);
        }

        foreach (Effect effect in this._playerData.currentEffects)
        {
            playerCharacterManager.currentEffects.Add(effect);
        }

        playerCharacterManager.journalEntries.Clear();
        foreach (JournalEntry entry in this._playerData.journalEntries)
        {
            playerCharacterManager.journalEntries.Add(entry);
        }

        playerCharacterManager.stateChecks.Clear();
        foreach (string uniqueID in this._playerData.stateChecks)
        {
            StateCheck stateCheck = scriptableObjectDatabase.GetStateCheckFromID(uniqueID);
            playerCharacterManager.stateChecks.Add(stateCheck);
        }

        playerCharacterManager.alreadyRunDialogueTopics = this._playerData.alreadyRunDialogueTopics;

        TimeController.SetTrackedTime(this._playerData.currentDay, this._playerData.currentHour, this._playerData.currentMinute);

        playerCharacterManager.LoadPlayer();
    }

    private void LoadSceneDataCharacters(string trackedSceneName, SceneData sceneData)
    {
        List<NonPlayerCharacterManager> sceneCharacters = _activeNPCharacters;

        foreach (NonPlayerCharacterManager character in sceneCharacters)
        {
            if (character.gameObject.scene.name != trackedSceneName)
            {
                break;
            }

            foreach (CharacterDataTracker characterDataTracker in sceneData.characterDataTrackers)
            {
                if (character.name == characterDataTracker.characterName)
                {
                    character.transform.position = characterDataTracker.characterPosition;

                    character.healthCurrent = characterDataTracker.healthCurrent;
                    character.staminaCurrent = characterDataTracker.staminaCurrent;

                    //Clear the inventory
                    character.currentInventory.Clear();

                    //Add items via uniqueID
                    foreach (string uniqueID in characterDataTracker.currentInventory)
                    {
                        Item item = Instantiate(scriptableObjectDatabase.GetItemFromID(uniqueID));

                        character.currentInventory.Add(item);
                    }

                    character.currentEffects = characterDataTracker.currentEffects;

                    character.characterState = characterDataTracker.characterState;

                    character.SetCharacterState();
                    break;
                }
            }
        }
    }

    private void LoadSceneDataContainers(string trackedSceneName, SceneData sceneData)
    {
        List<ItemContainer> sceneContainers = new(GameObject.FindObjectsOfType<ItemContainer>());

        foreach (ContainerDataTracker containerDataTracker in sceneData.containerDataTrackers)
        {
            bool foundExistingContainer = false;
            foreach (ItemContainer container in sceneContainers)
            {
                if (container.gameObject.scene.name != trackedSceneName)
                {
                    break;
                }

                if (container.name == containerDataTracker.containerName)
                {
                    foundExistingContainer = true;

                    container.inventory = containerDataTracker.inventory;

                    container.deleteEmpty = containerDataTracker.deleteEmpty;

                    container.transform.position = containerDataTracker.containerPosition;
                    break;
                }
            }

            if (!foundExistingContainer && containerDataTracker.inventory.Count != 0 || !foundExistingContainer && containerDataTracker.inventory.Count == 0 && !containerDataTracker.deleteEmpty)
            {
                //Create new container if none in scene
                ItemContainer newItemContainer = Instantiate(_itemContainerPrefab, containerDataTracker.containerPosition, Quaternion.identity).GetComponent<ItemContainer>();

                newItemContainer.inventory = containerDataTracker.inventory;

                newItemContainer.deleteEmpty = containerDataTracker.deleteEmpty;

                newItemContainer.transform.position = containerDataTracker.containerPosition;

                SceneLoader.instance.MoveObjectToScene(newItemContainer.gameObject);
            }
        }
    }

    private void LoadSceneDataTriggers(string trackedSceneName, SceneData targetSceneData)
    {
        //Get a list of all triggers in the scene
        List<TriggerController> sceneTriggers = new(GameObject.FindObjectsOfType<TriggerController>());

        //Go through them all
        foreach (TriggerController trigger in sceneTriggers)
        {
            //If the scene data tracker contains the trigger set it to already triggered
            if (targetSceneData.triggeredTriggerIDs.Contains(trigger.uniqueID))
            {
                trigger.triggered = true;
            }
        }
    }

    private void CreateNewTrackedScene(Scene loadedScene)
    {
        SceneData newTrackedScene = new();
        newTrackedScene.trackedSceneName = loadedScene.name;

        _trackedScenes.Add(newTrackedScene);

        LoadPlayerData();
    }
}
