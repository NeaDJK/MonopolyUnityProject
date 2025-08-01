using UnityEngine;

public class JailCell : MonopolyCell
{
    [Header("Настройки тюрьмы")]
    public int bailAmount = 50; 
    public int turnsToSkip = 3; 

    public override void OnPlayerLand(Player player)
    {
        player.GoToJail();

        //if (player.isInJail)
        //{
        //    MonopolyGameManager.Instance.LogEvent($"{player.playerName} пытается выйти из тюрьмы");
        //    player.HandleJailedPlayer(player);
        //}
        //else
        //{
        //    MonopolyGameManager.Instance.LogEvent($"{player.playerName} отправляется в тюрьму!");
        //}
    }

    //private void SendPlayerToJail(Player player)
    //{
    //    player.isInJail = true;
    //    player.turnsInJail = turnsToSkip;
    //    player.currentPosition = positionOnBoard; 
    //}

    
}