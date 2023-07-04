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
            "body" => "Your brute strength and natural resistance to damage. Determines your chance to hit enemies with melee weapons and increases your total health.",
            "hands" => "Your speed, finesse and ability to get out of the way of danger. Increases your chance to hit with ranged weapons and increases your total stamina.",
            "mind" => "How much you know, your magical skills and your ability to resist magic. Allows you to cast more powerful ritual spells, use more powerful magical items, and increase your chance to resist magic.",
            "heart" => "How well you get on with others, how well you manipulate and convince. Allows you to pass harder dialogue checks and access better trades. Increases the chance that characters will be wounded instead of killed.",
            _ => null,
        };
    }
}
