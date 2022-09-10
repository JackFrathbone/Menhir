[System.Serializable]
public class Abilities
{
    //Default is 3 which is a +0 to rolls
    public int body = 3;
    public int hands = 3;
    public int mind = 3;
    public int heart = 3;

    public static string GetAbilityName(string abilityName)
    {
        switch (abilityName)
        {
            case "body":
                return "Body";
            case "hands":
                return "Hands";
            case "mind":
                return "Mind";
            case "heart":
                return "Heart";
        }

        return null;
    }

    public static string GetAbilityDescription(string abilityName)
    {
        switch (abilityName)
        {
            case "body":
                return "Your brute strength and natural resistance to damage.";
            case "hands":
                return "Your speed, finesse and ability to get out of the way of danger.";
            case "mind":
                return "How much you know, your magical skills and your problem solving.";
            case "heart":
                return "How well you get on with others, how well you manipulate and convince.";
        }

        return null;
    }
}
