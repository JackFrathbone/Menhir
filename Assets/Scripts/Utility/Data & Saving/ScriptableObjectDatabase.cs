using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[ExecuteInEditMode]
[System.Serializable]
//This class holds list of every scriptable object that might need to be saved, allows comparing between a saved id and the objects id
public class ScriptableObjectDatabase : ScriptableObject
{
    [Header("Items")]
    public List<Item> allItems = new();
    [Header("Spells")]
    public List<Spell> allSpells = new();
    [Header("Skills")]
    public List<Skill> allSkills = new();
    [Header("State Check")]
    public List<StateCheck> allStateChecks = new();

    public Item GetItemFromID(string uniqueID)
    {
        if (uniqueID == "" || uniqueID == null)
        {
            return null;
        }

        foreach (Item item in allItems)
        {
            if (item.uniqueID == uniqueID)
            {
                return item;
            }
        }

        Debug.Log("Item does not exist!");
        return null;
    }

    public Spell GetSpellFromID(string uniqueID)
    {
        if (uniqueID == "" || uniqueID == null)
        {
            return null;
        }

        foreach (Spell spell in allSpells)
        {
            if (spell.uniqueID == uniqueID)
            {
                return spell;
            }
        }

        Debug.Log("Spell does not exist!");
        return null;
    }

    public Skill GetSkillFromID(string uniqueID)
    {
        if (uniqueID == "" || uniqueID == null)
        {
            return null;
        }

        foreach (Skill skill in allSkills)
        {
            if (skill.uniqueID == uniqueID)
            {
                return skill;
            }
        }

        Debug.Log("Skill does not exist!");
        return null;
    }

    public StateCheck GetStateCheckFromID(string uniqueID)
    {
        if (uniqueID == "" || uniqueID == null)
        {
            return null;
        }

        foreach (StateCheck stateCheck in allStateChecks)
        {
            if (stateCheck.uniqueID == uniqueID)
            {
                return stateCheck;
            }
        }

        Debug.Log("StateCheck does not exist!");
        return null;
    }

#if UNITY_EDITOR
    [InspectorButton("RefreshLists")]
    public bool refreshLists;

    private void RefreshLists()
    {
        RefreshSpellList();
        RefreshItemList();
        RefreshSkillsList();
        RefreshStateChecks();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log("Reloaded Asset Database!");
    }

    private void RefreshItemList()
    {
        List<Item> tempList = new();

        tempList = FindAssetsByType<Item>();

        foreach (Item item in tempList)
        {

            item.uniqueID = item.name + item.GetInstanceID().ToString();
            UnityEditor.EditorUtility.SetDirty(item);
        }

        allItems = new List<Item>(tempList);
    }

    private void RefreshSpellList()
    {
        List<Spell> tempList = new();

        tempList = FindAssetsByType<Spell>();

        foreach (Spell spell in tempList)
        {
            if (spell.uniqueID == "" || spell.uniqueID == null)
            {
                spell.uniqueID = spell.name + spell.GetInstanceID().ToString();
                UnityEditor.EditorUtility.SetDirty(spell);

                //Create a spell item that holds this spells
                SpellItem spellItem = ScriptableObject.CreateInstance<SpellItem>();
                spellItem.name = "Item_" + spell.name;
                spellItem.itemName = "Tablet of " + spell.spellName;
                spellItem.itemDescription = "A carved stone tablet containing information about how to case the spell " + spell.spellName;
                spellItem.itemWeight = 1;
                spellItem.spell = spell;
                UnityEditor.AssetDatabase.CreateAsset(spellItem, "Assets/Data/Spells/Items/" + spell.name + ".asset");
            }
        }

        allSpells = new List<Spell>(tempList);
    }

    private void RefreshSkillsList()
    {
        List<Skill> tempList = new();

        tempList = FindAssetsByType<Skill>();

        foreach (Skill skill in tempList)
        {
            if (skill.uniqueID == "" || skill.uniqueID == null)
            {
                skill.uniqueID = skill.name + skill.GetInstanceID().ToString();
                UnityEditor.EditorUtility.SetDirty(skill);
            }
        }

        allSkills = new List<Skill>(tempList);
    }

    private void RefreshStateChecks()
    {
        List<StateCheck> tempList = new();

        tempList = FindAssetsByType<StateCheck>();

        foreach (StateCheck stateCheck in tempList)
        {
            if (stateCheck.uniqueID == "" || stateCheck.uniqueID == null)
            {
                stateCheck.uniqueID = stateCheck.name + stateCheck.GetInstanceID().ToString();
                UnityEditor.EditorUtility.SetDirty(stateCheck);
            }
        }

        allStateChecks = new List<StateCheck>(tempList);
    }

    public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
    {
        List<T> assets = new();
        string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset != null)
            {
                assets.Add(asset);
            }
        }
        return assets;
    }

#endif
}
