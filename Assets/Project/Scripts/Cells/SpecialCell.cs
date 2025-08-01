using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCell : MonopolyCell
{
    public enum CellCategory { Chance, CommunityChest }
    public CellCategory cellCategory;

    private List<string> chanceEvents = new List<string>
    {
        "Оплатите ремонт домов - $150",
        "Получите наследство $100",
        "Штраф за превышение скорости $15"
    };

    private List<string> communityChestEvents = new List<string>
    {
        "Банковская ошибка в вашу пользу. Получите $200",
        "Оплатите обучение $50",
        "Получите доход от инвестиций $100",
        "Выиграли в лотерею! Получите $50",
        "Оплатите больничные счета $100"
    };

    public override void OnPlayerLand(Player player)
    {
        string randomEvent = GetRandomEvent(cellCategory);
        HandleEvent(player, randomEvent);
    }

    private string GetRandomEvent(CellCategory category)
    {
        List<string> events = category == CellCategory.Chance ? chanceEvents : communityChestEvents;
        return events[Random.Range(0, events.Count)];
    }

    private void HandleEvent(Player player, string eventText)
    {
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} на клетке {cellName}: {eventText}");

        if (eventText.Contains("Получите"))
        {
            int amount = ExtractAmount(eventText);
            if (amount > 0) player.AddMoney(amount);
        }
        else if (eventText.Contains("Оплатите"))
        {
            int amount = ExtractAmount(eventText);
            if (amount > 0) player.PayMoney(amount);
        }
        else if (eventText.Contains("тюрьму"))
        {
            player.GoToJail(player);
        }
    }

    private int ExtractAmount(string text)
    {
        string[] parts = text.Split('$');
        if (parts.Length > 1 && int.TryParse(parts[1], out int amount))
        {
            return amount;
        }
        return 0;
    }
}
