using UnityEngine.SceneManagement;

public class StartFireBag : Bag
{
    public int countBurnStack = 1;
    public override void StarActivation(Item item)
    {
        if (Player != null)
        {
            Player.menuFightIconData.AddBuff(countBurnStack, "IconBurn");
            //Debug.Log("����� ���� �������� 1 ���");
            //CreateLogMessage("FireBag give " + countBurnStack.ToString() + " burn");
            Player.menuFightIconData.CalculateFireFrostStats();//true = Player
        }
    }
}