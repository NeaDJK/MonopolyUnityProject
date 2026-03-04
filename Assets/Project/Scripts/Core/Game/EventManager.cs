using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static readonly UnityEvent<string> OnGameEvent = new UnityEvent<string>(); 
    public static readonly UnityEvent<int> OnBalanceChanged = new UnityEvent<int>();
    
    public static void UpdateGameEvent(string message)
    {
        OnGameEvent?.Invoke(message);
    }

    public static void UpdatePlayerBalance(int money)
    {
        OnBalanceChanged?.Invoke(money);
    }
}
