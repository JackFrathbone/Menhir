using System.Collections.Generic;
using UnityEngine;

public enum CharacterState { alive, wounded, dead };

public class CharacterManager : MonoBehaviour
{
    //The character sheet reference
    [SerializeField] protected CharacterSheet _baseCharacterSheet;
    [HideInInspector]
    public CharacterSheet characterSheet = null;

    //Current status//
    [HideInInspector]
    public float healthCurrent;
    [HideInInspector]
    public float staminaCurrent;
    [HideInInspector]
    public float magicPercentCurrent;
    [HideInInspector]
    public float healthTotal;
    [HideInInspector]
    public float staminaTotal;
    [HideInInspector]
    public int defenceArmourTotal;

    //Equipped Items
    [HideInInspector]
    public Item equippedWeapon;
    [HideInInspector]
    public ShieldItem equippedShield;
    [HideInInspector]
    public EquipmentItem equippedArmour;
    [HideInInspector]
    public EquipmentItem equippedCape;
    [HideInInspector]
    public EquipmentItem equippedFeet;
    [HideInInspector]
    public EquipmentItem equippedGreaves;
    [HideInInspector]
    public EquipmentItem equippedHands;
    [HideInInspector]
    public EquipmentItem equippedHelmet;
    [HideInInspector]
    public EquipmentItem equippedPants;
    [HideInInspector]
    public EquipmentItem equippedShirt;

    //Inventory
    [HideInInspector]
    public List<Item> currentInventory = new List<Item>();

    //Dialogue
    [HideInInspector]
    public List<Dialogue> alreadyRunDialogue = new List<Dialogue>();

    //Active Effects//
    //Perks list (bonuses from arms and armour)

    //CharacterStates
    [HideInInspector]
    public bool isHidden;
    [HideInInspector]
    public CharacterState characterState;


    protected virtual void Awake()
    {
        characterSheet = Instantiate(_baseCharacterSheet);
        currentInventory = new List<Item>(characterSheet.characterInventory);
    }

    protected virtual void Start()
    {
        SetCurrentStatus();
    }

    protected virtual void Update()
    {
        RegenStamina();
    }

    protected virtual void RegenStamina()
    {
        if (staminaCurrent >= staminaTotal)
        {
            staminaCurrent = staminaTotal;
            return;
        }
        else
        {
            staminaCurrent += StatFormulas.StaminaRegenRate(characterSheet.abilities.hands) * Time.deltaTime;
            return;
        }
    }

    protected virtual void SetCurrentStatus()
    {
        healthTotal = StatFormulas.TotalCharacterHealth(characterSheet.abilities.body);
        healthCurrent = healthTotal;

        staminaTotal = StatFormulas.TotalCharacterStamina(characterSheet.abilities.hands);
        staminaCurrent = staminaTotal;

        magicPercentCurrent = 0;
    }

    public virtual bool CheckHostility(Faction targetFaction)
    {
        if (characterSheet.characterAggression == Aggression.Hostile)
        {
            return true;
        }

        return false;
    }

    public virtual void TriggerBlock()
    {
        
    }

    public virtual int GetTotalDefence()
    {
       int weaponDefence = 0;
       int shieldDefence = 0;

        if (equippedWeapon != null)
        {
            if(equippedWeapon is WeaponMeleeItem)
            {
                weaponDefence = (equippedWeapon as WeaponMeleeItem).weaponDefence;
            }
            else
            {
                weaponDefence = 0;
            }
        }

        GetEquipmentDefence();

        if(equippedShield != null)
        {
            shieldDefence = equippedShield.shieldDefence;
        }

        return StatFormulas.GetTotalDefence(weaponDefence, defenceArmourTotal, shieldDefence, _baseCharacterSheet.abilities.hands, 0);
    }

    public virtual void DamageHealth(float i)
    {
        if (characterState == CharacterState.alive)
        {
            healthCurrent -= i;

            if (healthCurrent <= 0)
            {
                //Check if the character is wounded or dead
                if (StatFormulas.ToWound(healthCurrent))
                {
                    characterState = CharacterState.wounded;
                }
                else
                {
                    characterState = CharacterState.dead;
                }
            }
        }
        else if (characterState == CharacterState.wounded)
        {
            characterState = CharacterState.dead;
        }
    }

    public virtual void DamageStamina(float i)
    {
        if (characterState == CharacterState.alive)
        {
            staminaCurrent -= i;

            if (staminaCurrent <= 0)
            {
                //Do something at zero stamina
            }
        }
    }

    public virtual void GetEquipmentDefence()
    {
        defenceArmourTotal = 0;

        if(equippedArmour != null)
        {
            defenceArmourTotal += equippedArmour.equipmentDefence;
        }
        if (equippedCape != null)
        {
            defenceArmourTotal += equippedCape.equipmentDefence;
        }
        if (equippedFeet != null)
        {
            defenceArmourTotal += equippedFeet.equipmentDefence;
        }
        if (equippedGreaves != null)
        {
            defenceArmourTotal += equippedGreaves.equipmentDefence;
        }
        if (equippedHands != null)
        {
            defenceArmourTotal += equippedHands.equipmentDefence;
        }
        if (equippedHelmet != null)
        {
            defenceArmourTotal += equippedHelmet.equipmentDefence;
        }
        if (equippedPants != null)
        {
            defenceArmourTotal += equippedPants.equipmentDefence;
        }
        if (equippedShirt != null)
        {
            defenceArmourTotal += equippedShirt.equipmentDefence;
        }
    }

    public virtual void AddItem(Item i)
    {
        currentInventory.Add(i);
    }

    public virtual void RemoveItem(Item i)
    {
        currentInventory.Remove(i);
    }
}
