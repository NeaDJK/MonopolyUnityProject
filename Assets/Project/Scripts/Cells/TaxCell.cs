using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaxCell : MonopolyCell
{
    public int taxMoney = 200;

    public override void OnPlayerLand(Player player)
    {
        player.PayMoney(taxMoney);
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} ������� ����� � ������� ${taxMoney}!");
    }
}
