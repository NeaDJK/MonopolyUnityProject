using UnityEngine;

public class JailCell : MonopolyCell
{
    [Header("��������� ������")]
    public int bailAmount = 50; 
    public int turnsToSkip = 3; 

    public override void OnPlayerLand(Player player)
    {
        player.GoToJail();

        //if (player.isInJail)
        //{
        //    MonopolyGameManager.Instance.LogEvent($"{player.playerName} �������� ����� �� ������");
        //    player.HandleJailedPlayer(player);
        //}
        //else
        //{
        //    MonopolyGameManager.Instance.LogEvent($"{player.playerName} ������������ � ������!");
        //}
    }

    //private void SendPlayerToJail(Player player)
    //{
    //    player.isInJail = true;
    //    player.turnsInJail = turnsToSkip;
    //    player.currentPosition = positionOnBoard; 
    //}

    
}