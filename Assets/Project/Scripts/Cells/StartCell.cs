using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCell : MonopolyCell
{
    public static int startMoney = 200;

    public override void OnPlayerLand(Player player)
    {
        //player.AddMoney(startMoney);
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} находится на клетке старта.");
    }
}