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
    public int visionRange;
    public WeaponHandler weaponHandler;

    public int curHealth { get { return _curHealth; }}
    public int curMovesLeft { get { return _curMovesLeft; } }

    [HideInInspector] public int attackedCharacterCount;
    [HideInInspector] public int curAttackedCharacterCount;


    public GameManager gameManager;
    public BlockMovementController movementController;

    [SerializeField] private bool displayGizmos;

    private int _curHealth;
    private int _curMovesLeft;
    
    

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        movementController = FindObjectOfType<BlockMovementController>();
        OnPositionUpdated += gameManager.CallCharacterChangedPosition;
        OnCharacterRanOutOfMoves += gameManager.CallCharacterRanOutOfMoves;
        OnNextMoveRequired += gameManager.CallNextMoveRequired;

        // May need to reimplement (use when importing level design)
        forwardDirection = GridDirection.Forward;


    }

    private void OnDestroy()
    {
        OnPositionUpdated -= gameManager.CallCharacterChangedPosition;
        OnCharacterRanOutOfMoves -= gameManager.CallCharacterRanOutOfMoves;
        OnNextMoveRequired -= gameManager.CallNextMoveRequired;


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
        movementController.InitializeMovement(transform, forwardDirection, cell, cell, BlockMovementController.MovementType.BasicHop);
        // Movement no cost
        StartCoroutine(MovementCoroutine(1));

    }

    public void MoveFoward()
    {
        // Get one forward cell
        Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);

        // Set up movement controller
        movementController.InitializeMovement(transform, forwardDirection, cell, toCell, BlockMovementController.MovementType.BasicHop);
        OnPositionUpdated(this, toCell);
        cell = toCell;
        // Movement 1 cost
        StartCoroutine(MovementCoroutine(1));
    }

    private IEnumerator MovementCoroutine(int moveCost)
    {
        float t = 0;
        Cell steppedOnCell = cell;
        if(movementController.movementType == BlockMovementController.MovementType.BasicHop)
        {
            Transform one = movementController.transform.GetChild(0);
            Transform second = movementController.transform.GetChild(1);
            Transform third = movementController.transform.GetChild(2);

            while (true)
            {
                yield return null;
                if (t >= 1)
                {
                    t = 1;
                    MovementUtilities.QuadraticBezierLerp(transform, one, third, second, t, true);
                    break;
                }
                t += Time.deltaTime * movementController.speed * (1 + t);
                MovementUtilities.QuadraticBezierLerp(transform, one, third, second, t, true);
            }
        }
        else if(movementController.movementType == BlockMovementController.MovementType.Slide)
        {
            Transform one = movementController.transform.GetChild(0);
            Transform second = movementController.transform.GetChild(1);
            while (true)
            {
                yield return null;
                if (t >= 1)
                {
                    t = 1;
                    MovementUtilities.LinearLerp(transform, one, second, t, true);
                    break;
                }
                t += Time.deltaTime * movementController.speed * (1 + t);
                MovementUtilities.LinearLerp(transform, one, second, t, true);
            }
        }

        // Sound Effect


        // Finish movement
        gameManager.CallBlockEndedBehaviour(this);

        // Wait for any object block at the current cell
        GameObject objectBlock = gameManager.objectPlane.grid[steppedOnCell.gridPosition.y, steppedOnCell.gridPosition.z, steppedOnCell.gridPosition.x].block;
        if(objectBlock != null)
        {
            yield return new WaitUntil(() => objectBlock.GetComponent<ObjectBlock>().isFinished == true);
        }

        MinusMoves(moveCost);

    }

    public void SkipAction()
    {
        StartCoroutine(SkipActionCoroutine());
    }

    private IEnumerator SkipActionCoroutine()
    {
        Cell steppedOnCell = cell;

        gameManager.CallBlockEndedBehaviour(this);

        // Wait for any object block at the current cell
        GameObject objectBlock = gameManager.objectPlane.grid[steppedOnCell.gridPosition.y, steppedOnCell.gridPosition.z, steppedOnCell.gridPosition.x].block;
        if (objectBlock != null)
        {
            yield return new WaitUntil(() => objectBlock.GetComponent<ObjectBlock>().isFinished == true);
        }

        MinusMoves(1);
    }


    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    private IEnumerator AttackCoroutine()
    {
        weaponHandler.UseWeapon(this);

        yield return new WaitUntil(() => curAttackedCharacterCount >= attackedCharacterCount);


        // Finish movement
        gameManager.CallBlockEndedBehaviour(this);
        MinusMoves(weaponHandler.weapon.usageCost);
    }

    public void TakeDamage(CharacterBlock fromCharacter, int damageAmount)
    {
        StartCoroutine(TakeDamageCoroutine(fromCharacter, damageAmount));
    }

    private IEnumerator TakeDamageCoroutine(CharacterBlock fromCharacter, int damageAmount)
    {
        yield return null;
        MinusHealth(damageAmount);
        Debug.Log($"{gameObject} took {damageAmount} damages.");

        fromCharacter.curAttackedCharacterCount++;
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
        if(NoMoreMoves())
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

    public void CallOnPositionUpdated(Cell toCell)
    {
        OnPositionUpdated(this, toCell);
        cell = toCell;
    }

    private void MinusHealth(int amount)
    {
        if (amount < 0) { return; }
        _curHealth -= amount;
        if (_curHealth < 0) { _curHealth = 0; }

        if(HealthIsZero())
        {
            // Trigger Death Animation

            Destroy(gameObject);
        }
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
