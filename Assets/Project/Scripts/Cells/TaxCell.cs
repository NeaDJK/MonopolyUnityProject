using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxCell : MonopolyCell
{
    public int taxMoney = 200;

    public override void OnPlayerLand(MonopolyGameManager.Player player)
    {
        player.PayMoney(taxMoney);
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} оплатил штраф в размере ${taxMoney}!");
    }
}
