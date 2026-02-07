using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportCell : MonopolyCell
{
    public int purchasePrice = 200;
    public int rentPrice; 
    public Player owner;
    public bool isMortgaged;
    public bool isActiveForPurchase;
    

    void Start()
    {
        owner = null;
    }

    public override void OnPlayerLand(Player player)
    {

        if (owner == null)
        {
            //Debug.Log($"[{cellName}] Статус: {(owner == null ? "Свободна" : "Принадлежит " + owner.playerName)}");
            HandleUnownedCell();
            //MonopolyGameManager.Instance.LogEvent($"{player.playerName} может купить {cellName} за ${purchasePrice}");
        }
        else if (owner != player && !isMortgaged)
        {
            HandleRentPayment(player);
            //MonopolyGameManager.Instance.LogEvent($"{player.playerName} → {owner.playerName}: аренда {cellName} ${rent}");
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent("Игрок на своей клетке.");
        }
    }

    private void HandleUnownedCell()
    {
        isActiveForPurchase = true;
        //ShowPurchaseInfo(player);
        MonopolyGameManager.Instance.LogEvent($"[{cellName}] Статус: {(owner == null ? $"Можно купить за ${purchasePrice}" : "Принадлежит " + owner.playerName)}");
    }

        private void HandleRentPayment(Player player)
    {
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} платит аренду {owner.playerName}: ${rentPrice}");
        player.PayMoney(rentPrice);
        owner.AddMoney(rentPrice);
        MainInterface.Instance.UpdateBalance(player.money);
        MonopolyGameManager.Instance.TryToLose(player);
        MonopolyGameManager.Instance.TryToWin(owner);
    }

    public void TryPurchase(Player buyer)
    {
        if (isActiveForPurchase && owner == null)
        {
            if (buyer.money >= purchasePrice)
            {
                CompletePurchase(buyer);
            }
            else
            {
                MonopolyGameManager.Instance.LogEvent($"{buyer.playerName}: Недостаточно средств! Нужно ${purchasePrice}");
            }
        }
    }

    private void CompletePurchase(Player buyer)
    {
        buyer.PayMoney(purchasePrice);
        owner = buyer;
        buyer.ownedTransports.Add(this);
        isActiveForPurchase = false;

        //InventoryManager.Instance.AddNewCard(cellIndex);
        
        MonopolyGameManager.Instance.LogEvent($"{buyer.playerName} стал владельцем {cellName}");
        MonopolyGameManager.Instance.LogEvent($"Новый баланс: ${buyer.money}");
        PlayerStatusUI.Instanse.UpdateStatus();
        MainInterface.Instance.UpdateBalance(buyer.money);
    }
}
