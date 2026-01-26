using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BankCell : MonopolyCell
{
    [Header("Core")]
    [SerializeField] private MonopolyGameManager _gameManager;
    [SerializeField] private Credit _credit;

    [Header("UI")]
    [SerializeField] private GameObject _creditInfoField;
    [SerializeField] private TMP_Text _creditInfoText;

    public override void OnPlayerLand(Player player)
    {
        _gameManager.LogEvent($"Игрок {player.playerName} находится на клетке банка.");

        if (!player.isHaveCredit)
        {
            _gameManager.LogEvent("Чтобы выбрать кредитный план нажмите [K]");

            if (Input.GetKeyDown(KeyCode.K))
            {
                _creditInfoField.SetActive(true);
                _creditInfoText.text = _credit.GetCreditInfo();
            }
        }

        else 
            _gameManager.LogEvent($"У игрока {player.playerName} уже есть активный кредит!");
    }
}
