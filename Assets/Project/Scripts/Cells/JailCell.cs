using UnityEngine;

public class JailCell : MonopolyCell
{
    [Header("��������� ������")]
    public int bailAmount = 50; 
    public int turnsToSkip = 3; 

    public override void OnPlayerLand(MonopolyGameManager.Player player)
    {
        if (player.isInJail)
        {
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} �������� ����� �� ������");
            HandleJailedPlayer(player);
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} ������������ � ������!");
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
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} ������� �� ������");
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent($"�������� ����� � ������: {player.turnsInJail}");
        }
    }
}