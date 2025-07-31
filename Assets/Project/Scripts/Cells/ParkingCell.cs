using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingCell : MonopolyCell
{
    public override void OnPlayerLand(MonopolyGameManager.Player player)
    {
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} остановился на парковке.");
    }
}
