using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterBlock : Block
{
    public delegate void CharacterAdded();
    public static event CharacterAdded OnCharacterAdded;
    public delegate void CharacterRemoved();
    public static event CharacterRemoved OnCharacterRemoved;

    public delegate void CharacterRanOutOfMoves(CharacterBlock thisBlock);
    public event CharacterRanOutOfMoves OnCharacterRanOutOfMoves;
    public delegate void NextMoveRequired(CharacterBlock thisBlock);
    public event NextMoveRequired OnNextMoveRequired;

    public delegate void PositionUpdated(CharacterBlock thisBlock, Cell targetCell);
    public event PositionUpdated OnPositionUpdated;
    

    public int movesPerTurn;
    public int maxHealth;
    public int attackDamage;

    public int curHealth { get { return _curHealth; }}
    public int curMovesLeft { get { return _curMovesLeft; } }


    [SerializeField] private bool displayGizmos;

    private int _curHealth;
    private int _curMovesLeft;
    private GameManager _gameManager;
    private BlockMovementController _movementController;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _movementController = FindObjectOfType<BlockMovementController>();
        OnPositionUpdated += _gameManager.CallCharacterChangedPosition;
        OnCharacterRanOutOfMoves += _gameManager.CallCharacterRanOutOfMoves;
        OnNextMoveRequired += _gameManager.CallNextMoveRequired;

        // May need to reimplement (use when importing level design)
        forwardDirection = GridDirection.Forward;

    }

    private void OnDestroy()
    {
        OnPositionUpdated -= _gameManager.CallCharacterChangedPosition;
        OnCharacterRanOutOfMoves -= _gameManager.CallCharacterRanOutOfMoves;
        OnNextMoveRequired -= _gameManager.CallNextMoveRequired;


    }

    private void OnEnable()
    {
        if(OnCharacterAdded != null)
            OnCharacterAdded();
    }

    private void OnDisable()
    {
        if(OnCharacterRemoved != null)
            OnCharacterRemoved();
    }

    /// <summary>
    /// English: Rotate the block horizontally with hop animation
    /// </summary>
    /// <param name="rotation"></param>
    public override void RotateHorizontally(Rotations rotation)
    {
        if (rotation != Rotations.Left && rotation != Rotations.Right) { return; }

        switch (rotation)
        {
            case Rotations.Left:
                forwardDirection = GridDirection.RotateLeft(forwardDirection);
                break;
            case Rotations.Right:
                forwardDirection = GridDirection.RotateRight(forwardDirection);
                break;
            default:
                break;
        }

        // Set up movement controller
        _movementController.InitializeMovement(transform, forwardDirection, cell, cell, BlockMovementController.MovementType.BasicHop);
        // Movement no cost
        StartCoroutine(MovementCoroutine(1));

    }

    public void MoveFoward()
    {
        // Get one forward cell
        Cell toCell = _gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);

        // Set up movement controller
        _movementController.InitializeMovement(transform, forwardDirection, cell, toCell, BlockMovementController.MovementType.BasicHop);
        OnPositionUpdated(this, toCell);
        cell = toCell;
        // Movement 1 cost
        StartCoroutine(MovementCoroutine(1));
    }

    private IEnumerator MovementCoroutine(int moveCost)
    {
        float t = 0;
        if(_movementController.movementType == BlockMovementController.MovementType.BasicHop)
        {
            Transform one = _movementController.transform.GetChild(0);
            Transform second = _movementController.transform.GetChild(1);
            Transform third = _movementController.transform.GetChild(2);

            while (true)
            {
                yield return null;
                if (t >= 1)
                {
                    t = 1;
                    MovementUtilities.QuadraticBezierLerp(transform, one, third, second, t, true);
                    break;
                }
                t += Time.deltaTime * _movementController.speed * (1 + t);
                MovementUtilities.QuadraticBezierLerp(transform, one, third, second, t, true);
            }
        }
        else if(_movementController.movementType == BlockMovementController.MovementType.Slide)
        {
            Transform one = _movementController.transform.GetChild(0);
            Transform second = _movementController.transform.GetChild(1);
            while (true)
            {
                yield return null;
                if (t >= 1)
                {
                    t = 1;
                    MovementUtilities.LinearLerp(transform, one, second, t, true);
                    break;
                }
                t += Time.deltaTime * _movementController.speed * (1 + t);
                MovementUtilities.LinearLerp(transform, one, second, t, true);
            }
        }

        // Sound Effect

        // Finish movement
        _gameManager.CallBlockEndedBehaviour(this);

        MinusMoves(moveCost);
    }

    public void AttackForward(int damageAmount)
    {
        // Get an array of cells basing on the weapons
        Cell attackCell = _gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);
        // Use those cells from the gridController to query the characterPlane to check for those that is occupied

        // Animation and Sound effect

        // Trigger the occupants' TriggerHit method.
    }

    public void TakeDamage(int damageAmount)
    {
        MinusHealth(damageAmount);
    }

    public void HealHealth(int healAmount)
    {
        PlusHealth(healAmount);
    }

    public void ResetHealth()
    {
        _curHealth = maxHealth;
    }

    public void ResetCurrentMoves()
    {
        _curMovesLeft = movesPerTurn;
    }

    public void CheckForLeftOverMoves()
    {
        if(_curMovesLeft <= 0)
        {
            if(OnCharacterRanOutOfMoves != null)
            {
                OnCharacterRanOutOfMoves(this);
            }
        }
        else
        {
            if (OnNextMoveRequired != null)
            {
                OnNextMoveRequired(this);
            }
        }

    }

    private void MinusHealth(int amount)
    {
        if (amount < 0) { return; }
        _curHealth -= amount;
        if (_curHealth < 0) { _curHealth = 0; }
    }

    private void PlusHealth(int amount)
    {
        if(amount < 0) { return; }
        _curHealth += amount;
        if(_curHealth > maxHealth) { _curHealth = maxHealth; }
    }

    private bool HealthIsZero() { return _curHealth == 0; }

    private void MinusMoves(int movesNum)
    {
        if (movesNum < 0) { return; }
        _curMovesLeft -= movesNum;
        if (_curMovesLeft < 0) { _curMovesLeft = 0; }

        CheckForLeftOverMoves();
    }

    private void PlusMoves(int movesNum)
    {
        if (movesNum < 0) { return; }
        _curMovesLeft += movesNum;
    }

    private void ResetMovesPerTurn()
    {
        _curMovesLeft = movesPerTurn;
    }

    private bool NoMoreMoves() { return _curMovesLeft == 0; }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        if(forwardDirection == null) { return; }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + forwardDirection);

    }
}