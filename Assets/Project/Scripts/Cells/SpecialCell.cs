using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCell : MonopolyCell
{
    public enum CellCategory { Chance, CommunityChest }
    public CellCategory cellCategory;

    private List<string> chanceEvents = new List<string>
    {
        "�������� ������ ����� - $150",
        "�������� ���������� $100",
        "����� �� ���������� �������� $15"
    };

    private List<string> communityChestEvents = new List<string>
    {
        "���������� ������ � ���� ������. �������� $200",
        "�������� �������� $50",
        "�������� ����� �� ���������� $100",
        "�������� � �������! �������� $50",
        "�������� ���������� ����� $100"
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
        MonopolyGameManager.Instance.LogEvent($"{player.playerName} �� ������ {cellName}: {eventText}");

        if (eventText.Contains("��������"))
        {
            int amount = ExtractAmount(eventText);
            if (amount > 0) player.AddMoney(amount);
        }
        else if (eventText.Contains("��������"))
        {
            int amount = ExtractAmount(eventText);
            if (amount > 0) player.PayMoney(amount);
        }
        else if (eventText.Contains("������"))
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
