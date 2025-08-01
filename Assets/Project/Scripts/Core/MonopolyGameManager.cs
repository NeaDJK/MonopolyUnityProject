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
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 0.3f;
    public float rotationSpeed = 10f;
    public float rotationAngle = 90f;
    public float movementThreshold = 0.01f;
    public float minMoveTime = 0.2f;

    private int currentPlayerIndex = 0;
    private bool waitingForDiceRoll = true;
    private int stepsRemaining = 0;
    private bool isGameInitialized = false;

    private PropertyCell currentPurchaseCell;
    private PropertyCell currentProperty;
    private TransportCell currentTransport;

    public static event Action OnDiceRolled;
    public static event Action<Player> OnPlayerMoved;
    public static event Action OnPropertyChanged;
    public static event Action<string> OnGameEvent;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryRollDice();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            currentProperty.TryPurchase(GetCurrentPlayer());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (currentTransport != null)
            {
                currentTransport.TryPurchase(GetCurrentPlayer());
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            EndTurn();
        }
    }

    void Awake()
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

    void InitializePlayers()
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

    public void TryRollDice()
    {
        if (waitingForDiceRoll && !AnyPlayerMoving() && isGameInitialized)
            RollDice();
    }

    void RollDice()
    {
        int dice1 = UnityEngine.Random.Range(1, 7);
        int dice2 = UnityEngine.Random.Range(1, 7);
        int diceResult = dice1 + dice2;

        Debug.Log($"{players[currentPlayerIndex].playerName} rolled {dice1}+{dice2}={diceResult}");

        stepsRemaining = diceResult;
        waitingForDiceRoll = false;

        if (players[currentPlayerIndex].movementCoroutine != null)
            StopCoroutine(players[currentPlayerIndex].movementCoroutine);

        players[currentPlayerIndex].movementCoroutine = StartCoroutine(MovePlayerCoroutine(currentPlayerIndex));

        OnDiceRolled?.Invoke();
    }

    IEnumerator MovePlayerCoroutine(int playerIndex)
    {
        Player player = players[playerIndex];
        player.isMoving = true;

        if (player.isInJail)
        {
            // Обрабатываем пропуск хода
            player.turnsInJail--;
            if (player.turnsInJail <= 0)
            {
                player.isInJail = false;
                LogEvent($"{player.playerName} выходит из тюрьмы!");
            }
            else
            {
                LogEvent($"{player.playerName} пропускает ход (осталось: {player.turnsInJail})");                
            }

            // Немедленный переход к следующему игроку
            EndTurn();
            player.isMoving = false;
            yield break;
        }

        while (stepsRemaining > 0)
        {
            int nextPos = (player.currentPosition + 1) % cells.Length;
            Vector3 targetPos = GetExactCellPosition(nextPos);
            targetPos.y += playerBaseHeight;
            targetPos += new Vector3(player.offsetPosition.x, 0, player.offsetPosition.z);

            int previousPosition = player.currentPosition;
            player.currentPosition = (player.currentPosition + 1) % cells.Length;

            if (previousPosition > player.currentPosition)
            {
                player.AddMoney(200);
                Debug.Log($"{player.playerName} получил $200 за проход через Старт");
            }

            if (System.Array.IndexOf(cornerCellIndices, nextPos) >= 0)
            {
                float newRotY = player.piece.transform.eulerAngles.y + rotationAngle;
                player.targetRotation = Quaternion.Euler(0, newRotY, 0);
            }

            float moveStartTime = Time.time;
            yield return StartCoroutine(AnimateMoveToPosition(player, targetPos));

            player.currentPosition = nextPos;
            stepsRemaining--;

            if (Time.time - moveStartTime < minMoveTime)
                yield return new WaitForSeconds(minMoveTime - (Time.time - moveStartTime));
        }

        SnapToExactPosition(playerIndex);
        player.isMoving = false;
        player.movementCoroutine = null;

        players[currentPlayerIndex].playerCam.Priority = 20;

        ProcessCell(player.currentPosition, player);
    }

    private void ProcessCell(int cellIndex, Player player)
    {
        currentProperty = null;

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

        OnPlayerMoved?.Invoke(player);
    }

    public Player GetCurrentPlayer()
    {
        return players[currentPlayerIndex];
    }

    public MonopolyCell GetCurrentCell(Player player)
    {
        return cells[player.currentPosition].GetComponent<MonopolyCell>();
    }

    IEnumerator AnimateMoveToPosition(Player player, Vector3 targetPos)
    {
        GameObject piece = player.piece;
        Vector3 startPos = piece.transform.position;
        float journeyLength = Vector3.Distance(startPos, targetPos);
        float startTime = Time.time;

        while (Vector3.Distance(piece.transform.position, targetPos) > movementThreshold)
        {
            float fraction = Mathf.Clamp01((Time.time - startTime) * moveSpeed / journeyLength);
            float height = Mathf.Sin(fraction * Mathf.PI) * jumpHeight;

            Vector3 newPos = Vector3.Lerp(startPos, targetPos, fraction);
            newPos.y = boardBaseHeight + playerBaseHeight + height;

            piece.transform.position = newPos;

            if (player.targetRotation != Quaternion.identity)
            {
                piece.transform.rotation = Quaternion.Slerp(
                    piece.transform.rotation,
                    player.targetRotation,
                    Time.deltaTime * rotationSpeed
                );

                if (Quaternion.Angle(piece.transform.rotation, player.targetRotation) < 1f)
                {
                    piece.transform.rotation = player.targetRotation;
                    player.targetRotation = Quaternion.identity;
                }
            }

            yield return null;
        }
    }

    void SnapToExactPosition(int playerIndex)
    {
        Player player = players[playerIndex];
        Vector3 exactPos = GetExactCellPosition(player.currentPosition);
        exactPos.y = boardBaseHeight + playerBaseHeight;
        exactPos += new Vector3(player.offsetPosition.x, 0, player.offsetPosition.z);
        player.piece.transform.position = exactPos;
    }

    Vector3 GetExactCellPosition(int cellIndex)
    {
        if (cellIndex >= 0 && cellIndex < cells.Length)
        {
            Vector3 pos = cells[cellIndex].position;
            pos.y = boardBaseHeight;
            return pos;
        }
        return Vector3.zero;
    }

    bool AnyPlayerMoving()
    {
        foreach (Player player in players)
            if (player != null && player.isMoving) return true;
        return false;
    }

    void EndTurn()
    {
        players[currentPlayerIndex].playerCam.Priority = 5;

        if (players[currentPlayerIndex].isInJail)
        {
            players[currentPlayerIndex].turnsInJail--;
            if (players[currentPlayerIndex].turnsInJail <= 0)
            {
                players[currentPlayerIndex].isInJail = false;
                LogEvent($"{players[currentPlayerIndex].playerName} выходит из тюрьмы");
            }
        }
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Length;
        waitingForDiceRoll = true;
    }

    public void LogEvent(string message)
    {
        OnGameEvent?.Invoke(message);
        Debug.Log(message);
    }
}