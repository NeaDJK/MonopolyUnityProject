using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject piece;
    public int currentPosition = 0;
    public bool isMoving = false;
    public int playerNumber;
    public Color playerColor;
    public string playerName;
    public int money = 1500;
    public bool isInJail;
    public int turnsInJail;
    public int countOfSteps = 1;
    public CinemachineVirtualCamera playerCam;

    public List<TransportCell> ownedTransports = new List<TransportCell>();
    public List<PropertyCell> ownedProperties = new List<PropertyCell>();

    [HideInInspector] public Vector3 offsetPosition;
    [HideInInspector] public Quaternion targetRotation;
    [HideInInspector] public Coroutine movementCoroutine;

    public void AddMoney(int amount) => money += amount;

    public void PayMoney(int amount)
    {
        money -= amount;
        if (money < 0) Debug.Log($"{playerName} обанкротился!");
    }

    public void GoToJail(Player player)
    {
        isInJail = true;
        turnsInJail = 3;

        //MonopolyGameManager.OnGameEvent?.Invoke($"{player.playerName} отправлен в тюрьму!");
    }
}
