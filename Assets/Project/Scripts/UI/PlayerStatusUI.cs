using UnityEngine;
using TMPro;
using System.Linq;

public class PlayerStatusUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text playerNameText;
    public TMP_Text playerBalanceText;
    public TMP_Text ownedPropertiesText;
    public TMP_Text ownedTransportsText;

    private void OnEnable()
    {
        MonopolyGameManager.OnDiceRolled += UpdateStatus;
        MonopolyGameManager.OnPropertyChanged += UpdateStatus;
    }

    private void OnDisable()
    {
        MonopolyGameManager.OnDiceRolled -= UpdateStatus;
        MonopolyGameManager.OnPropertyChanged -= UpdateStatus;
    }

    public void UpdateStatus()
    {
        Player player = MonopolyGameManager.Instance.GetCurrentPlayer();

        playerNameText.text = $"Игрок: {player.playerName}";
        playerBalanceText.text = $"Баланс: ${player.money}";
        UpdateOwnedAssets(player);
    }

    private void UpdateOwnedAssets(Player player)
    {
        ownedPropertiesText.text = "Недвижимость:\n" +
            (player.ownedProperties.Count > 0
                ? string.Join("\n", player.ownedProperties.Select(p => p.cellName))
                : "Нет");

        ownedTransportsText.text = "Транспорт:\n" +
            (player.ownedTransports.Count > 0
                ? string.Join("\n", player.ownedTransports.Select(t => t.cellName))
                : "Нет");
    }
}
