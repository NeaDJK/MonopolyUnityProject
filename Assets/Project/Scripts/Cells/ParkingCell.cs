using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingCell : MonopolyCell
{
    public override void OnPlayerLand(Player player)
    {
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} ����������� �� ��������.");
    }
}
