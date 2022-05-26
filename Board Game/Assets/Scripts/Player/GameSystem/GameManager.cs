using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GridController gridController;
    public LevelPlane levelPlane;
    public CharacterPlane characterPlane;
    public PlayerController playerController;
    // Need to prepare grid controller
    // Need to prepare level plane
    // Need to prepare character plane
    public int prepCount = 3;
    public int currentPrepCount;

    public string fileName;
    public LevelDesign currentLevel;
    public delegate void LevelLoadingStarted(LevelDesign levelDesign);
    public event LevelLoadingStarted OnLevelLoadingStarted;

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
    public delegate void PlayerTurnEnded();
    public event PlayerTurnEnded OnPlayerTurnEnded;

    public delegate void AITurnStarted();
    public event AITurnStarted OnAITurnStarted;
    public delegate void AITurnEnded();
    public event AITurnEnded OnAITurnEnded;

    public delegate void CharacterRanOutOfMoves(CharacterBlock noMovesBlock);
    public event CharacterRanOutOfMoves OnCharacterRanOutOfMoves;
    public delegate void NextMoveRequired(CharacterBlock needMovesBlock);
    public event NextMoveRequired OnNextMoveRequired;

    public delegate void CharacterChangedPosition(CharacterBlock movedBlock, Cell toCell);
    public event CharacterChangedPosition OnCharacterChangedPosition;

    private void Start()
    {
        CallLevelLoadingStarted();
    }

    private void OnEnable()
    {
        GridController.OnGridInitialized += InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized += InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized += InitializeCharacterPlane;

        AIController.OnAIsAreFinished += CallAITurnEnded;
        OnAITurnEnded += CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished += CallPlayerTurnEnded;
        OnPlayerTurnEnded += CallAITurnStarted;

        OnLevelStarted += AIController.ResetAIsStats;
        OnAITurnStarted += AIController.ResetAIsMoves;

    }

    private void OnDisable()
    {
        GridController.OnGridInitialized -= InitializeGridController;
        LevelPlane.OnLevelPlaneInitialized -= InitializeLevelPlane;
        CharacterPlane.OnCharacterPlaneInitialized -= InitializeCharacterPlane;

        AIController.OnAIsAreFinished -= CallAITurnEnded;
        OnAITurnEnded -= CallPlayerTurnStarted;
        PlayerController.OnPlayerIsFinished -= CallPlayerTurnEnded;
        OnPlayerTurnEnded -= CallAITurnStarted;

        OnLevelStarted -= AIController.ResetAIsStats;
        OnAITurnStarted -= AIController.ResetAIsMoves;
    }

    

    public void CallLevelLoadingStarted()
    {
        if (gridController == null || levelPlane == null || characterPlane == null)
        {
            Debug.Log("Grid is not initialized in the editor");
            return;
        }

        currentLevel = new LevelDesign();
        LevelDesign saved = SaveSystem.LoadLevelDesign(fileName);
        currentLevel.gridHeight = saved.gridHeight;
        currentLevel.gridLength = saved.gridLength;
        currentLevel.gridWidth = saved.gridWidth;
        currentLevel.terrainGrid = saved.terrainGrid;
        currentLevel.characterGrid = saved.characterGrid;

        levelPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];
        characterPlane.idGrid = new int[currentLevel.gridHeight, currentLevel.gridLength, currentLevel.gridWidth];

        int count = 0;
        for (int h = 0; h < currentLevel.gridHeight; h++)
        {
            for (int l = 0; l < currentLevel.gridLength; l++)
            {
                for (int w = 0; w < currentLevel.gridWidth; w++)
                {
                    levelPlane.idGrid[h, l, w] = currentLevel.terrainGrid[count];
                    characterPlane.idGrid[h, l, w] = currentLevel.characterGrid[count];
                    count++;
                }
            }
        }

        Vector3Int gridSize = new Vector3Int(currentLevel.gridWidth, currentLevel.gridHeight, currentLevel.gridLength);
        gridController.InitializeGrid(gridSize);
    }

    public void CallLevelStarted()
    {
        Debug.Log("Level Started");
        if (OnLevelStarted != null)
            OnLevelStarted();
    }

    public void CallLevelFinished()
    {
        Debug.Log("Level Finished");
        if (OnLevelFinished != null)
            OnLevelFinished();

    }

    public void CallPlayerTurnStarted()
    {
        Debug.Log("Player Turn Started");
        if (OnPlayerTurnStarted != null)
            OnPlayerTurnStarted();

    }

    public void CallPlayerTurnEnded()
    {
        Debug.Log("Player Turn Ended");
        if (OnPlayerTurnEnded != null)
            OnPlayerTurnEnded();
    }

    public void CallAITurnStarted()
    {
        Debug.Log("AI Turn Started");
        if (OnAITurnStarted != null)
            OnAITurnStarted();
    }

    public void CallAITurnEnded()
    {
        Debug.Log("AI Turn Ended");
        if (OnAITurnEnded != null)
            OnAITurnEnded();
    }

    public void CallCharacterRanOutOfMoves(CharacterBlock noMovesBlock)
    {
        Debug.Log($"{noMovesBlock.name} ran out of moves");
        if (OnCharacterRanOutOfMoves != null)
            OnCharacterRanOutOfMoves(noMovesBlock);
    }

    public void CallNextMoveRequired(CharacterBlock needMovesBlock)
    {
        Debug.Log($"{needMovesBlock.name} need more moves");
        if (OnNextMoveRequired != null)
            OnNextMoveRequired(needMovesBlock);
    }

    public void CallCharacterChangedPosition(CharacterBlock block, Cell toCell)
    {
        Debug.Log($"{block.name}'s position is updated in Character Plane");
        characterPlane.UpdateCharacterPosition(block, toCell);

        // Currently not used Event
        if (OnCharacterChangedPosition != null)
            OnCharacterChangedPosition(block, toCell);
    }

    public void CallBlockStartedBehaviour(Block behavingBlock)
    {
        Debug.Log($"{behavingBlock.name}'s behaviour started");
        if (OnBlockStartedBehaviour != null)
            OnBlockStartedBehaviour(behavingBlock);
    }

    public void CallBlockEndedBehaviour(Block behavingBlock)
    {
        Debug.Log($"{behavingBlock.name}'s behaviour ended");
        if (OnBlockEndedBehaviour != null)
            OnBlockEndedBehaviour(behavingBlock);
    }

    private void InitializeGridController(GridController gridController)
    {
        this.gridController = gridController;
        IncrementPrepCount();
    }

    private void InitializeLevelPlane(LevelPlane plane)
    {
        levelPlane = plane;
        IncrementPrepCount();

    }

    private void InitializeCharacterPlane(CharacterPlane plane)
    {
        characterPlane = plane;
        IncrementPrepCount();

    }

    private void IncrementPrepCount()
    {
        currentPrepCount++;
        if (currentPrepCount == prepCount)
        {
            CallLevelStarted();
        }
    }
}
