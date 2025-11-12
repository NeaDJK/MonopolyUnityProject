using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    public static readonly UnityEvent<string> OnGameEvent = new UnityEvent<string>(); 
    public static readonly UnityEvent On = new UnityEvent(); 
    public static readonly UnityEvent OnPropertyChanged = new UnityEvent();
    public static readonly UnityEvent OnTransportBuy = new UnityEvent(); 
    
    public static void UpdateGameEvent(string message)
    {
        OnGameEvent?.Invoke(message);
    }
}
