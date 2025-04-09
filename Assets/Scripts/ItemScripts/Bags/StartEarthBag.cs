using UnityEngine.SceneManagement;

public class StartEarthBag : Bag
{
    public int countArmorStack = 5;
    public override void StarActivation(Item item)
    {
        if (Player != null)
        {
            Player.armorMax += countArmorStack;
            Player.armor += countArmorStack;
            //CreateLogMessage("StartEarthBag give " + countArmorStack.ToString(), Player.isPlayer);
            logManager.CreateLogMessageGive(originalName, "armor", countArmorStack, Player.isPlayer);
        }
    }
}