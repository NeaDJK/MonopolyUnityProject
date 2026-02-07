using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpInfoManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _popUpInfoText;
    [SerializeField] private GameObject _popUpInfoField;
    [Range(0, 10), SerializeField] private float messageInterval;      // Интервал между сообщениями
    [Range(1, 10), SerializeField] private float displayDuration;    // Время отображения одного сообщения

    private Queue<string> messageQueue = new Queue<string>(); // Очередь сообщений
    private Coroutine displayCoroutine;                       // Ссылка на корутину
    private bool isDisplaying = false;                        // Флаг отображения

    // Статический экземпляр для доступа из других скриптов
    public static PopUpInfoManager Instance { get; private set; }

    private void Awake()
    {
        // Реализация Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        EventManager.OnGameEvent.AddListener(AddNotification);
    }

    /// <summary>
    /// Добавить новое уведомление в очередь
    /// </summary>
    /// <param name="message">Текст уведомления</param>
    public void AddNotification(string message)
    {
        // Добавляем сообщение в очередь
        messageQueue.Enqueue(message);

        // Если в данный момент ничего не отображается, запускаем корутину
        if (!isDisplaying && displayCoroutine == null)
        {
            displayCoroutine = StartCoroutine(DisplayMessages());
        }
    }

    /// <summary>
    /// Корутина для отображения сообщений из очереди
    /// </summary>
    private IEnumerator DisplayMessages()
    {
        isDisplaying = true;

        // Пока в очереди есть сообщения
        while (messageQueue.Count > 0)
        {
            // Достаем следующее сообщение
            string currentMessage = messageQueue.Dequeue();

            // Отображаем сообщение
            ShowMessage(currentMessage);

            // Ждем указанное время отображения
            yield return new WaitForSeconds(displayDuration);

            // Скрываем сообщение
            HideMessage();

            // Если в очереди еще есть сообщения, ждем интервал
            if (messageQueue.Count > 0)
            {
                yield return new WaitForSeconds(messageInterval);
            }
        }

        // Сбрасываем флаги
        isDisplaying = false;
        displayCoroutine = null;
    }

    /// <summary>
    /// Показать сообщение на экране
    /// </summary>
    private void ShowMessage(string message)
    {
        if (_popUpInfoText != null)
        {
            _popUpInfoField.SetActive(true);
            _popUpInfoText.text = message;
        }
    }

    /// <summary>
    /// Скрыть сообщение
    /// </summary>
    private void HideMessage()
    {
        if (_popUpInfoText != null)
        {
            _popUpInfoField.SetActive(false);
            _popUpInfoText.text = "";
        }
    }

    /// <summary>
    /// Очистить очередь сообщений
    /// </summary>
    public void ClearQueue()
    {
        messageQueue.Clear();

        // Останавливаем корутину, если она запущена
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }

        HideMessage();
        isDisplaying = false;
    }

    /// <summary>
    /// Получить количество сообщений в очереди
    /// </summary>
    public int GetQueueCount()
    {
        return messageQueue.Count;
    }

    /// <summary>
    /// Установить новый интервал между сообщениями
    /// </summary>
    public void SetMessageInterval(float newInterval)
    {
        messageInterval = Mathf.Max(0.1f, newInterval); // Минимальный интервал 0.1 секунды
    }

    /// <summary>
    /// Установить новую длительность отображения
    /// </summary>
    public void SetDisplayDuration(float newDuration)
    {
        displayDuration = Mathf.Max(0.5f, newDuration); // Минимальная длительность 0.5 секунды
    }
}
