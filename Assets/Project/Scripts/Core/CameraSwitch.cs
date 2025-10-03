using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera[] _cameras;
    private int _currentCameraIndex = 0;
    private Stack<int> _cameraHistory = new Stack<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Активация первой камеры и отключение всех остальных
        for (int i = 0; i < _cameras.Length; i++)
        {
            _cameras[i].gameObject.SetActive(i == 0);
        }

        _cameraHistory.Push(0);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            OpenScreenPanels();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseScreenPanels();
        }
    }

    /// <summary>
    /// Переключение на камеру с заданным инедексом
    /// </summary>
    public void SwitchToCamera(int index)
    {
        if (index >= 0 && index < _cameras.Length && index != _currentCameraIndex)
        {
            _cameraHistory.Push(_currentCameraIndex);

            _cameras[_currentCameraIndex].gameObject.SetActive(false);
            _cameras[index].gameObject.SetActive(true);
            _currentCameraIndex = index;
        }
    }

    /// <summary>
    /// Переключение на следующую по индексу камеру
    /// </summary>
    private void NextCamera()
    {
        int nextIndex = (_currentCameraIndex + 1) % _cameras.Length;
        SwitchToCamera(nextIndex);
    }

    /// <summary>
    /// Открытие меню панелей
    /// </summary>
    private void OpenScreenPanels() 
    {
        SwitchToCamera(1);
    }

    /// <summary>
    /// Закрытие меню панелей
    /// </summary>
    public void CloseScreenPanels()
    {
        if (_cameraHistory.Count > 1 && _cameraHistory.Pop() != 1) // Проверяем, что есть куда возвращаться
        {
            // Извлекаем предыдущую камеру из истории
            int previousIndex = _cameraHistory.Pop();

            SwitchToCamera(previousIndex);
        }
    }
}
