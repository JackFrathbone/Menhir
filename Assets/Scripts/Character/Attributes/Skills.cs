[System.Serializable]
public class Skills
{
    //Physique Skills
    public int shortWeapons;
    public int longWeapons;
    public int block;
    public int healing;
    //Agility Skills
    public int rangedWeapons;
    public int dodge;
    public int stealth;
    public int trapsmith;
    //Mental Skills
    public int elementalism;
    public int soulMagic;
    public int ritualism;
    public int alchemy;
    //Social Skills
    public int persuasion;
    public int intimidation;
    public int barter;
    public int wisdom;

    public void SetSkill(string skillName, int i)
    {
        switch (skillName)
        {
            //Physique
            case "shortWeapons":
                shortWeapons = i;
                break;
            case "longWeapons":
                longWeapons = i;
                break;
            case "block":
                block = i;
                break;
            case "healing":
                healing = i;
                break;
            //Agility
            case "rangedWeapons":
                rangedWeapons = i;
                break;
            case "dodge":
                dodge = i;
                break;
            case "stealth":
                stealth = i;
                break;
            case "trapsmith":
                trapsmith = i;
                break;
            //Mental
            case "elementalism":
                elementalism = i;
                break;
            case "soulMagic":
                soulMagic = i;
                break;
            case "ritualism":
                ritualism = i;
                break;
            case "alchemy":
                alchemy = i;
                break;
            //Social
            case "persuasion":
                persuasion = i;
                break;
            case "intimidation":
                intimidation = i;
                break;
            case "barter":
                barter = i;
                break;
            case "wisdom":
                wisdom = i;
                break;
        }
    }

    public static string GetSkillName(string skillName)
    {
        switch (skillName)
        {
            //Physique
            case "shortWeapons":
                return "Short Weapons";
            case "longWeapons":
                return "Long Weapons";
            case "block":
                return "Block";
            case "healing":
                return "Healing";
            //Agility
            case "rangedWeapons":
                return "Ranged Weapons";
            case "dodge":
                return "Dodge";
            case "stealth":
                return "Stealth";
            case "trapsmith":
                return "Trapsmith";
            //Mental
            case "elementalism":
                return "Elementalism";
            case "soulMagic":
                return "Soul Magic";
            case "ritualism":
                return "Ritualism";
            case "alchemy":
                return "Alchemy";
            //Social
            case "persuasion":
                return "Persuasion";
            case "intimidation":
                return "Intimidation";
            case "barter":
                return "Barter";
            case "wisdom":
                return "Wisdom";
        }

        return null;
    }

    public static string GetSkillVariableName(string skillName)
    {
        switch (skillName)
        {
            //Physique
            case "Short Weapons":
                return "shortWeapons";
            case "Long Weapons":
                return "longWeapons";
            case "Block":
                return "block";
            case "Healing":
                return "healing";
            //Agility
            case "Ranged Weapons":
                return "rangedWeapons";
            case "Dodge":
                return "dodge";
            case "Stealth":
                return "stealth";
            case "Trapsmith":
                return "trapsmith";
            //Mental
            case "Elementalism":
                return "elementalism";
            case "Soul Magic":
                return "soulMagic";
            case "Ritualism":
                return "ritualism";
            case "Alchemy":
                return "alchemy";
            //Social
            case "Persuasion":
                return "persuasion";
            case "Intimidation":
                return "intimidation";
            case "Barter":
                return "barter";
            case "Wisdom":
                return "wisdom";
        }

        return null;
    }

    public static string GetSkillDescription(string skillName)
    {
        switch (skillName)
        {
            //Physique
            case "shortWeapons":
                return "";
            case "longWeapons":
                return "";
            case "block":
                return "";
            case "healing":
                return "";
            //Agility
            case "rangedWeapons":
                return "";
            case "dodge":
                return "";
            case "stealth":
                return "";
            case "trapsmith":
                return "";
            //Mental
            case "elementalism":
                return "";
            case "soulMagic":
                return "";
            case "ritualism":
                return "";
            case "alchemy":
                return "";
            //Social
            case "persuasion":
                return "";
            case "intimidation":
                return "";
            case "barter":
                return "";
            case "wisdom":
                return "";
        }

        return null;
    }
}
