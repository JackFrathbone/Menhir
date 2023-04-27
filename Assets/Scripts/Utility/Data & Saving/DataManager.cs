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
    private List<SceneData> _trackedScenes = new();

    private PlayerDataTracker _playerData;

    public List<CharacterManager> activeCharacters = new();

    private void Start()
    {
        keepAlive = true;
    }

    public void SaveSaveSlot(int i)
    {
        if(_playerData == null)
        {
            _playerData = new();
        }

        //Save current data
        SavePlayerTracker();
        SaveSceneData(SceneLoader.instance.GetCurrentScene());

        //Create a new saveSlot
        SaveSlot saveSlot = new()
        {
            saveSlotIndex = i,
            savedScenes = _trackedScenes,
            playerData = _playerData
        };

        string json = JsonUtility.ToJson(saveSlot);

        File.WriteAllText(Application.dataPath + "/Saves/save" + i.ToString() + ".txt", json);

        Debug.Log("Saved Game");
    }

    public void LoadSaveSlot(int i)
    {
        if (!File.Exists(Application.dataPath + "/Saves/save" + i.ToString() + ".txt"))
        {
            return;
        }

        string saveSlotPath = File.ReadAllText(Application.dataPath + "/Saves/save" + i.ToString() + ".txt");

        SaveSlot saveSlot = JsonUtility.FromJson<SaveSlot>(saveSlotPath);

        _trackedScenes = saveSlot.savedScenes;
        _playerData = saveSlot.playerData;

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
        _playerData.playerName = playerCharacterManager.characterSheet.characterName;
        _playerData.pronounInt = ((int)playerCharacterManager.characterSheet.characterPronouns);

        _playerData.colorHair = ColorUtility.ToHtmlStringRGBA(playerCharacterManager.characterSheet.characterHairColor);

        if(playerCharacterManager.characterSheet.characterHair != null)
        {
            _playerData.hairSprite = playerCharacterManager.characterSheet.characterHair.name;
        }
        else
        {
            _playerData.hairSprite = "";
        }

        if (playerCharacterManager.characterSheet.characterBeard != null)
        {
            _playerData.beardSprite = playerCharacterManager.characterSheet.characterBeard.name;
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

        _playerData.bodyLevel = playerCharacterManager.characterSheet.abilities.body;
        _playerData.handsLevel = playerCharacterManager.characterSheet.abilities.hands;
        _playerData.mindLevel = playerCharacterManager.characterSheet.abilities.mind;
        _playerData.heartLevel = playerCharacterManager.characterSheet.abilities.heart;

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
        if (playerCharacterManager.equippedWeapon != null) { _playerData.equippedWeapon = playerCharacterManager.equippedWeapon.GetUniqueID();} else { _playerData.equippedWeapon = ""; };
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

        if(playerMagic.GetEquippedSpell(1) != null) { _playerData.equippedSpell1 = playerCharacterManager.GetPlayerMagic().GetEquippedSpell(1).uniqueID; } else { _playerData.equippedSpell1 = ""; };
        if (playerMagic.GetEquippedSpell(2) != null) { _playerData.equippedSpell2 = playerCharacterManager.GetPlayerMagic().GetEquippedSpell(2).uniqueID; } else { _playerData.equippedSpell2 = ""; };

        if (playerMagic.GetLearnedSpell(1) != null) { _playerData.learnedSpell1 = playerCharacterManager.GetPlayerMagic().GetLearnedSpell(1).uniqueID; } else { _playerData.learnedSpell1 = ""; };
        if (playerMagic.GetLearnedSpell(2) != null) { _playerData.learnedSpell2 = playerCharacterManager.GetPlayerMagic().GetLearnedSpell(2).uniqueID; } else { _playerData.learnedSpell2 = ""; };

        _playerData.currentEffects = playerCharacterManager.currentEffects;

        _playerData.quests.Clear();
        foreach (Quest quest in playerCharacterManager._playerQuests)
        {
            _playerData.quests.Add(quest.uniqueID);
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
                    if (characterData.characterName == characterManager.characterSheet.characterName)
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

        if(targetTracker == null)
        {
            Debug.Log("Invalid tracked character");
            return;
        }

        //The different stats to save//
        targetTracker.characterName = characterManager.characterSheet.characterName;

        targetTracker.characterPosition = characterManager.transform.position;

        targetTracker.healthCurrent = characterManager.healthCurrent;
        targetTracker.staminaCurrent = characterManager.staminaCurrent;

        targetTracker.currentInventory = characterManager.currentInventory;

        targetTracker.alreadyRunDialogueTopics = characterManager.alreadyRunDialogueTopics;

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
        playerCharacterManager.characterSheet.characterName = _playerData.playerName;
        playerCharacterManager.characterSheet.characterPronouns = (CharacterPronouns)_playerData.pronounInt;

        ColorUtility.TryParseHtmlString("#" + _playerData.colorHair, out Color hair);

        playerCharacterManager.characterSheet.characterHairColor = hair;

        if (_playerData.hairSprite != "") { playerCharacterManager.characterSheet.characterHair = spriteDatabase.GetHairFromName(_playerData.hairSprite); }
        else { playerCharacterManager.characterSheet.characterHair = null; }
        if(_playerData.beardSprite != "") { playerCharacterManager.characterSheet.characterBeard = spriteDatabase.GetBeardFromName(_playerData.beardSprite); }
        else { playerCharacterManager.characterSheet.characterBeard = null; }


        //Player manager
        playerCharacterManager.healthCurrent = _playerData.healthCurrent;
        playerCharacterManager.staminaCurrent = _playerData.staminaCurrent;

        playerCharacterManager.characterSheet.abilities.body = _playerData.bodyLevel;
        playerCharacterManager.characterSheet.abilities.hands = _playerData.handsLevel;
        playerCharacterManager.characterSheet.abilities.mind = _playerData.mindLevel;
        playerCharacterManager.characterSheet.abilities.heart = _playerData.heartLevel;

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

        playerCharacterManager.characterSheet.skills.Clear();
        foreach (string uniqueID in this._playerData.currentSkills)
        {
            Skill skill = Instantiate(scriptableObjectDatabase.GetSkillFromID(uniqueID));
            playerCharacterManager.characterSheet.skills.Add(skill);
        }

        foreach (Effect effect in this._playerData.currentEffects)
        {
            playerCharacterManager.AddEffect(effect);
        }

        playerCharacterManager._playerQuests.Clear();
        foreach (string uniqueID in this._playerData.quests)
        {
            Quest quest = Instantiate(scriptableObjectDatabase.GetQuestFromID(uniqueID));
            playerCharacterManager._playerQuests.Add(quest);
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
        List<CharacterManager> sceneCharacters = activeCharacters;

        foreach (CharacterManager character in sceneCharacters)
        {
            if (character.gameObject.scene.name != trackedSceneName)
            {
                break;
            }

            foreach (CharacterDataTracker characterDataTracker in sceneData.characterDataTrackers)
            {
                if (character.characterSheet.characterName == characterDataTracker.characterName)
                {
                    character.transform.position = characterDataTracker.characterPosition;

                    character.healthCurrent = characterDataTracker.healthCurrent;
                    character.staminaCurrent = characterDataTracker.staminaCurrent;

                    character.currentInventory = characterDataTracker.currentInventory;

                    character.alreadyRunDialogueTopics = characterDataTracker.alreadyRunDialogueTopics;

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

    private void CreateNewTrackedScene(Scene loadedScene)
    {
        SceneData newTrackedScene = new();
        newTrackedScene.trackedSceneName = loadedScene.name;

        _trackedScenes.Add(newTrackedScene);

        LoadPlayerData();
    }
}
