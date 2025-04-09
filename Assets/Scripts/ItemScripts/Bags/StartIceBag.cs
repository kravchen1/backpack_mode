using UnityEngine.SceneManagement;

public class StartIceBag : Bag
{
    public int countIceStack = 1;
    public override void StarActivation(Item item)
    {
        if (Enemy != null)
        {
            Enemy.menuFightIconData.AddDebuff(countIceStack, "IconFrost");
            //Debug.Log("StartIceBag inflict 1");
            //CreateLogMessage("StartIceBag inflict " + countIceStack.ToString(), Player.isPlayer);
            logManager.CreateLogMessageInflict(originalName, "frost", countIceStack, Player.isPlayer);
            Enemy.menuFightIconData.CalculateFireFrostStats();//true = Player
        }
    }
}