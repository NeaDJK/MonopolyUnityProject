using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonopolyCell : MonoBehaviour
{
    public enum CellType { Property, Transport, Chance, CommunityChest, Tax, Jail, Parking, Start }

    public CellType cellType;
    public string cellName;
    public int positionOnBoard;

    public abstract void OnPlayerLand(Player player);
}
