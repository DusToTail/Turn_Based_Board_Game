using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterBlock playerBlock;

    public delegate void PlayerIsFinished();
    public static event PlayerIsFinished OnPlayerIsFinished;

    private bool _canControl = false;
    private GameManager _gameManager;

    private void Start()
    {
        CharacterPlane.OnCharacterPlaneInitialized += InitializePlayerBlock;
        _gameManager = FindObjectOfType<GameManager>();

        _gameManager.OnPlayerTurnStarted += AllowInput;
        _gameManager.OnLevelStarted += AllowInput;
        _gameManager.OnCharacterRanOutOfMoves += CallPlayerIsFinished;
        _gameManager.OnNextMoveRequired += ContinueToMoveIfAllowed;
    }

    private void OnDestroy()
    {
        CharacterPlane.OnCharacterPlaneInitialized -= InitializePlayerBlock;

        _gameManager.OnPlayerTurnStarted -= AllowInput;
        _gameManager.OnLevelStarted -= AllowInput;
        _gameManager.OnCharacterRanOutOfMoves -= CallPlayerIsFinished;
        _gameManager.OnNextMoveRequired -= ContinueToMoveIfAllowed;

    }



    private void Update()
    {
        if (!_canControl) { return; }

        if(Input.GetKeyDown(KeyCode.W))
        {
            // Move forward
            MovePlayerForward();
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            // Guard or Use items?
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
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            // Menu or Cancel something
            PreventInput();
        }

    }

    private void RotatePlayer(Block.Rotations rotation)
    {
        playerBlock.RotateHorizontally(rotation);
    }

    private void MovePlayerForward()
    {
        playerBlock.MoveFoward();
    }

    private void AllowInput()
    {
        _canControl = true;
    }

    private void PreventInput()
    {
        _canControl = false;
    }

    private void ContinueToMoveIfAllowed(CharacterBlock compareBlock)
    {
        if(compareBlock != playerBlock) { return; }
        AllowInput();
    }

    private void CallPlayerIsFinished(CharacterBlock compareBlock)
    {
        if(compareBlock != playerBlock) { return ; }
        playerBlock.ResetCurrentMoves();
        if(OnPlayerIsFinished != null)
            OnPlayerIsFinished();
    }

    private void InitializePlayerBlock(CharacterPlane plane)
    {
        playerBlock = plane.GetPlayerBlock();
        AllowInput();
    }

}
