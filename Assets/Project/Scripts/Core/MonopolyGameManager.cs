using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using static MonopolyGameManager;
using System.Linq;
using Cinemachine;
using TMPro;
using UnityEngine.UI;

public class MonopolyGameManager : MonoBehaviour
{
    public static MonopolyGameManager Instance { get; private set; }

    [Header("Board Settings")]
    public Transform[] cells;
    public int[] cornerCellIndices;
    public float positionOffset = 0.3f;
    public float cellSize = 1.5f;
    public float boardBaseHeight = 0f;

    [Header("Player Settings")]
    public Player[] players;
    public float playerBaseHeight = 0.5f;        
    
    public bool isGameInitialized = false;

    public static event Action<Player> OnPlayerMoved;
    public static event Action OnPropertyChanged;
    public static event Action<string> OnGameEvent;

    private PropertyCell currentProperty = new PropertyCell();
    private TransportCell currentTransport;
    private PlayerMover _playerMover;
    private CameraSwitch _cameraSwitch;

    [Header("Inteface")]
    [SerializeField] private MainInterface _mainInterface;
    [SerializeField] private GameObject _playerNameUI;
    [SerializeField] private Image _playerAvatar;
    [SerializeField] private GameObject _playerBalanceUI;
    [SerializeField] private GameObject _controlGuide;
    [SerializeField] private PlayerStatusUI _playerStatusUI;

    [Header("Credit")]
    [SerializeField] private Credit _credit;
    [SerializeField] private Credit _creditPlan;   
    [SerializeField] private TMP_Text _creditPlanText;
    [SerializeField] private Button _creditPlanButton;
    [SerializeField] private TMP_Text _creditInfoText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayers();
            _mainInterface.UpdateBalance(players[0].money);
            _mainInterface.UpdatePlayerName(players[0].playerName);
            isGameInitialized = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (_playerMover == null)
        {
            _playerMover = PlayerMover.Instance;
        }

        if (_cameraSwitch == null)
        {
            _cameraSwitch = CameraSwitch.Instance;
        }

        if (_mainInterface == null)
        {
            _mainInterface = MainInterface.Instance;
        }

        _playerAvatar.sprite = players[_playerMover.currentPlayerIndex].avatar;
        _playerStatusUI.UpdateStatus();

        _creditPlanButton.gameObject.SetActive(false);
        _creditPlanText.gameObject.SetActive(false);
        _creditInfoText.text = "Здесь можно взять кредит.";
    }

    private void Update()
    { 

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_cameraSwitch._currentCamera == _cameraSwitch._mainViewCam)
            {
                Player player = players[_playerMover.currentPlayerIndex]; 
                
                // Всегда показываем план кредита, если у игрока нет кредита
                if (!player.isHaveCredit)
                {
                    _creditPlanButton.gameObject.SetActive(true);
                    _creditPlanText.gameObject.SetActive(true);
                    _creditInfoText.text = "";
                    
                    _creditPlan.NewCredit();
                    _creditPlanText.text = _creditPlan.GetCreditInfo_ToString();   
                }  
                else
                {
                    // Если есть кредит, показываем информацию о нем
                    _creditPlanButton.gameObject.SetActive(false);
                    _creditPlanText.gameObject.SetActive(false);
                    
                    if (player.activeCredit != null)
                    {
                        _creditInfoText.text = $"У игрока {player.playerName} активный кредит: \n\n{player.activeCredit.GetCreditInfo_ToString()}";
                    }
                }

                _playerMover.Move();
                OnPlayerMoved?.Invoke(player);                
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentProperty.TryPurchase(GetCurrentPlayer());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            currentTransport.TryPurchase(GetCurrentPlayer());
        }

        if (Input.GetKeyDown(KeyCode.S) && _cameraSwitch._currentCamera != _cameraSwitch._panelMenuCam)
        {
            _playerMover.EndTurn();
            Player player = players[_playerMover.currentPlayerIndex];
            _playerAvatar.sprite = players[_playerMover.currentPlayerIndex].avatar; 
            _playerStatusUI.UpdateStatus();
            _creditInfoText.gameObject.SetActive(true);

            if (player.isHaveCredit)
            {
                _creditPlanButton.gameObject.SetActive(false);
                _creditPlanText.gameObject.SetActive(false);

                _creditInfoText.text = CreditUnfoUpdate();
            }

            else
            {
                _creditInfoText.text = "Здесь можно взять кредит.";
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_cameraSwitch != null)
            {
                _cameraSwitch.OpenPanelMenu();

                _playerNameUI.SetActive(false);
                _playerBalanceUI.SetActive(false);

                _creditPlanButton.interactable = true;
            }
            else
            {
                Debug.LogError("CameraSwitch.Instance is null!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_cameraSwitch != null)
            {
                _playerNameUI.SetActive(true);
                _playerBalanceUI.SetActive(true);
                _creditPlanButton.interactable = false;

                if (_cameraSwitch._isPreviousPlayerCamera)
                {
                    _cameraSwitch.ClosePanelMenu(_playerMover.currentPlayerIndex);
                }
                else if (_cameraSwitch._isPreviousMainViewCamera)
                {
                    _cameraSwitch.ClosePanelMenu();    
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            _controlGuide.SetActive(!_controlGuide.activeSelf);
        }   
    }

    public string CreditUnfoUpdate()
    {
        Player player = GetCurrentPlayer();
        return $"У игрока {player.playerName} активный кредит: \n\n{player.activeCredit.GetCreditInfo_ToString()}";
    }

    public void TryToWin(Player player)
    {
        if (player.money >= 15000)
        {
            LogEvent($"Игрок {player.playerName} победил!");
        }
    }

    public void TryToLose(Player player)
    {
        if (player.money < 0)
        {
            LogEvent($"Игрок {player.playerName} проиграл!");
        }
    }

    public void TakeCredit()
    {
        Player player = GetCurrentPlayer();
        
        if (player.isHaveCredit)
        {
            Debug.LogWarning($"Игрок {player.playerName} уже имеет активный кредит!");
            return;
        }
        
        // ВАЖНО: Создаем новый GameObject с компонентом Credit
        GameObject creditGO = new GameObject($"Credit_{player.playerName}");
        
        // Добавляем компонент Credit к GameObject
        Credit newCredit = creditGO.AddComponent<Credit>();
        
        // Копируем параметры из плана кредита
        // В Credit.cs поля должны быть public или иметь [SerializeField]
        newCredit._sum = _creditPlan._sum;
        newCredit._countOfSteps = _creditPlan._countOfSteps;
        newCredit._percent = _creditPlan._percent;
        newCredit._coefPercent = _creditPlan._coefPercent;
        newCredit._typeOfCredit = _creditPlan._typeOfCredit;
        newCredit._currentCircle = 0; // Начинаем с 0 выплаченных кругов
        newCredit.CalculateCurrentPayment(1); // Рассчитываем первый платеж
        
        // Присваиваем игроку
        player.activeCredit = newCredit;
        player.isHaveCredit = true;
        
        // Начисляем деньги ОДИН РАЗ при взятии кредита
        player.AddMoney(newCredit.GetSum());
        
        // Обновляем UI
        _creditPlanButton.gameObject.SetActive(false);
        _creditPlanText.gameObject.SetActive(false);
        _creditInfoText.gameObject.SetActive(true);
        _creditInfoText.text = $"Игрок {player.playerName} взял кредит: \n\n{player.activeCredit.GetCreditInfo_ToString()}";
        
        LogEvent($"{player.playerName} взял кредит на сумму {newCredit.GetSum()}");
        _playerStatusUI.UpdateStatus();
        _mainInterface.UpdateBalance(player.money);
        
        Debug.Log($"Кредит создан: activeCredit != null: {player.activeCredit != null}, isHaveCredit: {player.isHaveCredit}");
    }

    public void LogEvent(string message)
    {
        EventManager.UpdateGameEvent(message);
        Debug.Log(message);
    }

    public MonopolyCell GetCurrentCell(Player player) => cells[player.currentPosition].GetComponent<MonopolyCell>();

    public Player GetCurrentPlayer() => players[_playerMover.currentPlayerIndex];

    public void ProcessCell(int cellIndex, Player player)
    {
        currentProperty = null;
        currentTransport = null;

        if (cells[cellIndex].TryGetComponent(out TransportCell transport))
        {
            currentTransport = transport;
            transport.OnPlayerLand(player);
        }

        else if (cells[cellIndex].TryGetComponent(out PropertyCell property))
        {
            currentProperty = property;
            property.OnPlayerLand(player);
        }

        else
        {
            cells[cellIndex].GetComponent<MonopolyCell>().OnPlayerLand(player);
        }
    }

    private void InitializePlayers()
    {
        if (cells == null || cells.Length == 0)
        {
            Debug.LogError("[Monopoly] Cells array is not assigned in inspector!");
            return;
        }

        if (players == null || players.Length == 0)
        {
            Debug.LogError("[Monopoly] Players array is not configured in inspector!");
            return;
        }

        Vector3[] offsets = {
            new Vector3(-positionOffset, 0, -positionOffset),
            new Vector3(positionOffset, 0, -positionOffset),
            new Vector3(-positionOffset, 0, positionOffset),
            new Vector3(positionOffset, 0, positionOffset)
        };

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == null)
            {
                Debug.LogError($"[Monopoly] Player slot {i} is empty!");
                continue;
            }

            if (players[i].piece == null)
            {
                Debug.LogError($"[Monopoly] Player {i} has no piece assigned!");
                continue;
            }

            players[i].offsetPosition = offsets[i];
            players[i].targetRotation = Quaternion.identity;
            players[i].currentPosition = 0;
            players[i].isInJail = false;

            Vector3 spawnPos = GetExactCellPosition(0);
            spawnPos.y = boardBaseHeight + playerBaseHeight;
            spawnPos += new Vector3(players[i].offsetPosition.x, 0, players[i].offsetPosition.z);

            players[i].piece.transform.position = spawnPos;

            Renderer renderer = players[i].piece.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = players[i].playerColor;
            }
            else
            {
                Debug.LogWarning($"[Monopoly] Player {i} piece has no Renderer component");
            }

            Debug.Log($"[Monopoly] Player {players[i].playerName} initialized at position {spawnPos}");
        }
    }

    private Vector3 GetExactCellPosition(int cellIndex)
    {
        if (cellIndex >= 0 && cellIndex < cells.Length)
        {
            Vector3 pos = cells[cellIndex].position;
            pos.y = boardBaseHeight;
            return pos;
        }
        return Vector3.zero;
    }

    public void DebugPlayerCredit(Player player)
    {
        Debug.Log($"Игрок {player.playerName}: isHaveCredit={player.isHaveCredit}, activeCredit={player.activeCredit != null}");
        if (player.activeCredit != null)
        {
            Debug.Log($"Кредит: сумма={player.activeCredit._sum}, кругов={player.activeCredit._countOfSteps}");
        }
    }
}