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
        "Пришёл подарок от друзей. Получите $75",
        "Штраф за превышение скорости. Оплатите $15",
        "Получили завещание дальнего родственника. Получите $100",
        "Вас обокрали на сумму $50",
        "Второе место в конкурсе красоты. Получите $50",
        "Вашему автомобилю нужен ремонт. Оплатите $115",
        "Штраф за парковку. Оплатите $35",
        "Получите дивиденды по акциям в размере $50",
        "Первое место в конкурсе знаний. Получите $75"
    };

    private List<string> communityChestEvents = new List<string>
    {
        "Банковская ошибка в вашу пользу. Получите $200",
        "Оплатите обучение $50",
        "Получите доход от инвестиций $100",
        "Выиграли в лотерею! Получите $50",
        "Оплатите больничные счета $100",
        "Возврат подоходного налога. Получите $75",
        "Получите проценты по вкладу в размере $25",
        "Оплатите взнос в клуб $75",
        "Потребовались услуги адвоката. Оплатите $150",
        "Обвал акций. Вы потеряли $200"
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
            if (amount > 0)
            { 
                player.AddMoney(amount);
                MainInterface.Instance.UpdateBalance(player.money);
            }

            MonopolyGameManager.Instance.TryToWin(player);
        }
        else if (eventText.Contains("Оплатите") || eventText.Contains("обокрали") || eventText.Contains("потеряли"))
        {
            int amount = ExtractAmount(eventText);
            if (amount > 0) 
            {
                player.PayMoney(amount);
                MainInterface.Instance.UpdateBalance(player.money);
            }

            MonopolyGameManager.Instance.TryToLose(player);
        }

        MonopolyGameManager.Instance.LogEvent($"Новый баланс: ${player.money}");
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
