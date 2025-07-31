using UnityEngine;

public class JailCell : MonopolyCell
{
    [Header("Настройки тюрьмы")]
    public int bailAmount = 50; 
    public int turnsToSkip = 3; 

    public override void OnPlayerLand(MonopolyGameManager.Player player)
    {
        if (player.isInJail)
        {
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} пытается выйти из тюрьмы");
            HandleJailedPlayer(player);
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} отправляется в тюрьму!");
            SendPlayerToJail(player);
        }
    }

    private void SendPlayerToJail(MonopolyGameManager.Player player)
    {
        player.isInJail = true;
        player.turnsInJail = turnsToSkip;
        player.currentPosition = positionOnBoard; 
    }

    private void HandleJailedPlayer(MonopolyGameManager.Player player)
    {
        player.turnsInJail--;

        if (player.turnsInJail <= 0)
        {
            player.isInJail = false;
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} выходит из тюрьмы");
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent($"Осталось ходов в тюрьме: {player.turnsInJail}");
        }
    }
}