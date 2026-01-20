using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainInterface : MonoBehaviour
{
    public static MainInterface Instance { get; private set; }

    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _playerBalance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void UpdatePlayerName(string playerName)
    {
        if (_playerName != null)
        {
            _playerName.text = playerName;
        }
        else
        {
            Debug.LogWarning("PlayerName Text field is not assigned in MainInterface!");
        }
    }

    public void UpdateBalance(int balance)
    {
        if (_playerBalance != null)
        {
            // Форматируем число с разделителями тысяч
            string formattedBalance = balance.ToString("N0");
            _playerBalance.text = $"${formattedBalance}";
        }
        else
        {
            Debug.LogWarning("Balance Text field is not assigned in MainInterface!");
        }
    }
}
