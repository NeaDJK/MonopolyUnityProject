using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerMover : MonoBehaviour
{
    public static PlayerMover Instance { get; private set; }
    

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpHeight = 0.3f;
    public float rotationSpeed = 10f;
    public float rotationAngle = 90f;
    public float movementThreshold = 0.01f;
    public float minMoveTime = 0.2f;

    private int currentPlayerIndex = 0;    
    public static event Action OnDiceRolled;

    private int stepsRemaining = 0;
    private bool waitingForDiceRoll = true;

    private MonopolyGameManager _gameManager;
    private PlayerStatusUI _playerStatusUI;
    private CameraSwitch _cameraSwitch;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // ������������� ������������
            _gameManager = MonopolyGameManager.Instance;
            _cameraSwitch = CameraSwitch.Instance;
            _playerStatusUI = FindObjectOfType<PlayerStatusUI>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // �������������� ��������
        if (_gameManager == null)
        {
            _gameManager = MonopolyGameManager.Instance;
        }

        if (_playerStatusUI == null)
        {
            _playerStatusUI = FindObjectOfType<PlayerStatusUI>();
        }
    }

    public void Move() => StartCoroutine(TryRollDice(currentPlayerIndex));
    
    public IEnumerator TryRollDice(int playerIndex)
    {
        Player player = _gameManager.players[playerIndex];

        if (player.isInJail)
        {
            _gameManager.GetCurrentPlayer().HandleJailedPlayer();
            EndTurn();

            yield break;
        }

        if (waitingForDiceRoll && !AnyPlayerMoving() && _gameManager.isGameInitialized && player.countOfSteps > 0)
            RollDice();
    }

    private void RollDice()
    {
        int dice1 = UnityEngine.Random.Range(1, 7);
        int dice2 = UnityEngine.Random.Range(1, 7);
        int diceResult = dice1 + dice2;

        if (diceResult == 12)
        {
            _gameManager.players[currentPlayerIndex].countOfSteps++;
        }

        Debug.Log($"{_gameManager.players[currentPlayerIndex].playerName} rolled {dice1}+{dice2}={diceResult}");

        stepsRemaining = diceResult;
        waitingForDiceRoll = false;

        if (_gameManager.players[currentPlayerIndex].movementCoroutine != null)
            StopCoroutine(_gameManager.players[currentPlayerIndex].movementCoroutine);

        _gameManager.players[currentPlayerIndex].movementCoroutine = StartCoroutine(MovePlayerCoroutine(currentPlayerIndex));

        OnDiceRolled?.Invoke();
    }

    public void EndTurn()
    {
        _cameraSwitch.SwitchToMainViewCamera();
        waitingForDiceRoll = true;

        if (_gameManager.players[currentPlayerIndex].countOfSteps <= 0)
        {
            _gameManager.players[currentPlayerIndex].countOfSteps = Player.defaultCountOfStep;
            currentPlayerIndex = (currentPlayerIndex + 1) % _gameManager.players.Length;
        }
        
        else
        {            
            Debug.Log($"Еще ходов: {_gameManager.players[currentPlayerIndex].countOfSteps}");
        }
    }

    private Vector3 GetExactCellPosition(int cellIndex)
    {
        if (cellIndex >= 0 && cellIndex < _gameManager.cells.Length)
        {
            Vector3 pos = _gameManager.cells[cellIndex].position;
            pos.y = _gameManager.boardBaseHeight;
            return pos;
        }
        return Vector3.zero;
    }

    private void SnapToExactPosition(int playerIndex)
    {
        Player player = _gameManager.players[playerIndex];
        Vector3 exactPos = GetExactCellPosition(player.currentPosition);
        exactPos.y = _gameManager.boardBaseHeight + _gameManager.playerBaseHeight;
        exactPos += new Vector3(player.offsetPosition.x, 0, player.offsetPosition.z);
        player.piece.transform.position = exactPos;
    }


    private bool AnyPlayerMoving()
    {
        foreach (Player player in _gameManager.players)
            if (player != null && player.isMoving) return true;
        return false;
    }

    private IEnumerator MovePlayerCoroutine(int playerIndex)
    {
        Player player = _gameManager.players[playerIndex];
        player.isMoving = true;
        player.countOfSteps--;

        while (stepsRemaining > 0)
        {
            int nextPos = (player.currentPosition + 1) % _gameManager.cells.Length;
            Vector3 targetPos = GetExactCellPosition(nextPos);
            targetPos.y += _gameManager.playerBaseHeight;
            targetPos += new Vector3(player.offsetPosition.x, 0, player.offsetPosition.z);

            int previousPosition = player.currentPosition;
            player.currentPosition = (player.currentPosition + 1) % _gameManager.cells.Length;

            if (previousPosition > player.currentPosition)
            {
                player.AddMoney(StartCell.startMoney);

                _playerStatusUI.UpdateStatus();
                _gameManager.LogEvent($"{player.playerName} ������� $200 �� ������ ����� �����");
            }

            if (Array.IndexOf(_gameManager.cornerCellIndices, nextPos) >= 0)
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

        _cameraSwitch.SwitchToPlayerCamera(currentPlayerIndex);          
    }

    private IEnumerator AnimateMoveToPosition(Player player, Vector3 targetPos)
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
            newPos.y = _gameManager.boardBaseHeight + _gameManager.playerBaseHeight + height;

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
}
