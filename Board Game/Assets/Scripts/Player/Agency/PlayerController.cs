using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterBlock playerBlock;
    public Cell focusCell;
    public RevealAreaSkill revealSkill;
    public GameManager gameManager;
    public delegate void PlayerIsFinished();
    public static event PlayerIsFinished OnPlayerIsFinished;

    public enum ControlMode
    {
        Character,
        RevealSkill,
        Survey
    }

    [SerializeField]
    private bool displayGizmos;

    public ControlMode Mode { get { return _controlMode; } }
    private ControlMode _controlMode;

    public bool CanControl { get { return _canControl; } }
    private bool _canControl = false;

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

    }

    private void Update()
    {
        if (!_canControl) { return; }
        if(playerBlock == null) { return; }

        if(Input.GetKeyDown(KeyCode.W))
        {
            if(_controlMode == ControlMode.Character)
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
            else if(_controlMode == ControlMode.Survey)
            {
                FocusCellMoveInDirection(GridDirection.Forward);

            }
            else if (_controlMode == ControlMode.RevealSkill)
            {
                FocusCellMoveInDirection(GridDirection.Forward);

            }
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            if(_controlMode == ControlMode.Character)
            {
                // Skip Action
                PreventInput();
                SkipAction();
            }
            else if(_controlMode == ControlMode.Survey)
            {
                FocusCellMoveInDirection(GridDirection.Backward);

            }
            else if (_controlMode == ControlMode.RevealSkill)
            {
                FocusCellMoveInDirection(GridDirection.Backward);

            }
        }
        else if(Input.GetKeyDown(KeyCode.D))
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
        else if(Input.GetKeyDown(KeyCode.A))
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
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            if (_controlMode == ControlMode.Character)
            {
                // Attack forward
                PreventInput();
                AttackForward();
            }
            else if(_controlMode == ControlMode.Survey)
            {

            }
            else if (_controlMode == ControlMode.RevealSkill && revealSkill.IsOffCooldown)
            {
                PreventInput();
                RevealArea(focusCell);
            }

        }
        else if(Input.GetKeyDown(KeyCode.E))
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
        else if(Input.GetKeyDown(KeyCode.Q))
        {
            if (_controlMode == ControlMode.Character)
            {
                _controlMode = ControlMode.RevealSkill;
            }
            else if(_controlMode == ControlMode.RevealSkill)
            {
                _controlMode = ControlMode.Survey; 
            }
            else if (_controlMode == ControlMode.Survey)
            {
                _controlMode = ControlMode.Character;
            }
            FocusCellBackToPlayer();
        }
    }

    private void SkipAction()
    {
        playerBlock.SkipAction();
    }

    private void RotatePlayer(Block.Rotations rotation)
    {
        playerBlock.RotateHorizontally(rotation);
    }

    private void MovePlayerForward()
    {
        playerBlock.MoveFoward();
    }

    private void AttackForward()
    {
        playerBlock.Attack();
    }

    private void ActivateForward()
    {
        playerBlock.ActivateForward();
    }

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
        // Anticipate obstacles, enemies, end goal
        // May refactor to have the validation check be calculated before hand for obstacles. Keep for enemies to trigger hit and goal to clear level
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
