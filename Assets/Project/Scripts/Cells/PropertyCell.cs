using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PropertyCell : MonopolyCell
{
    public int purchasePrice;
    public int[] rentPrices;
    public int cellIndex;

    [HideInInspector] public Player owner;
    [HideInInspector] public int housesBuilt;
    [HideInInspector] public bool isMortgaged;
    [HideInInspector] public bool isActiveForPurchase;

    void Start()
    {
        owner = null;
    }

    public override void OnPlayerLand(Player player)
    {
        Debug.Log($"[{cellName}] Статус: {(owner == null ? "Свободна" : "Принадлежит " + owner.playerName)}");

        if (owner == null)
        {
            HandleUnownedCell(player);
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} может купить {cellName} за ${purchasePrice}");
        }
        else if (owner != player && !isMortgaged)
        {
            HandleRentPayment(player);
            int rent = CalculateRent();
            MonopolyGameManager.Instance.LogEvent($"{player.playerName} → {owner.playerName}: аренда {cellName} ${rent}");
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent("Игрок на своей клетке или клетка в залоге");
        }
    }

    private void HandleUnownedCell(Player player)
    {
        isActiveForPurchase = true;
        ShowPurchaseInfo(player);
        MonopolyGameManager.Instance.LogEvent($"Клетка {cellName} доступна для покупки");
    }

    private void HandleRentPayment(Player player)
    {
        int rent = CalculateRent();
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} платит аренду {owner.playerName}: ${rent}");
        player.PayMoney(rent);
        owner.AddMoney(rent);
    }

    private void ShowPurchaseInfo(Player player)
    {
        string info = $"[{cellName}]\nЦена: ${purchasePrice}\nАренда: ${rentPrices[0]}";
        if (rentPrices.Length > 1) info += $" (до ${rentPrices[^1]})";
        info += $"\nБаланс игрока: ${player.money}\nНажмите E для покупки";
        MonopolyGameManager.Instance.LogEvent(info);
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
        buyer.ownedProperties.Add(this);
        isActiveForPurchase = false;

        InventoryManager.Instance.AddNewCard(cellIndex);
        
        MonopolyGameManager.Instance.LogEvent($"[ПОКУПКА] {buyer.playerName} стал владельцем {cellName}");
        MonopolyGameManager.Instance.LogEvent($"Новый баланс: ${buyer.money}");
    }

    private int CalculateRent()
    {
        if (rentPrices == null || rentPrices.Length == 0)
        {
            Debug.LogError("Не настроены арендные ставки!");
            return 0;
        }
        return rentPrices[Mathf.Clamp(housesBuilt, 0, rentPrices.Length - 1)];
    }
}
