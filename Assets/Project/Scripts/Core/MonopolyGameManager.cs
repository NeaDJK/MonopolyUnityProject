using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using static MonopolyGameManager;
using System.Linq;
using Cinemachine;

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

    private int currentPlayerIndex = 0;

    private PropertyCell currentProperty = new PropertyCell();
    private TransportCell currentTransport;
    private PlayerMover _playerMover;

    private CameraSwitch _cameraSwitch = new CameraSwitch();

    private void Start()
    {
        if (_playerMover == null)
        {
            _playerMover = PlayerMover.Instance;
        }
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

            Player player = players[currentPlayerIndex];

            PlayerMover.Instance.Move();
            ProcessCell(player.currentPosition, player);

            OnPlayerMoved?.Invoke(player);
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (_playerMover == null)
            {
                Debug.LogWarning("PlayerMover is not assigned!");
                return;
            }

            PlayerMover.Instance.EndTurn();
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _cameraSwitch.OpenPanelMenu();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_cameraSwitch._isPreviousPlayerCamera)
            {
                _cameraSwitch.ClosePanelMenu(currentPlayerIndex);
            }

            if (_cameraSwitch._isPreviousMainViewCamera)
            {
                _cameraSwitch.ClosePanelMenu();
            }
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePlayers();
            isGameInitialized = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }    

    public void LogEvent(string message)
    {
        OnGameEvent?.Invoke(message);
        Debug.Log(message);
    }

    public MonopolyCell GetCurrentCell(Player player) => cells[player.currentPosition].GetComponent<MonopolyCell>();

    public Player GetCurrentPlayer() => players[currentPlayerIndex];

    private void ProcessCell(int cellIndex, Player player)
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