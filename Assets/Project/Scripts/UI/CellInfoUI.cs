using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CellInfoUI : MonoBehaviour
{
    [Header("UI Components")]
    public TMP_Text cellNameText;
    public TMP_Text cellPriceText;
    public TMP_Text cellRentText;
    public TMP_Text cellOwnerText;

    private void OnEnable()
    {
        MonopolyGameManager.OnPlayerMoved += UpdateCellInfo;
    }

    private void OnDisable()
    {
        MonopolyGameManager.OnPlayerMoved -= UpdateCellInfo;
    }

    private void UpdateCellInfo(MonopolyGameManager.Player player)
    {
        MonopolyCell currentCell = MonopolyGameManager.Instance.GetCurrentCell(player);

        cellNameText.text = $"Клетка: {currentCell.cellName}";
        UpdateCellSpecificInfo(currentCell);
    }

    private void UpdateCellSpecificInfo(MonopolyCell cell)
    {
        if (cell is PropertyCell property)
        {
            cellPriceText.text = $"Цена: ${property.purchasePrice}";
            cellRentText.text = $"Аренда: ${property.rentPrices[0]} - ${property.rentPrices[^1]}";
            cellOwnerText.text = property.owner != null
                ? $"Владелец: {property.owner.playerName}"
                : "Свободно";
        }
        else if (cell is TransportCell transport)
        {
            cellPriceText.text = $"Цена: ${transport.purchasePrice}";
            cellRentText.text = $"Аренда: ${transport.rentPrices[0]} (x{transport.owner?.ownedTransports.Count ?? 0})";
            cellOwnerText.text = transport.owner != null
                ? $"Владелец: {transport.owner.playerName}"
                : "Свободно";
        }
        else
        {
            cellPriceText.text = "";
            cellRentText.text = "";
            cellOwnerText.text = "";
        }
    }
}