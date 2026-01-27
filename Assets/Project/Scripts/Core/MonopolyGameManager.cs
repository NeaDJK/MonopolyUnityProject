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

    //private int currentPlayerIndex = 0;

    private PropertyCell currentProperty = new PropertyCell();
    private TransportCell currentTransport;
    private PlayerMover _playerMover;
    private CameraSwitch _cameraSwitch;
    [SerializeField] private MainInterface _mainInterface;

    [Header("Credit")]
    [SerializeField] private Credit _credit;
    [SerializeField] private Credit _creditPlan_1;
    [SerializeField] private Credit _creditPlan_2;
    [SerializeField] private Credit _creditPlan_3;     
    [SerializeField] private TMP_Text _creditPlansText_1;
    [SerializeField] private TMP_Text _creditPlansText_2;
    [SerializeField] private TMP_Text _creditPlansText_3;
    [SerializeField] private Button _creditPlansButton_1;
    [SerializeField] private Button _creditPlansButton_2;
    [SerializeField] private Button _creditPlansButton_3;
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

            //_cameraSwitch = CameraSwitch.Instance;
        }
        else
        {
            Destroy(gameObject);
        }

        // if (_playerMover == null)
        // {
        //     _playerMover = PlayerMover.Instance;
        // }

        // if (_cameraSwitch == null)
        // {
        //     _cameraSwitch = CameraSwitch.Instance;
        // }
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
        // if (_playerMover == null)
        // {
        //     _playerMover = PlayerMover.Instance;
        // }

        // if (_cameraSwitch == null)
        // {
        //     _cameraSwitch = CameraSwitch.Instance;
        // }

        // if (_mainInterface == null)
        // {
        //     _mainInterface = MainInterface.Instance;
        // }

        _creditPlansButton_1.gameObject.SetActive(false);
        _creditPlansButton_2.gameObject.SetActive(false);
        _creditPlansButton_3.gameObject.SetActive(false);

        _creditPlansText_1.gameObject.SetActive(false);
        _creditPlansText_2.gameObject.SetActive(false);
        _creditPlansText_3.gameObject.SetActive(false);

        _creditInfoText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //if (_playerMover == null)
            //{
            //    Debug.LogWarning("PlayerMover is not assigned!");
            //    return;
            //}

            if (_cameraSwitch._currentCamera == _cameraSwitch._mainViewCam)
            {
                Player player = players[_playerMover.currentPlayerIndex];                

                if (!player.isHaveCredit)
                {
                    _creditPlansButton_1.gameObject.SetActive(true);
                    _creditPlansButton_2.gameObject.SetActive(true);
                    _creditPlansButton_3.gameObject.SetActive(true);

                    _creditPlansText_1.gameObject.SetActive(true);
                    _creditPlansText_2.gameObject.SetActive(true);
                    _creditPlansText_3.gameObject.SetActive(true);

                    _creditPlan_1.NewCredit();
                    _creditPlan_2.NewCredit();
                    _creditPlan_3.NewCredit();

                    _creditPlansText_1.text = _creditPlan_1.GetCreditInfo_ToString();                       
                    _creditPlansText_2.text = _creditPlan_2.GetCreditInfo_ToString();
                    _creditPlansText_3.text = _creditPlan_3.GetCreditInfo_ToString();
                }  
            
                else
                {
                    _creditPlansButton_1.gameObject.SetActive(false);
                    _creditPlansButton_2.gameObject.SetActive(false);
                    _creditPlansButton_3.gameObject.SetActive(false);

                    _creditPlansText_1.gameObject.SetActive(false);
                    _creditPlansText_2.gameObject.SetActive(false);
                    _creditPlansText_3.gameObject.SetActive(false);

                    _creditInfoText.gameObject.SetActive(true);

                    _creditInfoText.text = $"У игрока {player.playerName} есть активный кредит: \n{player.activeCredit.GetCreditInfo_ToString()}";              
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

            //if (currentTransport != null)
            //{
                
            //}
        }

        if (Input.GetKeyDown(KeyCode.S) && _cameraSwitch._currentCamera != _cameraSwitch._panelMenuCam)
        {
            _playerMover.EndTurn();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (_cameraSwitch != null)
            {
                _cameraSwitch.OpenPanelMenu();
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
    }

    public void TakeCredit_1()
    {
        Player player = GetCurrentPlayer();
        
        // Добавляем кредит игроку
        player.activeCredit = _creditPlan_1;
        
        // Начисляем деньги игроку
        player.AddMoney(_creditPlan_1.GetSum());
        player.isHaveCredit = true;
        
        _creditPlansButton_1.gameObject.SetActive(false);
        _creditPlansButton_2.gameObject.SetActive(false);
        _creditPlansButton_3.gameObject.SetActive(false);

        _creditPlansText_1.gameObject.SetActive(false);
        _creditPlansText_2.gameObject.SetActive(false);
        _creditPlansText_3.gameObject.SetActive(false);

        _creditInfoText.gameObject.SetActive(true);

        _creditInfoText.text = $"Игрок {player.playerName} взял кредит: \n\n{player.activeCredit.GetCreditInfo_ToString()}";
    }

    public void TakeCredit_2()
    {
        Player player = GetCurrentPlayer();
        
        // Добавляем кредит игроку
        player.activeCredit = _creditPlan_2;
        
        // Начисляем деньги игроку
        player.AddMoney(_creditPlan_2.GetSum());
        player.isHaveCredit = true;
        
        _creditPlansButton_1.gameObject.SetActive(false);
        _creditPlansButton_2.gameObject.SetActive(false);
        _creditPlansButton_3.gameObject.SetActive(false);

        _creditPlansText_1.gameObject.SetActive(false);
        _creditPlansText_2.gameObject.SetActive(false);
        _creditPlansText_3.gameObject.SetActive(false);

        _creditInfoText.gameObject.SetActive(true);

        _creditInfoText.text = $"Игрок {player.playerName} взял кредит: \n\n{player.activeCredit.GetCreditInfo_ToString()}";
    }

    public void TakeCredit_3()
    {
        Player player = GetCurrentPlayer();
        
        // Добавляем кредит игроку
        player.activeCredit = _creditPlan_3;
        
        // Начисляем деньги игроку
        player.AddMoney(_creditPlan_3.GetSum());
        player.isHaveCredit = true;
        
        _creditPlansButton_1.gameObject.SetActive(false);
        _creditPlansButton_2.gameObject.SetActive(false);
        _creditPlansButton_3.gameObject.SetActive(false);

        _creditPlansText_1.gameObject.SetActive(false);
        _creditPlansText_2.gameObject.SetActive(false);
        _creditPlansText_3.gameObject.SetActive(false);

        _creditInfoText.gameObject.SetActive(true);

        _creditInfoText.text = $"Игрок {player.playerName} взял кредит: \n\n{player.activeCredit.GetCreditInfo_ToString()}";
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

        if (cells[cellIndex].TryGetComponent(out PropertyCell property))
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
}