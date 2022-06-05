using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterBlock playerBlock;
    public GameManager gameManager;
    public delegate void PlayerIsFinished();
    public static event PlayerIsFinished OnPlayerIsFinished;

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
            // Move forward
            Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(playerBlock.cell, playerBlock.forwardDirection);
            if(CellIsValidToMove(toCell))
            {
                PreventInput();
                MovePlayerForward();
            }
            
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            // Skip Action
            PreventInput();
            SkipAction();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            // Rotate right
            PreventInput();
            RotatePlayer(Block.Rotations.Right);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            // Rotate left
            PreventInput();
            RotatePlayer(Block.Rotations.Left);
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            // Attack forward
            PreventInput();
            AttackForward();
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            // Activate forward
            Cell forwardCell = gameManager.gridController.GetCellFromCellWithDirection(playerBlock.cell,playerBlock.forwardDirection);
            if(ObjectAtCellIsValidToActivate(forwardCell))
            {
                PreventInput();
                ActivateForward();
            }
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
        FindObjectOfType<LightingManager>().playerBlockTransform = playerBlock.transform;
        FindObjectOfType<CameraController>().playerBlockTransform = playerBlock.transform;
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

}
