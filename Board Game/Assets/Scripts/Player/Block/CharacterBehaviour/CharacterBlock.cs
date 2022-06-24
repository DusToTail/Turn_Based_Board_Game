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
    public static event PositionUpdated OnPositionUpdated;

    public delegate void DamageTaken(CharacterBlock thisBlock, int damageTaken);
    public static event DamageTaken OnDamageTaken;

    public CharacterDataScriptableObject characterData;
    [HideInInspector] public int movesPerTurn;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int visionRange;
    public WeaponHandler weaponHandler;
    public AudioHandler audioHandler;
    public Animator animator;

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
        OnCharacterRanOutOfMoves += gameManager.CallCharacterRanOutOfMoves;
        OnNextMoveRequired += gameManager.CallNextMoveRequired;

        // Will need to reimplement (use when importing level design with more data about each block)
        movesPerTurn = characterData.movesPerTurn;
        maxHealth = characterData.maxHealth;
        visionRange = characterData.visionRange;
        weaponHandler.weapon.data = characterData.defaultWeapon;
        weaponHandler.weapon.InitializeWeapon();
        audioHandler.InitializeAudioSources(characterData.soundEffects);

    }

    private void OnDestroy()
    {
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
        StartCoroutine(MovementCoroutine(1, cell, cell));

    }

    public void MoveFoward()
    {
        Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);
        movementController.InitializeMovement(transform, forwardDirection, cell, toCell, BlockMovementController.MovementType.Slide);
        StartCoroutine(MovementCoroutine(1, cell, toCell));
    }

    public void ActivateForward() => StartCoroutine(ActivateForwardCoroutine());
    public void Attack() => StartCoroutine(AttackCoroutine());
    public void TakeDamage(CharacterBlock fromCharacter, int damageAmount)
        => StartCoroutine(TakeDamageCoroutine(fromCharacter, damageAmount));
    public void TakeDamage(ObjectBlock fromObject, int damageAmount)
        => StartCoroutine(TakeDamageCoroutine(fromObject, damageAmount));
    public void SkipAction() => StartCoroutine(SkipActionCoroutine());
    public void HealHealth(int healAmount) => PlusHealth(healAmount);
    public void ResetHealth() => _curHealth = maxHealth;
    public void ResetCurrentMoves() => _curMovesLeft = movesPerTurn;

    public void CallMovesSituation()
    {
        if(NoMoreMoves())
        {
            if(OnCharacterRanOutOfMoves != null)
                OnCharacterRanOutOfMoves(this);
            return;
        }

        if (OnNextMoveRequired != null)
            OnNextMoveRequired(this);
        return;
    }
    public void CallOnPositionUpdated(Cell toCell) { OnPositionUpdated(this, toCell); }

    private IEnumerator MovementCoroutine(int moveCost, Cell from, Cell to)
    {
        // Check for collision if move foward
        if(from != to)
        {
            Cell collidedBlockCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);
            ObjectBlock collidedObject = gameManager.objectPlane.GetBlockFromCell(collidedBlockCell);
            Cell belowCell = gameManager.gridController.GetCellFromCellWithDirection(collidedBlockCell, GridDirection.Down);
            ObjectBlock belowObject = gameManager.objectPlane.GetBlockFromCell(belowCell);
            if (collidedObject != null && collidedObject.activationBehaviour.GetComponent<IActivationOnCollision>() != null)
            {
                Debug.Log($"{gameObject.name} waiting for {collidedObject.name} to finish");
                collidedObject.activationBehaviour.GetComponent<IActivationOnCollision>().OnCollision(collidedObject, this);
                yield return new WaitUntil(() => collidedObject.isFinished == true);
                gameManager.CallBlockEndedBehaviour(this);
                MinusMoves(moveCost);
                yield break;
            }
            else if(belowObject != null && belowObject.activationBehaviour.GetComponent<StairBehaviour>() != null)
            {
                Debug.Log($"{gameObject.name} waiting for {belowObject.name} to finish");
                belowObject.activationBehaviour.GetComponent<StairBehaviour>().OnCollision(belowObject, this);
                yield return new WaitUntil(() => belowObject.isFinished == true);
                gameManager.CallBlockEndedBehaviour(this);
                MinusMoves(moveCost);
                yield break;
            }
            else
                Debug.Log($"There is no object at {collidedBlockCell.gridPosition} to wait");
        }

        

        float t = 0;
        Cell steppedOnCell = to;
        ObjectBlock objectBlock = gameManager.objectPlane.GetBlockFromCell(steppedOnCell);

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
                t += Time.deltaTime * movementController.speed;
                MovementUtilities.MoveQuadraticBezierLerp(transform, one, third, second, t, true);
            }
        }
        else if (movementController.movementType == BlockMovementController.MovementType.Slide)
        {
            // Initialize the trajectory to be used
            Transform one = movementController.transform.GetChild(0);
            Transform second = movementController.transform.GetChild(1);
            animator.SetTrigger("Move");
            animator.SetFloat("Speed Multiplier", movementController.speed);
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
                t += Time.deltaTime * movementController.speed;
                MovementUtilities.MoveLinearLerp(transform, one, second, t, true);
            }
            animator.SetTrigger("Idle");
        }

        // Sound Effect (NOT YET)


        // Announce to game manager that this character's is technically done
        gameManager.CallBlockEndedBehaviour(this);
        CallOnPositionUpdated(steppedOnCell);
        // Wait for any object block at the current cell
        objectBlock = gameManager.objectPlane.grid[steppedOnCell.gridPosition.y, steppedOnCell.gridPosition.z, steppedOnCell.gridPosition.x].block?.GetComponent<ObjectBlock>();
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
        GetComponentInChildren<AttackImpactEventTrigger>().isTriggered = false;
        animator.SetTrigger("Attack");
        animator.SetFloat("Speed Multiplier", movementController.speed);
        yield return new WaitUntil(() => GetComponentInChildren<AttackImpactEventTrigger>().isTriggered);
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
        if(OnDamageTaken != null)
            OnDamageTaken(this, damageAmount);
    }

    private IEnumerator TakeDamageCoroutine(ObjectBlock fromObject, int damageAmount)
    {
        yield return null;
        MinusHealth(damageAmount);
        Debug.Log($"{gameObject} took {damageAmount} damages from {fromObject.name}.");

        fromObject.activationBehaviour.GetComponent<IDamageOnActivation>().curAttackedCharacterCount++;
        if(OnDamageTaken != null)
            OnDamageTaken(this, damageAmount);
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

        CallMovesSituation();
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
