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
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.OnPlayerTurnStarted += AllowInput;
        CharacterPlane.OnCharacterPlaneInitialized += InitializePlayerBlock;
        // Temp
        _gameManager.OnLevelStarted += AllowInput;
    }

    private void OnDestroy()
    {
        _gameManager.OnPlayerTurnStarted -= AllowInput;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializePlayerBlock;
        // Temp
        _gameManager.OnLevelStarted -= AllowInput;
    }



    private void Update()
    {
        if (!_canControl) { return; }

        if(Input.GetKeyDown(KeyCode.W))
        {
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
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
            // Move forward
            MovePlayerForward();
            PreventInput();
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {

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

    private void InitializePlayerBlock(CharacterPlane plane)
    {
        playerBlock = plane.GetPlayerBlock();
        AllowInput();
    }

}
