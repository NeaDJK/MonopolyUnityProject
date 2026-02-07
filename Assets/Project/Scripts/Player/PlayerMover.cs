using System;
using System.Collections;
using UnityEngine;

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

    public int currentPlayerIndex = 0;
    public static event Action OnDiceRolled;

    private int stepsRemaining = 0;
    private bool waitingForDiceRoll = true;

    private MonopolyGameManager _gameManager;
    private PlayerStatusUI _playerStatusUI;
    private CameraSwitch _cameraSwitch;
    private MainInterface _mainInterface;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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
            player.countOfSteps-- ;
            _gameManager.GetCurrentPlayer().HandleJailedPlayer();
            yield break;
        }

        if (waitingForDiceRoll && !AnyPlayerMoving() && _gameManager.isGameInitialized && player.countOfSteps > 0)
            StartCoroutine(RollDice());
    }

    private IEnumerator RollDice()
    {
        int dice1 = UnityEngine.Random.Range(1, 7);
        int dice2 = UnityEngine.Random.Range(1, 7);
        int diceResult = dice1 + dice2;


        if (dice1 == dice2)
        {
            _gameManager.players[currentPlayerIndex].countOfSteps++;
            _gameManager.LogEvent($"Игроку {_gameManager.players[currentPlayerIndex].playerName} выпал дубль {diceResult}!");
        }

        else
        {
            _gameManager.LogEvent($"Игроку {_gameManager.players[currentPlayerIndex].playerName} выпало {dice1}+{dice2}={diceResult}");            
        }
        

        yield return new WaitForSeconds(1);

        stepsRemaining = diceResult;
        waitingForDiceRoll = false;

        if (_gameManager.players[currentPlayerIndex].movementCoroutine != null)
            StopCoroutine(_gameManager.players[currentPlayerIndex].movementCoroutine);

        _gameManager.players[currentPlayerIndex].movementCoroutine = StartCoroutine(MovePlayerCoroutine(currentPlayerIndex));

        OnDiceRolled?.Invoke();
        yield break;
    }

    public void EndTurn()
    {
        _cameraSwitch.SwitchToMainViewCamera();
        waitingForDiceRoll = true;

        if (_gameManager.players[currentPlayerIndex].countOfSteps <= 0)
        {
            _gameManager.players[currentPlayerIndex].countOfSteps = Player.defaultCountOfStep;
            currentPlayerIndex = (currentPlayerIndex + 1) % _gameManager.players.Length;
            MainInterface.Instance.UpdateBalance(_gameManager.players[currentPlayerIndex].money);
            MainInterface.Instance.UpdatePlayerName(_gameManager.players[currentPlayerIndex].playerName);
        }
        else
        {
            _gameManager.LogEvent($"Еще ходов: {_gameManager.players[currentPlayerIndex].countOfSteps}");
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

            // Сохраняем предыдущую позицию
            int oldPos = player.currentPosition;
            
            // Обновляем позицию
            player.currentPosition = nextPos;

            // Проверяем прохождение круга
            if (nextPos < oldPos)
            {
                // Начисляем деньги за круг
                player.AddMoney(StartCell.startMoney);
                _gameManager.LogEvent($"{player.playerName} получает $200 за проход круга!");
                MainInterface.Instance.UpdateBalance(player.money);

                _gameManager.DebugPlayerCredit(player);

                // Если есть кредит - списываем
                if (player.isHaveCredit && player.activeCredit != null)
                {
                    player.PayCredit();
                    MainInterface.Instance.UpdateBalance(player.money);
                    _gameManager.CreditUnfoUpdate();
                }

                _gameManager.TryToWin(player);

                _gameManager.DebugPlayerCredit(player);
            }

            if (Array.IndexOf(_gameManager.cornerCellIndices, nextPos) >= 0)
            {
                float newRotY = player.piece.transform.eulerAngles.y + rotationAngle;
                player.targetRotation = Quaternion.Euler(0, newRotY, 0);
            }

            float moveStartTime = Time.time;
            yield return StartCoroutine(AnimateMoveToPosition(player, targetPos));

            stepsRemaining--;

            if (Time.time - moveStartTime < minMoveTime)
                yield return new WaitForSeconds(minMoveTime - (Time.time - moveStartTime));
        }

        SnapToExactPosition(playerIndex);
        player.isMoving = false;
        player.movementCoroutine = null;

        _cameraSwitch.SwitchToPlayerCamera(playerIndex);
        _gameManager.ProcessCell(player.currentPosition, player);
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