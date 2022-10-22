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
        return abilityName switch
        {
            "body" => "Body",
            "hands" => "Hands",
            "mind" => "Mind",
            "heart" => "Heart",
            _ => null,
        };
    }

    public static string GetAbilityDescription(string abilityName)
    {
        return abilityName switch
        {
            "body" => "Your brute strength and natural resistance to damage.",
            "hands" => "Your speed, finesse and ability to get out of the way of danger.",
            "mind" => "How much you know, your magical skills and your problem solving.",
            "heart" => "How well you get on with others, how well you manipulate and convince.",
            _ => null,
        };
    }
}
