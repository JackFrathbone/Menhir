using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    [Header("References")]
    private SaveSlot _saveSlot;

    [SerializeField] GameObject _itemContainerPrefab;

    //This script is a container of all tracked scenes with their associated data
    [Header("Data")]
    public List<SceneData> _trackedScenes = new();

    private void Start()
    {
        GameManager.instance.dataManager = this;
    }

    public void SaveSaveSlot(int i)
    {
        //Save a new save slot, either creating a new instance or overwriting an existing one
    }

    public void CheckLoadedScene(Scene loadedScene)
    {
        foreach (SceneData sceneData in _trackedScenes)
        {
            if (sceneData.trackedSceneName == loadedScene.name)
            {
                LoadSceneData(sceneData.trackedSceneName);
                return;
            }
        }

        CreateNewTrackedScene(loadedScene);
    }

    //Collects all data types from a scene when it is closed
    public void SaveSceneData(int buildIndex)
    {
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

        LoadSceneDataCharacters(trackedSceneName, targetSceneData);
        LoadSceneDataContainers(trackedSceneName, targetSceneData);
    }

    private void LoadSceneDataCharacters(string trackedSceneName, SceneData sceneData)
    {
        List<CharacterManager> sceneCharacters = new(GameObject.FindObjectsOfType<CharacterManager>());

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

                SceneLoader.MoveObjectToScene(newItemContainer.gameObject);
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
