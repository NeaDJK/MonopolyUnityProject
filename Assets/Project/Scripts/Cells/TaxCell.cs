using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxCell : MonopolyCell
{
    public int taxMoney = 200;

    public override void OnPlayerLand(Player player)
    {
        player.PayMoney(taxMoney);
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} платит налог в размере ${taxMoney}!");
        MainInterface.Instance.UpdateBalance(player.money);
        MonopolyGameManager.Instance.LogEvent($"Новый баланс: ${player.money}");
        MonopolyGameManager.Instance.TryToLose(player);
    }
}
