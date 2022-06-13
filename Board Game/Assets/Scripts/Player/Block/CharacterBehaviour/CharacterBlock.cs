using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A character block can make movements, activation, interaction with objects and attacks
/// </summary>
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

    public CharacterDataScriptableObject characterData;
    [HideInInspector] public int movesPerTurn;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int visionRange;
    public WeaponHandler weaponHandler;
    public AudioHandler audioHandler;

    public int curHealth { get { return _curHealth; }}

    // To know when to send events regarding running out of moves
    public int curMovesLeft { get { return _curMovesLeft; } }

    // To know when can the attact coroutine continues
    [HideInInspector] public int attackedEntityCount;
    [HideInInspector] public int curAttackedEntityCount;


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

        // Will need to reimplement (use when importing level design with more data about each block)
        forwardDirection = GridDirection.Forward;
        movesPerTurn = characterData.movesPerTurn;
        maxHealth = characterData.maxHealth;
        visionRange = characterData.visionRange;
        weaponHandler.weapon.data = characterData.defaultWeapon;
        weaponHandler.weapon.InitializeWeapon();
        audioHandler.InitializeAudioSources(characterData.soundEffects);

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
        GameManager.OnLevelFinished += StopBehaviour;
    }

    private void OnDisable()
    {
        if(OnCharacterRemoved != null)
            OnCharacterRemoved();
        GameManager.OnLevelFinished -= StopBehaviour;
    }


    /// <summary>
    /// Rotate the block horizontally with hop animation
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
        // Movement one cost
        StartCoroutine(MovementCoroutine(1));

    }

    /// <summary>
    /// Move the block forward with hop animation
    /// </summary>
    public void MoveFoward()
    {
        // Get one forward cell
        Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);

        // Set up movement controller
        movementController.InitializeMovement(transform, forwardDirection, cell, toCell, BlockMovementController.MovementType.BasicHop);
        OnPositionUpdated(this, toCell);
        cell = toCell;
        // Movement one cost
        StartCoroutine(MovementCoroutine(1));
    }

    /// <summary>
    /// Activate the object block forward
    /// </summary>
    public void ActivateForward()
    {
        StartCoroutine(ActivateForwardCoroutine());
    }

    /// <summary>
    /// Attack using the weapon handler with the equipped weapon
    /// </summary>
    public void Attack()
    {
        StartCoroutine(AttackCoroutine());
    }

    /// <summary>
    /// Take damage from a character block
    /// </summary>
    /// <param name="fromCharacter"></param>
    /// <param name="damageAmount"></param>
    public void TakeDamage(CharacterBlock fromCharacter, int damageAmount)
    {
        StartCoroutine(TakeDamageCoroutine(fromCharacter, damageAmount));
    }

    /// <summary>
    /// Take damage from an object block
    /// </summary>
    /// <param name="fromObject"></param>
    /// <param name="damageAmount"></param>
    public void TakeDamage(ObjectBlock fromObject, int damageAmount)
    {
        StartCoroutine(TakeDamageCoroutine(fromObject, damageAmount));
    }

    /// <summary>
    /// Skip one action
    /// </summary>
    public void SkipAction()
    {
        StartCoroutine(SkipActionCoroutine());
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

    /// <summary>
    /// Check for moves and send the corresponsding event (to game manager)
    /// </summary>
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

    /// <summary>
    /// Send an event that the character's position has updated (to character plane)
    /// </summary>
    /// <param name="toCell"></param>
    public void CallOnPositionUpdated(Cell toCell)
    {
        OnPositionUpdated(this, toCell);
        cell = toCell;
    }


    private IEnumerator MovementCoroutine(int moveCost)
    {
        float t = 0;
        Cell steppedOnCell = cell;
        if (movementController.movementType == BlockMovementController.MovementType.BasicHop)
        {
            // Initialize the trajectory to be used
            Transform one = movementController.transform.GetChild(0);
            Transform second = movementController.transform.GetChild(1);
            Transform third = movementController.transform.GetChild(2);

            // Update position and rotation per frame
            while (true)
            {
                yield return null;
                if (t >= 1)
                {
                    t = 1;
                    MovementUtilities.MoveQuadraticBezierLerp(transform, one, third, second, t, true);
                    break;
                }
                t += Time.deltaTime * movementController.speed * (1 + t);
                MovementUtilities.MoveQuadraticBezierLerp(transform, one, third, second, t, true);
            }
        }
        else if (movementController.movementType == BlockMovementController.MovementType.Slide)
        {
            // Initialize the trajectory to be used
            Transform one = movementController.transform.GetChild(0);
            Transform second = movementController.transform.GetChild(1);

            // Update position and rotation per frame
            while (true)
            {
                yield return null;
                if (t >= 1)
                {
                    t = 1;
                    MovementUtilities.MoveLinearLerp(transform, one, second, t, true);
                    break;
                }
                t += Time.deltaTime * movementController.speed * (1 + t);
                MovementUtilities.MoveLinearLerp(transform, one, second, t, true);
            }
        }

        // Sound Effect (NOT YET)


        // Announce to game manager that this character's is technically done
        gameManager.CallBlockEndedBehaviour(this);

        // Wait for any object block at the current cell
        GameObject objectBlock = gameManager.objectPlane.grid[steppedOnCell.gridPosition.y, steppedOnCell.gridPosition.z, steppedOnCell.gridPosition.x].block;
        if (objectBlock != null && objectBlock.GetComponentInChildren<IActivationOnStep>() != null)
        {
            Debug.Log($"{gameObject.name} waiting for {objectBlock.name} to finish");
            yield return new WaitUntil(() => objectBlock.GetComponent<ObjectBlock>().isFinished == true);
        }
        else
        {
            Debug.Log($"There is no object at {steppedOnCell.gridPosition} to wait");
        }

        // Officially finish and minus the action points (or move cost)
        MinusMoves(moveCost);
    }


    private IEnumerator ActivateForwardCoroutine()
    {
        Cell forwardCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);

        // Wait for any object block at the current cell
        GameObject objectBlock = gameManager.objectPlane.grid[forwardCell.gridPosition.y, forwardCell.gridPosition.z, forwardCell.gridPosition.x].block;
        objectBlock.GetComponent<ObjectBlock>().ActivateOnTriggered(this);
        if (objectBlock != null)
        {
            yield return new WaitUntil(() => objectBlock.GetComponent<ObjectBlock>().isFinished == true);
        }

        MinusMoves(1);
    }

    private IEnumerator AttackCoroutine()
    {
        weaponHandler.UseWeapon(this);

        yield return new WaitUntil(() => curAttackedEntityCount >= attackedEntityCount);

        gameManager.CallBlockEndedBehaviour(this);
        MinusMoves(weaponHandler.weapon.usageCost);
    }

    private IEnumerator TakeDamageCoroutine(CharacterBlock fromCharacter, int damageAmount)
    {
        yield return null;
        MinusHealth(damageAmount);
        Debug.Log($"{gameObject} took {damageAmount} damages from {fromCharacter.name}.");

        fromCharacter.curAttackedEntityCount++;
    }

    private IEnumerator TakeDamageCoroutine(ObjectBlock fromObject, int damageAmount)
    {
        yield return null;
        MinusHealth(damageAmount);
        Debug.Log($"{gameObject} took {damageAmount} damages from {fromObject.name}.");

        fromObject.activationBehaviour.GetComponent<IDamageOnActivation>().curAttackedCharacterCount++;
    }

    private IEnumerator SkipActionCoroutine()
    {
        Cell steppedOnCell = cell;

        gameManager.CallBlockEndedBehaviour(this);

        // Wait for any object block at the current cell
        GameObject objectBlock = gameManager.objectPlane.grid[steppedOnCell.gridPosition.y, steppedOnCell.gridPosition.z, steppedOnCell.gridPosition.x].block;
        if (objectBlock != null && objectBlock.GetComponent<IActivationOnStep>() != null)
        {
            Debug.Log($"{gameObject.name} waiting for {objectBlock.name} to finish");
            yield return new WaitUntil(() => objectBlock.GetComponent<ObjectBlock>().isFinished == true);
        }
        else
        {
            Debug.Log($"There is no object at {steppedOnCell.gridPosition} to wait");
        }

        MinusMoves(1);
    }

    private void MinusHealth(int amount)
    {
        if (amount < 0) { return; }
        _curHealth -= amount;
        if (_curHealth < 0) { _curHealth = 0; }

        if(HealthIsZero())
        {
            // Trigger Death Animation
            if(this == gameManager.playerController.playerBlock)
                gameManager.CallLevelFailed();
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

    private void StopBehaviour()
    {
        StopAllCoroutines();
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        if(forwardDirection == null) { return; }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + forwardDirection);

    }
}
