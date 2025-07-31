using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportCell : MonopolyCell
{
    public int purchasePrice = 200;
    public int[] rentPrices = { 50, 100, 200, 400 }; 
    public MonopolyGameManager.Player owner;
    public bool isMortgaged;

    void Start()
    {
        owner = null;
    }

    public override void OnPlayerLand(MonopolyGameManager.Player player)
    {
        if (owner == null)
        {
            ShowPurchaseInfo(player);
        }
        else if (owner != player && !isMortgaged)
        {
            ChargeRent(player);
        }
    }

    private void ShowPurchaseInfo(MonopolyGameManager.Player player)
    {
        if (owner == null)
        {
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} может купить {cellName} за ${purchasePrice}");
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent("Игрок на своей клетке или клетка в залоге");
        }
    }

    private void ChargeRent(MonopolyGameManager.Player player)
    {
        int transportCount = owner.ownedTransports.Count;
        int rent = rentPrices[Mathf.Clamp(transportCount - 1, 0, rentPrices.Length - 1)];

        MonopolyGameManager.Instance.LogEvent($"{player.playerName} платит за транспорт {owner.playerName}: ${rent}");
        player.PayMoney(rent);
        owner.AddMoney(rent);
    }

    public void TryPurchase(MonopolyGameManager.Player buyer)
    {
        if (buyer.money >= purchasePrice)
        {
            buyer.PayMoney(purchasePrice);
            owner = buyer;
            buyer.ownedTransports.Add(this);
            MonopolyGameManager.Instance.LogEvent($"{buyer.playerName} купил {cellName}!");
        }
    }
}
