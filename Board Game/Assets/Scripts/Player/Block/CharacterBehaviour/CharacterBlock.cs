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
    public delegate void PositionUpdated(CharacterBlock thisBlock, Cell targetCell);
    public static event PositionUpdated OnPositionUpdated;
    public delegate void DamageTaken(CharacterBlock thisBlock, int damageTaken);
    public static event DamageTaken OnDamageTaken;

    public CharacterDataScriptableObject characterData;
    public WeaponHandler weaponHandler;
    public AudioHandler audioHandler;
    public Animator animator;
    public GameManager gameManager;
    public BlockMovementController movementController;
    [HideInInspector] public int movesPerTurn;
    [HideInInspector] public int maxHealth;
    [HideInInspector] public int visionRange;
    [HideInInspector] public int attackedEntityCount;
    [HideInInspector] public int curAttackedEntityCount;
    public int CurHealth { get { return _curHealth; } }
    public int CurMovesLeft { get { return _curMovesLeft; } }
    [SerializeField] private bool displayGizmos;
    private int _curHealth;
    private int _curMovesLeft;

    private void OnEnable()
    {
        GameManager.OnLevelFinished += StopBehaviour;
        if (OnCharacterAdded != null)
            OnCharacterAdded();
    }

    private void OnDisable()
    {
        GameManager.OnLevelFinished -= StopBehaviour;
        if (OnCharacterRemoved != null)
            OnCharacterRemoved();
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        movementController = FindObjectOfType<BlockMovementController>();
    }

    private void Start()
    {
        movesPerTurn = characterData.movesPerTurn;
        maxHealth = characterData.maxHealth;
        visionRange = characterData.visionRange;
        weaponHandler.weapon.data = characterData.defaultWeapon;
        // TODO: Initialize according to level design
        weaponHandler.weapon.InitializeWeapon();
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
        movementController.InitializeMovement(transform, forwardDirection, cell, cell, BlockMovementController.MovementType.BasicHop);
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
    public void TakeDamage(CharacterBlock fromCharacter, int damageAmount) => StartCoroutine(TakeDamageCoroutine(fromCharacter, damageAmount));
    public void TakeDamage(ObjectBlock fromObject, int damageAmount) => StartCoroutine(TakeDamageCoroutine(fromObject, damageAmount));
    public void SkipAction() => StartCoroutine(SkipActionCoroutine());
    public void HealHealth(int healAmount) => PlusHealth(healAmount);
    public void ResetHealth() => _curHealth = maxHealth;
    public void ResetCurrentMoves() => _curMovesLeft = movesPerTurn;
    public void CallOnPositionUpdated(Cell toCell) { OnPositionUpdated(this, toCell); }
    private IEnumerator MovementCoroutine(int moveCost, Cell from, Cell to)
    {
        // Check for collision (or stair) if move foward
        if(from != to)
        {
            Cell collidedBlockCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);
            Cell belowCell = gameManager.gridController.GetCellFromCellWithDirection(collidedBlockCell, GridDirection.Down);
            bool isFinished = false;
            yield return StartCoroutine(ExistsCollisionObjectCoroutine(collidedBlockCell, x => isFinished = x));
            if(isFinished)
            {
                MinusMoves(moveCost);
                yield break;
            }
            yield return StartCoroutine(ExistsCollisionObjectCoroutine(belowCell, x => isFinished = x));
            if (isFinished)
            {
                MinusMoves(moveCost);
                yield break;
            }
        }

        // Move character in normal cases
        float t = 0;
        Cell steppedOnCell = to;
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
                if (t > 1)
                {
                    MovementUtilities.MoveQuadraticBezierLerp(transform, one, third, second, 1, true);
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
                if (t > 1)
                {
                    MovementUtilities.MoveLinearLerp(transform, one, second, 1, true);
                    break;
                }
                t += Time.deltaTime * movementController.speed;
                MovementUtilities.MoveLinearLerp(transform, one, second, t, true);
            }
            animator.SetTrigger("Idle");
        }
        CallOnPositionUpdated(steppedOnCell);
        yield return StartCoroutine(ExistsOnStepObjectCoroutine(steppedOnCell));
        MinusMoves(moveCost);
    }
    private IEnumerator ExistsCollisionObjectCoroutine(Cell cell, System.Action<bool> callback)
    {
        ObjectBlock collidedObject = gameManager.objectPlane.GetBlockFromCell(cell);
        if (collidedObject != null && collidedObject.activationBehaviour.GetComponent<IActivationOnCollision>() != null)
        {
            //Debug.Log($"{gameObject.name} waiting for {collidedObject.name} to finish");
            collidedObject.activationBehaviour.GetComponent<IActivationOnCollision>().OnCollision(collidedObject, this);
            yield return new WaitUntil(() => collidedObject.isFinished == true);
            callback.Invoke(true);
            yield break;
        }
        //Debug.Log($"There is no object at {cell.gridPosition} to wait");
        callback.Invoke(false);
    }
    private IEnumerator ExistsOnStepObjectCoroutine(Cell cell)
    {
        ObjectBlock onStepObject = gameManager.objectPlane.GetBlockFromCell(cell);
        IActivationOnStep trigger = onStepObject?.activationBehaviour.GetComponent<IActivationOnStep>();
        if (onStepObject != null && trigger != null)
        {
            trigger.OnStepped(onStepObject, this);
            //Debug.Log($"{gameObject.name}: Waiting for {onStepObject.name} to finish");
            yield return new WaitUntil(() => onStepObject.isFinished == true);
            yield break;
        }
        //Debug.Log($"{gameObject.name}: There is no object at {cell.gridPosition} to wait");
    }
    private IEnumerator ActivateForwardCoroutine()
    {
        Cell forwardCell = gameManager.gridController.GetCellFromCellWithDirection(cell, forwardDirection);
        GameObject objectBlock = gameManager.objectPlane.GetCellAndBlockFromCell(forwardCell).block;
        objectBlock.GetComponent<ObjectBlock>().ActivateOnTriggered(this);
        if (objectBlock != null)
            yield return new WaitUntil(() => objectBlock.GetComponent<ObjectBlock>().isFinished == true);
        MinusMoves(1);
    }
    private IEnumerator AttackCoroutine()
    {
        GetComponentInChildren<AttackImpactEventTrigger>().isTriggered = false;
        animator.SetTrigger("Attack");
        animator.SetFloat("Speed Multiplier", movementController.speed);
        yield return new WaitUntil(() => GetComponentInChildren<AttackImpactEventTrigger>().isTriggered);
        yield return new WaitUntil(() => curAttackedEntityCount >= attackedEntityCount);
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
        yield return StartCoroutine(ExistsOnStepObjectCoroutine(steppedOnCell));
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
        if (NoMoreMoves()) { return; }
        _curMovesLeft -= movesNum;
        if (_curMovesLeft < 0) { _curMovesLeft = 0; }
        GoToNextMoveOrTurn();
    }
    private void GoToNextMoveOrTurn()
    {
        if (NoMoreMoves())
        {
            gameManager.CallCharacterRanOutOfMoves(this);
            return;
        }
        gameManager.CallCharacterRequiresNextMove(this);
        return;
    }
    private bool NoMoreMoves() { return _curMovesLeft == 0; }
    private void StopBehaviour() => StopAllCoroutines();
    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        if(forwardDirection == null) { return; }
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + forwardDirection);

    }
}
