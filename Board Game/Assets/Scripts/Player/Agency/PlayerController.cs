using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player agent that controls a character block via keyboard input in different modes to give diversity in commands
/// </summary>
public class PlayerController : MonoBehaviour
{
    public CharacterBlock playerBlock;
    public Cell focusCell;
    public RevealAreaSkill revealSkill;
    public GameManager gameManager;
    public delegate void PlayerIsFinished();
    public static event PlayerIsFinished OnPlayerIsFinished;
    public delegate void ControlModeSwitched(ControlMode mode);
    public static event ControlModeSwitched OnControlModeSwitched;

    public enum ControlMode
    {
        Character, // Perform basic actions of the character
        RevealSkill, // Move the focus cell around the map to reveal an area
        Survey // Move the focus cell around the map to see the field
    }

    [SerializeField]
    private bool displayGizmos;

    public ControlMode Mode { get { return _controlMode; } }
    private ControlMode _controlMode;

    public bool CanControl { get { return _canControl; } }
    private bool _canControl = false;
    private MovesUI _movesUI;

    private void OnEnable()
    {
        GameManager.OnLevelLoadingStarted += PreventInput;
        GameManager.OnLevelStarted += AllowInput;
        GameManager.OnPlayerTurnStarted += AllowInput;
        GameManager.OnLevelFinished += PreventInput;
        GameManager.OnLevelFailed += PreventInput;
        GameManager.OnCharacterRanOutOfMoves += CallPlayerIsFinished;
        GameManager.OnNextMoveRequired += ContinueToMoveIfAllowed;

        CharacterPlane.OnCharacterPlaneInitialized += InitializePlayerBlock;
    }

    private void OnDisable()
    {
        GameManager.OnLevelLoadingStarted -= PreventInput;
        GameManager.OnLevelStarted -= AllowInput;
        GameManager.OnPlayerTurnStarted -= AllowInput;
        GameManager.OnLevelFinished -= PreventInput;
        GameManager.OnLevelFailed -= PreventInput;
        GameManager.OnCharacterRanOutOfMoves -= CallPlayerIsFinished;
        GameManager.OnNextMoveRequired -= ContinueToMoveIfAllowed;

        CharacterPlane.OnCharacterPlaneInitialized -= InitializePlayerBlock;
    }


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        _movesUI = FindObjectOfType<MovesUI>();
    }

    private void Update()
    {
        if (!_canControl) { return; }
        if(playerBlock == null) { return; }
        if (!_movesUI.isFinished) { return; }
        // Keyboard Input (If Else)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (_controlMode == ControlMode.Character)
                {
                    // Move forward
                    Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(playerBlock.cell, playerBlock.forwardDirection);
                    if (CellIsValidToMove(toCell))
                    {
                        PreventInput();
                        MovePlayerForward();
                        FocusCellMoveInDirection(playerBlock.forwardDirection);
                    }
                }
                else if (_controlMode == ControlMode.Survey)
                {
                    FocusCellMoveInDirection(GridDirection.Forward);

                }
                else if (_controlMode == ControlMode.RevealSkill)
                {
                    FocusCellMoveInDirection(GridDirection.Forward);

                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                if (_controlMode == ControlMode.Character)
                {
                    // Skip Action
                    PreventInput();
                    SkipAction();
                }
                else if (_controlMode == ControlMode.Survey)
                {
                    FocusCellMoveInDirection(GridDirection.Backward);

                }
                else if (_controlMode == ControlMode.RevealSkill)
                {
                    FocusCellMoveInDirection(GridDirection.Backward);

                }
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (_controlMode == ControlMode.Character)
                {
                    // Rotate right
                    PreventInput();
                    RotatePlayer(Block.Rotations.Right);
                }
                else if (_controlMode == ControlMode.Survey)
                {
                    FocusCellMoveInDirection(GridDirection.Right);

                }
                else if (_controlMode == ControlMode.RevealSkill)
                {
                    FocusCellMoveInDirection(GridDirection.Right);

                }
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                if (_controlMode == ControlMode.Character)
                {
                    // Rotate left
                    PreventInput();
                    RotatePlayer(Block.Rotations.Left);
                }
                else if (_controlMode == ControlMode.Survey)
                {
                    FocusCellMoveInDirection(GridDirection.Left);

                }
                else if (_controlMode == ControlMode.RevealSkill)
                {
                    FocusCellMoveInDirection(GridDirection.Left);

                }
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_controlMode == ControlMode.Character)
                {
                    // Attack forward
                    PreventInput();
                    AttackForward();
                }
                else if (_controlMode == ControlMode.Survey)
                {

                }
                else if (_controlMode == ControlMode.RevealSkill && revealSkill.IsOffCooldown)
                {
                    PreventInput();
                    RevealArea(focusCell);
                }

            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                if (_controlMode == ControlMode.Character)
                {
                    // Activate forward
                    Cell forwardCell = gameManager.gridController.GetCellFromCellWithDirection(playerBlock.cell, playerBlock.forwardDirection);
                    if (ObjectAtCellIsValidToActivate(forwardCell))
                    {
                        PreventInput();
                        ActivateForward();
                    }
                }

            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                if (_controlMode == ControlMode.Character)
                {
                    _controlMode = ControlMode.RevealSkill;
                }
                else if (_controlMode == ControlMode.RevealSkill)
                {
                    _controlMode = ControlMode.Survey;
                }
                else if (_controlMode == ControlMode.Survey)
                {
                    _controlMode = ControlMode.Character;
                }
                if(OnControlModeSwitched != null)
                    OnControlModeSwitched(_controlMode);
                FocusCellBackToPlayer();
            }
        }
        
    }

    /// <summary>
    /// Do nothing and consume 1 action point
    /// </summary>
    private void SkipAction()
    {
        playerBlock.SkipAction();
    }

    /// <summary>
    /// Rotate the controlled character block to a direction relative to it's orientation
    /// </summary>
    /// <param name="rotation"></param>
    private void RotatePlayer(Block.Rotations rotation)
    {
        playerBlock.RotateHorizontally(rotation);
    }

    /// <summary>
    /// Move the controlled character block to the cell in front of its position
    /// </summary>
    private void MovePlayerForward()
    {
        playerBlock.MoveFoward();
    }

    /// <summary>
    /// Attack the cell in front of its position
    /// </summary>
    private void AttackForward()
    {
        playerBlock.Attack();
    }

    /// <summary>
    /// Activate an object block in front of its position
    /// </summary>
    private void ActivateForward()
    {
        playerBlock.ActivateForward();
    }

    /// <summary>
    /// Start revealing an area at a cell's position
    /// </summary>
    /// <param name="atCell"></param>
    private void RevealArea(Cell atCell)
    {
        StartCoroutine(RevealAreaCoroutine(atCell));
    }

    private IEnumerator RevealAreaCoroutine(Cell atCell)
    {
        revealSkill.RevealArea(atCell);
        yield return new WaitUntil(() => revealSkill.isFinished);
        AllowInput();
    }

    private bool CellIsValidToMove(Cell cell)
    {
        if (gameManager.levelPlane.CheckIfCellIsOccupied(cell))
        {
            Debug.Log($"{cell.gridPosition} is terrain, cant move");
            return false;
        }
        else
        {
            Cell cellBelow = gameManager.gridController.GetCellFromCellWithDirection(cell, GridDirection.Down);
            if(!gameManager.levelPlane.CheckIfCellIsOccupied(cellBelow))
            {
                Debug.Log($"{cell.gridPosition} is cliff, cant move");
                return false;
            }
        }

        if (gameManager.characterPlane.CheckIfCellIsOccupied(cell))
        {
            Debug.Log($"{cell.gridPosition} is character, cant move");
            return false;
        }

        if (gameManager.objectPlane.CheckIfCellIsOccupied(cell))
        {
            ObjectBlock block = gameManager.objectPlane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block.GetComponent<ObjectBlock>();
            if(block.isPassable)
            {
                return true;
            }
            else
            {
                Debug.Log($"{cell.gridPosition} is impassible object, cant move");
                return false;
            }
            
        }

        return true;
    }

    private bool ObjectAtCellIsValidToActivate(Cell cell)
    {
        if(gameManager.objectPlane.CheckIfCellIsOccupied(cell))
        {
            Debug.Log($"There is object at {cell.gridPosition}");
            ObjectBlock block = gameManager.objectPlane.grid[cell.gridPosition.y, cell.gridPosition.z, cell.gridPosition.x].block.GetComponent<ObjectBlock>();
            if (block.activationBehaviour != null && block.activationBehaviour.GetComponent<IActivationOnTrigger>() != null)
                return true;
        }
        return false;
    }

    

    

    private void ContinueToMoveIfAllowed(CharacterBlock compareBlock)
    {
        if (compareBlock != playerBlock) { return; }
        AllowInput();
    }

    private void CallPlayerIsFinished(CharacterBlock compareBlock)
    {
        if(compareBlock != playerBlock) { return ; }
        playerBlock.ResetCurrentMoves();
        Debug.Log($"Player of {playerBlock.name} is finished with his/her moves");
        if(OnPlayerIsFinished != null)
            OnPlayerIsFinished();
    }

    private void InitializePlayerBlock(CharacterPlane plane)
    {
        playerBlock = plane.GetPlayerBlock();
        ResetStats();
        focusCell = playerBlock.cell;
        _controlMode = ControlMode.Character;
        if (OnControlModeSwitched != null)
            OnControlModeSwitched(_controlMode);
        FindObjectOfType<LightingManager>().playerBlockTransform = playerBlock.transform;
        FindObjectOfType<CameraController>().playerController = this;
    }

    private void ResetStats()
    {
        playerBlock.ResetHealth();
        playerBlock.ResetCurrentMoves();
    }

    private void AllowInput()
    {
        _canControl = true;
        Debug.Log("Allow player controller movement");
    }

    private void PreventInput()
    {
        _canControl = false;
        Debug.Log("Prevent player controller movement");

    }

    private void PreventInput(LevelDesign design)
    {
        _canControl = false;
        Debug.Log("Prevent player controller movement");

    }

    private void FocusCellMoveInDirection(GridDirection direction)
    {
        Cell nextCell = gameManager.gridController.GetCellFromCellWithDirection(focusCell, direction);
        if (gameManager.levelPlane.CheckIfCellIsOccupied(nextCell))
        {
            Debug.Log($"{nextCell.gridPosition} is terrain, move focus cell up");

            Cell cellAbove = gameManager.gridController.GetCellFromCellWithDirection(nextCell, GridDirection.Up);
            if(gameManager.levelPlane.CheckIfCellIsOccupied(cellAbove))
            {
                int curHeight = nextCell.gridPosition.y;
                while (curHeight < gameManager.gridController.gridSize.y)
                {
                    Cell cellAboveAbove = gameManager.gridController.GetCellFromCellWithDirection(cellAbove, GridDirection.Up);
                    if (!gameManager.levelPlane.CheckIfCellIsOccupied(cellAboveAbove))
                    {
                        Debug.Log($"{cellAboveAbove.gridPosition} is empty, stop focus cell at {cellAbove.gridPosition}");
                        cellAbove = cellAboveAbove;
                        break;
                    }
                    else
                    {
                        Debug.Log($"{cellAboveAbove.gridPosition} is not empty, move focus cell up");
                        cellAbove = cellAboveAbove;
                    }
                    curHeight++;
                }
                nextCell = cellAbove;
                focusCell = nextCell;
            }
            else
            {
                nextCell = cellAbove;
                focusCell = nextCell;

            }

        }
        else
        {
            Cell cellBelow = gameManager.gridController.GetCellFromCellWithDirection(nextCell, GridDirection.Down);
            if (!gameManager.levelPlane.CheckIfCellIsOccupied(cellBelow))
            {
                Debug.Log($"{nextCell.gridPosition} is cliff, move focus cell down");
                int curHeight = nextCell.gridPosition.y;
                while(curHeight > 0)
                {
                    Cell cellBelowBelow = gameManager.gridController.GetCellFromCellWithDirection(cellBelow, GridDirection.Down);
                    if(!gameManager.levelPlane.CheckIfCellIsOccupied(cellBelowBelow))
                    {
                        Debug.Log($"{cellBelowBelow.gridPosition} is empty, move focus cell down");
                        cellBelow = cellBelowBelow;
                    }
                    else
                    {
                        Debug.Log($"{cellBelowBelow.gridPosition} is not empty, stop at {cellBelow.gridPosition}");
                        break;
                    }
                    curHeight--;
                }
                nextCell = cellBelow;
                focusCell = nextCell;
            }
            else
            {
                focusCell = nextCell;

            }


        }

    }

    private void FocusCellBackToPlayer()
    {
        focusCell = playerBlock.cell;
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }
        if(focusCell == null) { return; }
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(focusCell.worldPosition, gameManager.gridController.cellSize);
    }
}
