using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject piece;
    public int currentPosition = 0;
    public bool isMoving = false;
    public int playerNumber;
    public Color playerColor;
    public string playerName;
    public Sprite avatar;
    public int money = 1500;
    public bool isInJail;
    public bool isHaveCredit;
    public int turnsInJail;
    public const int defaultCountOfStep = 1;
    public int countOfSteps = defaultCountOfStep;
    public CinemachineVirtualCamera playerCam;
    public List<TransportCell> ownedTransports = new List<TransportCell>();
    public List<PropertyCell> ownedProperties = new List<PropertyCell>();

    [HideInInspector] public Vector3 offsetPosition;
    [HideInInspector] public Quaternion targetRotation;
    [HideInInspector] public Coroutine movementCoroutine;
    [HideInInspector] public Credit activeCredit;

    public void AddMoney(int amount) => money += amount;

    public void PayMoney(int amount)
    {
        money -= amount;
        if (money < 0) 
            Debug.Log($"{playerName} заплатил {amount}!");
    }

    public void GoToJail()
    {
        isInJail = true;
        turnsInJail = 3;

        MonopolyGameManager.Instance.LogEvent($"{playerName} отправляется в тюрьму!");
    }

    public void HandleJailedPlayer()
    {
        turnsInJail--;

        if (turnsInJail <= 0)
        {
            isInJail = false;
            MonopolyGameManager.Instance.LogEvent($"{playerName} выходит из тюрьмы.");
        }
        else
        {
            MonopolyGameManager.Instance.LogEvent($"Осталось ходов в тюрьме: {turnsInJail}");
        }
    }

    public void PayCredit()
    {
        if (activeCredit == null || !isHaveCredit) 
        {
            isHaveCredit = false;
            return;
        }

        // Получаем платеж для следующего круга
        int payment = activeCredit.GetNextPayment();
        
        if (payment <= 0)
        {
            // Кредит выплачен - уничтожаем GameObject
            if (activeCredit.gameObject != null)
                Destroy(activeCredit.gameObject);
            
            activeCredit = null;
            isHaveCredit = false;
            MonopolyGameManager.Instance.LogEvent($"{playerName} полностью выплатил кредит!");
            return;
        }

        // СПИСЫВАЕМ деньги
        PayMoney(payment);
        
        // Увеличиваем счетчик выплаченных кругов
        activeCredit.AddCurrentCircle(1);
        
        MonopolyGameManager.Instance.LogEvent($"{playerName} выплатил {payment} по кредиту. Осталось кругов: {activeCredit.GetCountOfSteps() - activeCredit.GetCurrentCircle()}");

        // Проверяем, не выплачен ли кредит полностью
        if (activeCredit.GetCurrentCircle() >= activeCredit.GetCountOfSteps())
        {
            // Уничтожаем GameObject кредита
            if (activeCredit.gameObject != null)
                Destroy(activeCredit.gameObject);
            
            activeCredit = null;
            isHaveCredit = false;
            MonopolyGameManager.Instance.LogEvent($"{playerName} полностью выплатил кредит!");
        }
    }
}