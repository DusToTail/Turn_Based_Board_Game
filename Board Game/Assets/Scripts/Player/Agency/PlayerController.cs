using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterBlock playerBlock;
    public GameManager gameManager;
    public delegate void PlayerIsFinished();
    public static event PlayerIsFinished OnPlayerIsFinished;

    private bool _canControl = false;

    private void OnEnable()
    {
        GameManager.OnLevelStarted += AllowInput;
        GameManager.OnPlayerTurnStarted += AllowInput;
        GameManager.OnCharacterRanOutOfMoves += CallPlayerIsFinished;
        GameManager.OnNextMoveRequired += ContinueToMoveIfAllowed;

        CharacterPlane.OnCharacterPlaneInitialized += InitializePlayerBlock;
    }

    private void OnDisable()
    {
        GameManager.OnLevelStarted -= AllowInput;
        GameManager.OnPlayerTurnStarted -= AllowInput;
        GameManager.OnCharacterRanOutOfMoves -= CallPlayerIsFinished;
        GameManager.OnNextMoveRequired -= ContinueToMoveIfAllowed;

        CharacterPlane.OnCharacterPlaneInitialized -= InitializePlayerBlock;
    }


    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        FindObjectOfType<LightingManager>().player = playerBlock.transform;
        
    }

    private void Update()
    {
        if (!_canControl) { return; }

        if(Input.GetKeyDown(KeyCode.W))
        {
            // Move forward
            Cell toCell = gameManager.gridController.GetCellFromCellWithDirection(playerBlock.cell, playerBlock.forwardDirection);

            if(CellIsValidToMove(toCell))
            {
                MovePlayerForward();
                PreventInput();
            }
            
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            // Skip Action
            SkipAction();
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            // Rotate right
            RotatePlayer(Block.Rotations.Right);
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            // Rotate left
            RotatePlayer(Block.Rotations.Left);
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            // Attack forward
            AttackForward();
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            // Menu or Cancel something
            PreventInput();
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
        return true;
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
        AllowInput();
    }

    private void ResetStats()
    {
        playerBlock.ResetHealth();
        playerBlock.ResetCurrentMoves();
    }

    private void AllowInput()
    {
        _canControl = true;
    }

    private void PreventInput()
    {
        _canControl = false;
    }

}
