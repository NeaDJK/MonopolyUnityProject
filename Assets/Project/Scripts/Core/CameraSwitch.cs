using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public static CameraSwitch Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera[] _playerCams;
    [SerializeField] private CinemachineVirtualCamera _mainViewCam;
    [SerializeField] private CinemachineVirtualCamera _panelMenuCam;

    private CinemachineVirtualCamera _currentCamera;

    [HideInInspector] public bool _isPreviousPlayerCamera;
    [HideInInspector] public bool _isPreviousMainViewCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _currentCamera = _mainViewCam;

        _isPreviousPlayerCamera = false;
        _isPreviousMainViewCamera = true;
    }

    public void SwitchToPlayerCamera(int currentPlayerIndex)
    {
        if (_currentCamera != null && _playerCams.Length > 0 && _playerCams[currentPlayerIndex] != null)
        {
            _currentCamera.gameObject.SetActive(false);
            _playerCams[currentPlayerIndex].gameObject.SetActive(true);

            _currentCamera = _playerCams[currentPlayerIndex];

            _isPreviousPlayerCamera = true;
            _isPreviousMainViewCamera = false;
        }

        else Debug.LogError("Камеры назначены неправильно!");
    }

    public void SwitchToMainViewCamera()
    {
        if (_mainViewCam != null && _currentCamera != null)
        {
            _currentCamera.gameObject.SetActive(false);
            _mainViewCam.gameObject.SetActive(true);

            _currentCamera = _mainViewCam;

            _isPreviousPlayerCamera = false;
            _isPreviousMainViewCamera = true;
        }

        else Debug.LogError("Камеры назначены неправильно!");
    }

    public void OpenPanelMenu()
    {
        if (_panelMenuCam != null && _currentCamera != null)
        {
            _currentCamera.gameObject.SetActive(false);
            _panelMenuCam.gameObject.SetActive(true);

            _currentCamera = _panelMenuCam;
        }

        else Debug.LogError("Камеры назначены неправильно!");
    }


    /// <summary>
    /// Возвращение к игроку
    /// </summary>
    public void ClosePanelMenu(int currentPlayerIndex)
    {
        if (_panelMenuCam != null && _playerCams.Length > 0 && _playerCams[currentPlayerIndex] != null && _currentCamera != null)
        {
            _panelMenuCam.gameObject.SetActive(false);
            _playerCams[currentPlayerIndex].gameObject.SetActive(true);

            _currentCamera = _playerCams[currentPlayerIndex];
        }

        else Debug.LogError("Камеры назначены неправильно!");
    }


    /// <summary>
    /// Возвращение к mainView
    /// </summary>
    public void ClosePanelMenu()
    {
        if (_panelMenuCam != null && _currentCamera != null)
        {
            _panelMenuCam.gameObject.SetActive(false);
            _mainViewCam.gameObject.SetActive(true);

            _currentCamera = _mainViewCam;
        }

        else Debug.LogError("Камеры назначены неправильно!");
    }
}
