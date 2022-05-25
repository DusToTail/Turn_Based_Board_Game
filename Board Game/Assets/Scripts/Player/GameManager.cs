using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridController gridController { get; private set; }
    public LevelPlane levelPlane { get; private set; }
    public CharacterPlane characterPlane { get; private set; }
    public PlayerController playerController { get; private set; }
    // Need to prepare grid controller
    // Need to prepare level plane
    // Need to prepare character plane
    public int prepCount = 3;
    public int currentPrepCount;

    public delegate void LevelStarted();
    public event LevelStarted OnLevelStarted;
    public delegate void LevelFinished();
    public event LevelFinished OnLevelFinished;

    public delegate void BlockStartedBehaviour(Block behavingBlock);
    public event BlockStartedBehaviour OnBlockStartedBehaviour;
    public delegate void BlockEndedBehaviour(Block behavingBlock);
    public event BlockEndedBehaviour OnBlockEndedBehaviour;

    public delegate void PlayerTurnStarted();
    public event PlayerTurnStarted OnPlayerTurnStarted;

    public void UpdateCharacterBlockPosition(Cell fromCell, Cell toCell)
    {
        GameObject block = characterPlane.grid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.z].block;
        characterPlane.grid[toCell.gridPosition.y, toCell.gridPosition.z, toCell.gridPosition.z].block = block;
        characterPlane.grid[fromCell.gridPosition.y, fromCell.gridPosition.z, fromCell.gridPosition.z].block = null;
    }

    public void CallBlockStartedBehaviour(Block behavingBlock)
    {
        if(OnBlockStartedBehaviour != null)
            OnBlockStartedBehaviour(behavingBlock);
    }

    public void CallBlockEndedBehaviour(Block behavingBlock)
    {
        if(OnBlockEndedBehaviour != null)
            OnBlockEndedBehaviour(behavingBlock);
    }

    public void CallLevelStarted()
    {
        if (OnLevelStarted != null)
            OnLevelStarted();
    }

    public void CallLevelFinished()
    {
        if(OnLevelFinished != null)
            OnLevelFinished();
    }

    public void CallPlayerTurnStarted()
    {
        if (OnPlayerTurnStarted != null)
            OnPlayerTurnStarted();
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;
    }

    private void IncrementPrepCount()
    {
        currentPrepCount++;
        if(currentPrepCount == prepCount)
        {
            if(OnLevelStarted != null)
                OnLevelStarted();
        }
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
    }

    private void InitializeLevelPlane(LevelPlane plane)
    {
        levelPlane = plane;
    }

    private void InitializeCharacterPlane(CharacterPlane plane)
    {
        characterPlane = plane;
    }
}
