using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : Singleton<DataManager>
{
    [Header("References")]
    [SerializeField] GameObject _itemContainerPrefab;

    //This script is a container of all tracked scenes with their associated data
    [Header("Data")]
    public List<SceneData> _trackedScenes = new();

    public PlayerDataTracker _playerData;

    public List<CharacterManager> _activeCharacters = new();

    private void Start()
    {
        keepAlive = true;
    }

    public void SaveSaveSlot(int i)
    {
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
        if(File.ReadAllText(Application.dataPath + "/Saves/save" + i.ToString() + ".txt") == null)
        {
            return;
        }

        string saveSlotPath = File.ReadAllText(Application.dataPath + "/Saves/save" + i.ToString() + ".txt");

        SaveSlot saveSlot = JsonUtility.FromJson<SaveSlot>(saveSlotPath);

        _trackedScenes = saveSlot.savedScenes;
        _playerData = saveSlot.playerData;

        SceneLoader.instance.LoadPlayerScene(_playerData.currentScene, null, _playerData.characterPosition, _playerData.characterRotation);

        Debug.Log("Loaded Game");
    }

    public void CheckLoadedScene()
    {
        Scene currentScene = SceneManager.GetSceneByBuildIndex(SceneLoader.instance.GetCurrentScene());

        if(currentScene.buildIndex == -1)
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

        if(playerObject == null)
        {
            //No player object in scene
            return;
        }

        PlayerCharacterManager playerCharacterManager = playerObject.GetComponent<PlayerCharacterManager>();

        if(playerCharacterManager == null)
        {
            Debug.Log("trying to save player data with no active player");
            return;
        }

        _playerData.currentScene = SceneLoader.instance.GetCurrentScene();
        _playerData.characterPosition = playerCharacterManager.transform.position;
        _playerData.characterRotation = playerCharacterManager.transform.rotation.eulerAngles;

        _playerData.healthCurrent = playerCharacterManager.healthCurrent;
        _playerData.staminaCurrent = playerCharacterManager.staminaCurrent;

        _playerData.abilities.body = playerCharacterManager.characterSheet.abilities.body;
        _playerData.abilities.hands = playerCharacterManager.characterSheet.abilities.hands;
        _playerData.abilities.mind = playerCharacterManager.characterSheet.abilities.mind;
        _playerData.abilities.heart = playerCharacterManager.characterSheet.abilities.heart;

        _playerData.currentInventory = new(playerCharacterManager.currentInventory);
        _playerData.currentSpells = new(playerCharacterManager.currentSpells);
        _playerData.currentSkills = new(playerCharacterManager.currentSkills);

        //Equipped
        _playerData.equippedWeapon = playerCharacterManager.equippedWeapon;
        _playerData.equippedShield = playerCharacterManager.equippedShield;

        _playerData.equippedArmour = playerCharacterManager.equippedArmour;
        _playerData.equippedCape = playerCharacterManager.equippedCape;
        _playerData.equippedFeet = playerCharacterManager.equippedFeet;
        _playerData.equippedGreaves = playerCharacterManager.equippedGreaves;
        _playerData.equippedHands = playerCharacterManager.equippedHands;
        _playerData.equippedHelmet = playerCharacterManager.equippedHelmet;
        _playerData.equippedPants = playerCharacterManager.equippedPants;
        _playerData.equippedShirt = playerCharacterManager.equippedShirt;

        _playerData.equippedSpell1 = playerCharacterManager.GetPlayerMagic().GetEquippedSpell(1);
        _playerData.equippedSpell2 = playerCharacterManager.GetPlayerMagic().GetEquippedSpell(2);

        _playerData.learnedSpell1 = playerCharacterManager.GetPlayerMagic().GetLearnedSpell(1);
        _playerData.learnedSpell2 = playerCharacterManager.GetPlayerMagic().GetLearnedSpell(2);

        _playerData.currentEffects = playerCharacterManager.currentEffects;

        _playerData.quests = playerCharacterManager._playerQuests;
        _playerData.stateChecks = playerCharacterManager.stateChecks;
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

                if(targetTracker != null)
                {
                    break;
                }

                CharacterDataTracker newCharacterDataTracker = new();
                sceneData.characterDataTrackers.Add(newCharacterDataTracker);
                targetTracker = newCharacterDataTracker;
                break;
            }
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

        LoadPlayerData(_playerData);
        LoadSceneDataCharacters(trackedSceneName, targetSceneData);
        LoadSceneDataContainers(trackedSceneName, targetSceneData);
    }

    private void LoadPlayerData(PlayerDataTracker playerData)
    {
        PlayerCharacterManager playerCharacterManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCharacterManager>();

        if (playerCharacterManager == null)
        {
            Debug.Log("trying to save player data with no active player");
            return;
        }

        playerCharacterManager.healthCurrent = _playerData.healthCurrent;
        playerCharacterManager.staminaCurrent = _playerData.staminaCurrent;

        playerCharacterManager.characterSheet.abilities.body =_playerData.abilities.body;
        playerCharacterManager.characterSheet.abilities.hands = _playerData.abilities.hands;
        playerCharacterManager.characterSheet.abilities.mind = _playerData.abilities.mind;
        playerCharacterManager.characterSheet.abilities.heart = _playerData.abilities.heart;

        playerCharacterManager.currentInventory = _playerData.currentInventory;
        playerCharacterManager.currentSpells = _playerData.currentSpells;
        playerCharacterManager.currentSkills = _playerData.currentSkills;

        //Equipped
        playerCharacterManager.EquipItem("weapon", _playerData.equippedWeapon);
        playerCharacterManager.EquipItem("shield", _playerData.equippedShield);

        playerCharacterManager.EquipItem("equipment", _playerData.equippedArmour);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedCape);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedFeet);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedGreaves);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedHands);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedHelmet);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedPants);
        playerCharacterManager.EquipItem("equipment", _playerData.equippedShirt);

        playerCharacterManager.GetPlayerMagic().PrepareSpell(_playerData.equippedSpell1);
        playerCharacterManager.GetPlayerMagic().PrepareSpell(_playerData.equippedSpell2);

        playerCharacterManager.GetPlayerMagic().LearnSpell(_playerData.learnedSpell1);
        playerCharacterManager.GetPlayerMagic().LearnSpell(_playerData.learnedSpell2);

        foreach(Effect effect in _playerData.currentEffects)
        {
            playerCharacterManager.AddEffect(effect);
        }

        playerCharacterManager._playerQuests = _playerData.quests;
        playerCharacterManager.stateChecks = _playerData.stateChecks;
        playerCharacterManager.alreadyRunDialogueTopics = _playerData.alreadyRunDialogueTopics;


        TimeController.SetTrackedTime(_playerData.currentDay, _playerData.currentHour, _playerData.currentMinute);
    }

    private void LoadSceneDataCharacters(string trackedSceneName, SceneData sceneData)
    {
        List<CharacterManager> sceneCharacters = _activeCharacters;

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
    }
}
