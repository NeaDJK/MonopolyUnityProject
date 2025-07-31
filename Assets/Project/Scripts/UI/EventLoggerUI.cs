using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class EventLoggerUI : MonoBehaviour
{
    [Header("Настройки")]
    public TMP_Text eventLogText;
    public int maxMessages = 5;

    private Queue<string> eventMessages = new Queue<string>();

    private void OnEnable()
    {
        MonopolyGameManager.OnGameEvent += AddEventMessage;
    }

    private void OnDisable()
    {
        MonopolyGameManager.OnGameEvent -= AddEventMessage;
    }

    public void AddEventMessage(string message)
    {
        eventMessages.Enqueue(message);

        if (eventMessages.Count > maxMessages)
            eventMessages.Dequeue();

        UpdateLogDisplay();
    }

    private void UpdateLogDisplay()
    {
        eventLogText.text = string.Join("\n", eventMessages);
    }
}