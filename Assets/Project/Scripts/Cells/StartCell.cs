using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCell : MonopolyCell
{
    public int startMoney = 200;

    public override void OnPlayerLand(MonopolyGameManager.Player player)
    {
        player.AddMoney(startMoney);
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} проходит старт и получает ${startMoney}.");
    }
}