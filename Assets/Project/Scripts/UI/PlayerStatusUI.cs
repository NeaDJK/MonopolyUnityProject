using UnityEngine;
using TMPro;
using System.Linq;

public class PlayerStatusUI : MonoBehaviour
{
    public static PlayerStatusUI Instanse {get; private set;}

    [Header("UI Components")]
    public TMP_Text playerNameText;
    public TMP_Text playerBalanceText;
    public TMP_Text ownedPropertiesText;
    public TMP_Text ownedTransportsText;

    private void Awake()
    {
        if (Instanse == null)
        {
            Instanse = this;
        }
    }

    private void OnEnable()
    {
        PlayerMover.OnDiceRolled += UpdateStatus;
        MonopolyGameManager.OnPropertyChanged += UpdateStatus;
    }

    private void OnDisable()
    {
        PlayerMover.OnDiceRolled -= UpdateStatus;
        MonopolyGameManager.OnPropertyChanged -= UpdateStatus;
    }

    public void UpdateStatus()
    {
        Player player = MonopolyGameManager.Instance.GetCurrentPlayer();

        playerNameText.text = $"{player.playerName}";
        playerBalanceText.text = $"{player.money}$";
        UpdateOwnedAssets(player);
    }

    private void UpdateOwnedAssets(Player player)
    {
        ownedPropertiesText.text = "" + (player.ownedProperties.Count > 0 ? string.Join("\n", player.ownedProperties.Select(p => p.cellName)) : "Нет");
        ownedTransportsText.text = "" + (player.ownedTransports.Count > 0 ? string.Join("\n", player.ownedTransports.Select(p => p.cellName)) : "Нет");

        // ownedTransportsText.text = "Транспорт:\n" +
        //     (player.ownedTransports.Count > 0
        //         ? string.Join("\n", player.ownedTransports.Select(t => t.cellName))
        //         : "Нет");
    }
}
